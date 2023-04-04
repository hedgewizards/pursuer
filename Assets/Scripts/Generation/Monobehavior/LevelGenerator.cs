using Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;

namespace Generation
{
    [RequireComponent(typeof(AstarPath))]
    public class LevelGenerator : MonoBehaviour
    {
        public LevelKit LevelKit;

        public Dictionary<Vector3Int, int> gridOccupancy;
        [HideInInspector]
        public WatchedList<ChunkBundle> ChunkBundles;

        public Dictionary<GridEdge, int> GridEdgeOccupancy;
        [HideInInspector]
        public List<ChunkBoundary> ChunkBoundaries;
        [HideInInspector]
        public WatchedList<int> ActiveBoundaries;

        private void Start()
        {
            if (BuildOnAwake)
            {
                Build();
            }
        }

        #region Generation

        public bool BuildOnAwake = false;
        public GenerationStep[] Steps;

        public void Build()
        {
            BuildFromChildren();
            var seed = Pursuer.PursuerGameManager.GetSeed();
            LevelGenerationContext context = new LevelGenerationContext(this, seed);

            foreach(GenerationStep step in Steps)
            {
                for(int attempts = 0; attempts < step.Attempts; attempts++)
                {
                    Chunk Target = (attempts == 0 && step.PreferStartFromOrigin) ? ChunkBundles[0].Chunk : null;
                    if (GenerateLoop(context, Target, step.AllowedSteps, step.MinimumLoopSize))
                    {
                        break;
                    }
                }
            }

            context.Finished();
            SealExits();

            var pathfinder = GetComponent<AstarPath>();
            var recastGraph = pathfinder.graphs[0] as RecastGraph;
            if (recastGraph != null)
            {
                recastGraph.SnapForceBoundsToScene();
            }

            pathfinder.Scan();
        }

        public bool BuildFromChildren()
        {
            gridOccupancy = new Dictionary<Vector3Int, int>();
            ChunkBundles = new WatchedList<ChunkBundle>();

            GridEdgeOccupancy = new Dictionary<GridEdge, int>();
            ChunkBoundaries = new List<ChunkBoundary>();
            ActiveBoundaries = new WatchedList<int>();

            Chunk[] children = GetComponentsInChildren<Chunk>();

            foreach (Chunk chunk in children)
            {
                Vector3Int gridPos = LevelKit.PositionToGrid(chunk.transform.localPosition);

                Direction direction = new Direction(chunk.transform.forward);

                ChunkBundle bundle = TestChunk(chunk, gridPos, direction, TestSettings.InitialSeed);
                    
                if (bundle == null) return false;

                AddChunk(bundle);
            }

            return true;
        }

        public void GenerateFromChildren()
        {
            LevelGenerationContext context;

            if (!BuildFromChildren()) return;

            context = new LevelGenerationContext(this);

            context.Finished();
        }

        public Chunk GenerateAnyChunk(LevelGenerationContext context)
        {
            List<Chunk> templates = context.ChooseRandomTemplates(ChunkBundles.Count);
            List<int> boundaryIndexes = context.ChooseRandomValidBoundaries(ActiveBoundaries.Count);

            foreach (Chunk template in templates)
            {
                foreach(int boundaryIndex in boundaryIndexes)
                {
                    int testBoundaryIndex = context.Random.Next(0, template.ChunkBoundaries.Count);

                    //Debug.Log($"local:{boundaryIndex} to {template.name}:{testBoundaryIndex}");

                    if (LevelKit.BoundariesAreCompatible(template.ChunkBoundaries[testBoundaryIndex], ChunkBoundaries[boundaryIndex]))
                    {
                        Chunk newChunk = TestAndAddChunkTemplate(template, ChunkBoundaries[boundaryIndex].LevelEdge, testBoundaryIndex);
                        if (newChunk != null) return newChunk;
                    }
                }
            }

            return null;
        }

        public void GenerateGoodChunk(LevelGenerationContext context)
        {
            // find the absolute best one
            List<TestChunkBatchJob.ResultBundle> resultBundles = BatchTestForSingleChunk(context);

            TestChunkBatchJob.ResultBundle bestBundle = resultBundles[0];

            foreach (TestChunkBatchJob.ResultBundle resultBundle in resultBundles)
            {
                // update overall best boundary
                if (resultBundle.score < bestBundle.score)
                {
                    bestBundle = resultBundle;
                }
            }

            // add the best combination to the level
            AddChunkTemplateByBundle(bestBundle);
        }

        public Chunk AddChunkTemplateByBundle(TestChunkBatchJob.ResultBundle bundle)
        {

            GameObject templateCopy = Instantiate(bundle.ChunkBundle.Chunk.gameObject);
            Chunk newChunk = templateCopy.GetComponent<Chunk>();
            newChunk.gameObject.transform.SetParent(transform, true);
            bundle.ChunkBundle.Chunk = newChunk;

            newChunk.AlignToEdge(bundle.targetEdge, newChunk.ChunkBoundaries[bundle.aligningBoundaryIndex]);
            AddChunk(bundle.ChunkBundle);

            return newChunk;

        }
    
        public bool GenerateLoop(LevelGenerationContext context, Chunk target, int allowedSteps, int minimumLoopSize)
        {
            if (allowedSteps <= 0) return false;

            if (target == null)
            {
                foreach (ChunkBundle chunkBundle in pickGoodStartingChunks(context, 3))
                {
                    if (GenerateLoop(context, chunkBundle.Chunk, allowedSteps, minimumLoopSize)) return true;
                }
                return false;
            }

            int chunkGuesses = 4;

            List<Chunk> templates = context.ChooseRandomTemplates(chunkGuesses);

            // get boundaries in activeBoundaries attached to target
            List<int> boundaryIndexes = ActiveBoundaries
                .Where(i => ChunkBoundaries[i].transform.parent.GetComponent<Chunk>() == target)
                .ToList();

            // test these boundaries
            TestChunkBatchJob job = new TestChunkBatchJob(this, templates, boundaryIndexes);
            job.ExecuteAll();
            List<TestChunkBatchJob.ResultBundle> results = job.GetResults();

            // test in order of score
            results = results.OrderBy(r => r.score).ToList();

            foreach(TestChunkBatchJob.ResultBundle batchTestResult in results)
            {
                // since these are sorted, every other attempt failed so return
                if (batchTestResult.score == int.MaxValue) return false;

                // If we made a loop...
                if (batchTestResult.score == 0)
                {
                    // we can't use this loop if we violate the at-least-one rule making it
                    if (batchTestResult.ChunkBundle.SoftViolatesAtLeastOneRule)
                    {
                        continue;
                    }

                    // skip this bundle if we didn't make a big enough loop
                    if (calculateLargestLoopLength(context, batchTestResult) < minimumLoopSize)
                    {
                        continue;
                    }
                    else
                    {
                        // success!
                        AddChunkTemplateByBundle(batchTestResult);
                        return true;
                    }
                }

                // Add this bundle
                Chunk newChunk = AddChunkTemplateByBundle(batchTestResult);

                bool success = GenerateLoop(context, newChunk, allowedSteps - 1, minimumLoopSize);

                if (success)
                {
                    return true;
                }
                else
                {
                    DeleteChunk(ChunkBundles.Count - 1);
                }
            }

            // we ran out of boundaries. return false
            return false;
        }

        private int calculateLargestLoopLength(LevelGenerationContext context, TestChunkBatchJob.ResultBundle batchTestResult)
        {
            int largestLoop = 0;

            int attachingChunkIndex = ChunkBoundaries[GridEdgeOccupancy[batchTestResult.targetEdge]].ParentChunkIndex;

            (Vector3Int chunkGridPosition, RotationType chunkRotation) = batchTestResult.ChunkBundle.Chunk.CalculateAlignment(batchTestResult.targetEdge, batchTestResult.AligningBoundary);

            // for each resulting boundary
            foreach (ChunkBoundary newBoundary in batchTestResult.ChunkBundle.Chunk.ChunkBoundaries)
            {
                // convert to level space and find the sibling
                GridEdge sibling = newBoundary.LocalEdge.ToLevelSpace(chunkGridPosition, chunkRotation).Sibling;
                if (GridEdgeOccupancy.TryGetValue(sibling, out int siblingIndex))
                {
                    int loopingChunkIndex = ChunkBoundaries[siblingIndex].ParentChunkIndex;

                    if (loopingChunkIndex == attachingChunkIndex)
                    {
                        largestLoop = Math.Max(largestLoop, 2);
                    }
                    else
                    {
                        // Adding 2 to the loop length to account for the additional 2 steps to make a cycle across our new chunk
                        largestLoop = Math.Max(largestLoop, 2 + context.GetChunkDistance(attachingChunkIndex, loopingChunkIndex));
                    }
                    
                }
            }

            return largestLoop;
        }

        public List<TestChunkBatchJob.ResultBundle> BatchTestForSingleChunk(LevelGenerationContext context)
        {
            int boundaryGuesses = 5;
            int chunkGuesses = 7;

            // test a few combinations of chunks on boundaries and measure how good they are
            return BatchTestSingleChunks(context, boundaryGuesses, chunkGuesses);
        }

        private List<TestChunkBatchJob.ResultBundle> BatchTestSingleChunks(LevelGenerationContext context, int pairGuesses, int chunkGuesses)
        {

            List<int> bestBoundaryIndexes = context.GetBestBoundaries(pairGuesses);

            List<Chunk> templates = context.ChooseRandomTemplates(chunkGuesses);


            TestChunkBatchJob job = new TestChunkBatchJob(this, templates, bestBoundaryIndexes);
            job.ExecuteAll();

            return job.GetResults();
        }

        public void SealExits()
        {
            foreach(ChunkBoundary b in ChunkBoundaries)
            {
                b.Populate(this);
            }
        }

        private List<ChunkBundle> pickGoodStartingChunks(LevelGenerationContext context, int maxCount)
        {
            // First we need to check how many chunks violate the atLeastOne rule
            var chunks = findAtLeastOneViolatingChunks();

            if (chunks.Count == 0)
            {
                chunks = context.ChooseRandomChunks(maxCount);
            }
            else if(chunks.Count > maxCount)
            {
                chunks.RemoveRange(maxCount, chunks.Count - maxCount);
            }

            return chunks;
        }

        private List<ChunkBundle> findAtLeastOneViolatingChunks()
        {
            List<ChunkBundle> violatingChunks = new List<ChunkBundle>();

            foreach(ChunkBundle bundle in ChunkBundles)
            {
                // for each rule
                for (int ruleIndex = 0; ruleIndex < bundle.Chunk.Rules.Length; ruleIndex++)
                {
                    var rule = bundle.Chunk.Rules[ruleIndex];

                    // we only care about AtLeastOne rules
                    if (rule.Rule != ChunkRule.RuleType.AtLeastOne) continue;

                    bool ruleFollowed = false;

                    // check each related boundary to see if it's occupied
                    for (int m = 0; m < rule.RelatedBoundaries.Length; m++)
                    {
                        int relatedBoundaryIndex = rule.RelatedBoundaries[m];
                        GridEdge siblingEdge = bundle.Chunk.ChunkBoundaries[relatedBoundaryIndex].LevelEdge.Sibling;

                        // if this boundary is occupied, our rule is being followed so go to the next one
                        if (GetChunkBoundaryAt(siblingEdge) != null)
                        {
                            ruleFollowed = true;
                            break;
                        } 
                    }

                    if (!ruleFollowed)
                    {
                        violatingChunks.Add(bundle);
                        // go to the next chunk
                        break;
                    }
                }
            }

            return violatingChunks;
        }

        #region Generation - Chunk Testing
        
        public Chunk TestAndAddChunkTemplate(Chunk chunk, GridEdge targetEdge, int aligningBoundaryIndex)
        {
            ChunkBundle bundle = TestChunk(chunk, targetEdge, chunk.ChunkBoundaries[aligningBoundaryIndex]);
            if (bundle == null) return null;
            if (bundle.SoftViolatesAtLeastOneRule)
            {
                //Debug.Log($"failed to place {bundle.Chunk.name} at edge {aligningBoundaryIndex} due to atLeastOneRule violation");
                return null;
            }

            GameObject templateCopy = Instantiate(chunk.gameObject);
            Chunk newChunk = templateCopy.GetComponent<Chunk>();
            newChunk.gameObject.transform.SetParent(transform, true);
            bundle.Chunk = newChunk;

            newChunk.AlignToEdge(targetEdge, newChunk.ChunkBoundaries[aligningBoundaryIndex]);
            AddChunk(bundle);

            return newChunk;
        }

        public ChunkBundle TestChunk(Chunk newChunk, GridEdge targetEdge, ChunkBoundary aligningBoundary, TestSettings testSettings = null)
        {
            GridEdge targetSibling = targetEdge.Sibling;
            RotationType rotation = targetSibling.Direction.GetRotationFrom(aligningBoundary.LocalEdge.Direction);
            var position = targetSibling.Position - (aligningBoundary.LocalEdge.Position).Rotate(rotation);

            return testChunk(newChunk, position, rotation, testSettings?? TestSettings.DefaultValidation);
        }

        public ChunkBundle TestChunk(Chunk newChunk, Vector3Int position, Direction direction, TestSettings testSettings = null)
        {
            RotationType rotation = direction.GetRotationFrom(Direction.forward);

            return testChunk(newChunk, position, rotation, testSettings?? TestSettings.DefaultValidation);
        }

        ChunkBundle testChunk(Chunk newChunk, Vector3Int position, RotationType rotation, TestSettings testSettings)
        {
            RuleTestContext ruleContext = new RuleTestContext(this, testSettings);
            // convert localgridspaces to LevelSpace
            List<Vector3Int> levelPositions = new List<Vector3Int>();
            foreach (Vector3Int localPosition in newChunk.LocalGridSpaces)
            {
                var newPosition = localPosition.ToLevelSpace(position, rotation);

                // Check if any levelgridspaces intersect
                if (gridOccupancy.ContainsKey(newPosition))
                {
                    return null;
                }
                else
                {
                    levelPositions.Add(newPosition);
                }
            }

            // convert Chunk Boundaries to LevelSpace
            var levelEdges = new List<(int chunkBoundaryIndex, GridEdge gridEdge)>();
            for (int n = 0; n < newChunk.ChunkBoundaries.Count; n++)
            {
                GridEdge newEdge = newChunk.ChunkBoundaries[n].LocalEdge.ToLevelSpace(position, rotation);

                // Check if any of these chunk boundaries overlap in unallowed ways
                GridEdge sibling = newEdge.Sibling;
                if (GridEdgeOccupancy.ContainsKey(sibling))
                {
                    var thisBoundary = newChunk.ChunkBoundaries[n];
                    var otherBoundary = ChunkBoundaries[GridEdgeOccupancy[sibling]];
                    if (!LevelKit.BoundariesAreCompatible(thisBoundary, otherBoundary))
                    {
                        return null;
                    }
                    var otherChunk = ChunkBundles[otherBoundary.ParentChunkIndex].Chunk;

                    ruleContext.AddChunkPairing(newChunk, thisBoundary, otherChunk, otherBoundary);
                }

                levelEdges.Add((n, newEdge));
            }

            // test if we violated any chunk rules
            var result = ruleContext.Validate();
            if (!result.Validated) return null;

            var bundle = new ChunkBundle
            {
                Chunk = newChunk,
                LevelPositions = levelPositions,
                LevelEdgePairs = levelEdges,
                SoftViolatesAtLeastOneRule = result.SoftViolatesAtLeastOneRule
            };
            return bundle;
        }

        public bool AddChunk(ChunkBundle bundle)
        {
            int newChunkIndex = ChunkBundles.Count;
            bundle.boundaryIndexOffset = ChunkBoundaries.Count;

            //insert boundaries into data structures
            foreach ((int chunkBoundaryIndex, GridEdge levelEdge) edgePair in bundle.LevelEdgePairs)
            {
                addBoundary(bundle.Chunk.ChunkBoundaries[edgePair.chunkBoundaryIndex], edgePair.levelEdge, newChunkIndex);
            }

            //insert chunk into data structures
            ChunkBundles.Add(bundle);

            foreach (Vector3Int levelPosition in bundle.LevelPositions)
            {
                gridOccupancy[levelPosition] = newChunkIndex;
            }

            return true;
        }

        public void DeleteChunk(int ChunkIndex)
        {
            ChunkBundle bundleToDelete = ChunkBundles[ChunkIndex];

            // remove boundaries associated with this chunk
            List<int> reAddBoundaries = new List<int>();
            for (int n = bundleToDelete.Chunk.ChunkBoundaries.Count - 1; n >= 0; n--)
            {
                int boundaryIndex = n + bundleToDelete.boundaryIndexOffset;
                ChunkBoundary removeBoundary = ChunkBoundaries[boundaryIndex];

                // remove this boundary from ActiveBoundaries
                if (ActiveBoundaries.Contains(boundaryIndex))
                {
                    ActiveBoundaries.Remove(boundaryIndex);
                }

                // Track the siblings of these boundaries to re-add in a second
                if (GridEdgeOccupancy.TryGetValue(removeBoundary.LevelEdge.Sibling, out int siblingIndex))
                {
                    reAddBoundaries.Add(siblingIndex);
                }

                // remove this boundary from GridEdgeOccupancy
                GridEdgeOccupancy.Remove(removeBoundary.LevelEdge);

                // remove this boundary from ChunkBoundaries
                ChunkBoundaries.RemoveAt(boundaryIndex);
            }

            // Add the reactivated boundaries to ActiveBoundaries
            foreach (int boundaryIndex in reAddBoundaries)
            {
                ActiveBoundaries.Add(boundaryIndex);
            }

            // convert localgridspaces to LevelSpace
            foreach (Vector3Int levelPosition in bundleToDelete.LevelPositions)
            {
                gridOccupancy.Remove(levelPosition);
            }

            // remove the chunk from Chunks
            ChunkBundles.RemoveAt(ChunkIndex);

            // delete this chunk
            DestroyImmediate(bundleToDelete.Chunk.gameObject);
        }

        #endregion

        #endregion

        #region Chunks
        public Chunk GetChunkAt(Vector3Int pos)
        {
            if (gridOccupancy.ContainsKey(pos))
            {
                return ChunkBundles[gridOccupancy[pos]].Chunk;
            }
            else
            {
                return null;
            }
        }

        public int GetChunkIndex(Chunk chunk)
        {
            for (int n = 0; n < ChunkBundles.Count; n++)
            {
                if (ChunkBundles[n].Chunk == chunk) return n;
            }

            return -1;
        }

        #endregion

        #region Boundaries

        public ChunkBoundary GetChunkBoundaryAt(GridEdge gridEdge)
        {
            if (GridEdgeOccupancy.ContainsKey(gridEdge))
            {
                return ChunkBoundaries[GridEdgeOccupancy[gridEdge]];
            }
            else
            {
                return null;
            }
        }

        void addBoundary(ChunkBoundary chunkBoundary, GridEdge levelEdge, int parentChunkIndex)
        {
            chunkBoundary.ApplyParent(levelEdge, parentChunkIndex);
            int newBoundaryIndex = ChunkBoundaries.Count;
            ChunkBoundaries.Add(chunkBoundary);
            GridEdgeOccupancy[levelEdge] = newBoundaryIndex;
            activateBoundary(newBoundaryIndex);
        }

        void activateBoundary(int boundaryIndex)
        {
            // check if the sibling edge is already active
            GridEdge sibling = ChunkBoundaries[boundaryIndex].LevelEdge.Sibling;
            if (GridEdgeOccupancy.ContainsKey(sibling))
            {
                // if there's a sibling, remove both from the active boundaries list
                ActiveBoundaries.Remove(GridEdgeOccupancy[sibling]);
            }
            else
            {
                ActiveBoundaries.Add(boundaryIndex);
            }
        }
        #endregion

        #region Debug
#if UNITY_EDITOR
        static Color boundaryGizmoColorExternal = new Color(1, 0, 0, 0.5f);
        static Color boundaryGizmoColorInternal = new Color(1f, 0.5f, 0.5f, 0.5f);
        private void OnDrawGizmosSelected()
        {
            if (ChunkBundles == null || gridOccupancy == null) return;

            Gizmos.matrix = transform.localToWorldMatrix;

            // Draw Boxes
            float parametricColorDelta = 1f / ChunkBundles.Count;

            foreach(KeyValuePair<Vector3Int, int> kv in gridOccupancy)
            {
                float t = parametricColorDelta * kv.Value;
                Color rgb = Color.HSVToRGB(t, 1, 1); //new Color(1 - t, 1, t, 0.5f);
                Gizmos.color = new Color(rgb.r, rgb.g, rgb.b, 0.5f);
                var levelPos = Vector3.Scale(kv.Key, LevelKit.GridDimensions);
                Vector3 cubePos = levelPos
                    + (Vector3.up * LevelKit.GridDimensions.y / 2);
                Gizmos.DrawCube(cubePos, LevelKit.GridDimensions);
            }

            // Draw Boundaries
            foreach (KeyValuePair<GridEdge, int> kv in GridEdgeOccupancy)
            {
                Gizmos.color = ActiveBoundaries.Contains(kv.Value) ? boundaryGizmoColorExternal : boundaryGizmoColorInternal;
                GridEdge edge = kv.Key;
                Vector3 pos = edge.Position
                    + Vector3.up * 0.5f
                    + (Vector3)edge.Direction.Vector * 0.5f;
                pos = Vector3.Scale(pos, LevelKit.GridDimensions);
                bool directionIsForwardOrBackward = edge.Direction.Vector.z != 0;
                Gizmos.DrawCube(pos, new Vector3
                {
                    x = LevelKit.GridDimensions.x * (directionIsForwardOrBackward ? 1.3f : 0.1f),
                    y = LevelKit.GridDimensions.y * 1.1f,
                    z = LevelKit.GridDimensions.z * (directionIsForwardOrBackward ? 0.1f : 1.3f)
                });

                // text label
                Gizmos.color = Color.white;
                Handles.Label(pos, kv.Value.ToString());
            }
        }
#endif
        #endregion
    }


    /// <summary>
    /// Given a list of testChunks to apply to a context at a set of boundaries,
    /// score each testChunk/boundary pair based on the best resulting edge heuristic
    /// </summary>
    public struct TestChunkBatchJob
    {
        LevelGenerator self;

        List<Chunk> testChunks;
        List<int> existingBoundaryIndexes;
        int[] perBoundaryChunkResultsStartIndex;
        int perBoundaryResultCount;

        /// <summary>
        /// an array containing the results for each Chunk and their ChunkBoundaries. <br/>
        /// each chunk stores its boundaries in this array starting at perBoundaryChunkResultsStartIndex in ascending order
        /// </summary>
        ResultBundle[] rawResults;

        public enum RulePriorityType
        {
            KeepValid,
            PreferFix
        }
        RulePriorityType rulePriority;

        public struct ResultBundle
        {
            public ChunkBundle ChunkBundle;
            public GridEdge targetEdge;
            public int aligningBoundaryIndex;
            public int score;

            public ChunkBoundary AligningBoundary => ChunkBundle.Chunk.ChunkBoundaries[aligningBoundaryIndex];

            public ResultBundle(ChunkBundle chunkBundle, GridEdge _targetEdge, int _aliginingBoundaryIndex, int _score)
            {
                ChunkBundle = chunkBundle;
                targetEdge = _targetEdge;
                aligningBoundaryIndex = _aliginingBoundaryIndex;
                score = _score;
            }

            public static ResultBundle MaxResult => new ResultBundle(null, GridEdge.Zero, -1, int.MaxValue);
        }


        public TestChunkBatchJob(LevelGenerator _self, List<Chunk> _testChunks, List<int> _existingBoundaryIndexes, RulePriorityType _rulePriority = RulePriorityType.KeepValid)
        {
            self = _self;
            testChunks = _testChunks;
            existingBoundaryIndexes = _existingBoundaryIndexes;
            rulePriority = _rulePriority;

            // calculate result starting index for each chunk
            perBoundaryChunkResultsStartIndex = new int[_testChunks.Count];
            int index = 0;
            for (int n = 0; n < _testChunks.Count; n++)
            {
                perBoundaryChunkResultsStartIndex[n] = index;
                index += testChunks[n].ChunkBoundaries.Count;
            }

            perBoundaryResultCount = index;
            rawResults = new ResultBundle[existingBoundaryIndexes.Count * perBoundaryResultCount];
        }

        public void ExecuteAll()
        {
            // for each chunk
            for (int ci = 0; ci < testChunks.Count; ci++)
            {
                Execute(ci);
            }
        }

        public void Execute(int index)
        {
            var testChunk = testChunks[index];

            // for each existingBoundary
            for (int existingBoundaryIndex = 0; existingBoundaryIndex < existingBoundaryIndexes.Count; existingBoundaryIndex++)
            {
                ChunkBoundary existingBoundary = self.ChunkBoundaries[existingBoundaryIndexes[existingBoundaryIndex]];
                GridEdge existingEdge = existingBoundary.LevelEdge;

                // for each exposed boundary on the test chunk
                for (int aligningBoundaryIndex = 0; aligningBoundaryIndex < testChunk.ChunkBoundaries.Count; aligningBoundaryIndex++)
                {
                    ChunkBoundary aligningBoundary = testChunk.ChunkBoundaries[aligningBoundaryIndex];
                    int resultIndex = existingBoundaryIndex * perBoundaryResultCount + perBoundaryChunkResultsStartIndex[index] + aligningBoundaryIndex;

                    if (!self.LevelKit.BoundariesAreCompatible(existingBoundary, aligningBoundary))
                    {
                        rawResults[resultIndex] = ResultBundle.MaxResult;
                        continue;
                    }

                    // try to line the 2 boundaries up
                    ChunkBundle bundle = self.TestChunk(testChunk, existingEdge, aligningBoundary);
                    if (bundle == null)
                    {
                        // if it doesn't fit, go to the next boundary
                        rawResults[resultIndex] = ResultBundle.MaxResult;
                        continue;
                    }

                    // the result is the cost of the cheapest new boundary pair from this test
                    int best = CalculateBestHeuristic(existingBoundaryIndex, aligningBoundaryIndex, bundle);

                    rawResults[resultIndex] = new ResultBundle(bundle, existingEdge, aligningBoundaryIndex, best);
                }
            }


        }

        private int CalculateBestHeuristic(int existingEdgeIndex, int aligningBoundaryIndex, ChunkBundle bundle)
        {
            var self = this.self;
            int best = int.MaxValue;
            foreach ((int OtherNewBoundaryIndex, GridEdge gridEdge) pair
                in bundle.LevelEdgePairs.Where(p => !p.chunkBoundaryIndex.Equals(aligningBoundaryIndex)))
            {
                var otherExistingBoundaries = self.ActiveBoundaries
                    .Where(i => i != existingEdgeIndex)
                    .Where(i => self.LevelKit.BoundariesAreCompatible(
                        bundle.Chunk.ChunkBoundaries[pair.OtherNewBoundaryIndex],
                        self.ChunkBoundaries[i]));

                foreach (int otherExistingBoundaryIndex in otherExistingBoundaries)
                {
                    int cost = self.LevelKit.CalculateEdgeHeuristic(pair.gridEdge, self.ChunkBoundaries[otherExistingBoundaryIndex].LevelEdge.Sibling);
                    best = Math.Min(best, cost);
                }
            }

            return best;
        }

        public List<ResultBundle> GetResults()
        {
            List<ResultBundle> resultList = new List<ResultBundle>();

            // for each chunk
            for (int ci = 0; ci < testChunks.Count; ci++)
            {
                // for each existingBoundary
                for (int e = 0; e < existingBoundaryIndexes.Count; e++)
                {
                    // for each exposed boundary on the test chunk
                    for (int t = 0; t < testChunks[ci].ChunkBoundaries.Count; t++)
                    {
                        resultList.Add(rawResults[e * perBoundaryResultCount + perBoundaryChunkResultsStartIndex[ci] + t]);
                    }
                }
            }

            return resultList;
        }
    }
}
