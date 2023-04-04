using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Generation
{
    /// <summary>
    /// Used to optimize level generation calculations <br/><br/>
    /// Unhandled behavior if elements are removed from ChunkBoundaries
    /// </summary>
    public class LevelGenerationContext
    {
        public System.Random Random;
        LevelGenerator self;
        public LevelKit LevelKit;
        public List<Chunk> Templates;

        TriangleArray<int> edgeHeuristicResults;
        TriangleArray<int> ChunkDistances;

        public LevelGenerationContext(LevelGenerator _levelGenerator, int? seed = null)
        {
            int chosenSeed = seed.GetValueOrDefault(DateTime.Now.GetHashCode());
            Debug.Log($"Seed for that run: {chosenSeed}");
            Random = new System.Random(chosenSeed);
            self = _levelGenerator;
            LevelKit = _levelGenerator.LevelKit;
            Templates = instantiateChunkTemplates();
            initializeEdgeHeuristic();
            initializeChunkDistances();

            self.ActiveBoundaries.OnAdd += addNewestBoundary;
            self.ChunkBundles.OnAdd += addNewestChunk;
            self.ChunkBundles.OnRemove += (sender, i) =>
            {
                if (GetAdjacentChunks(i).Count > 1)
                {
                    //  this might work
                    //initializeChunkDistances();
                    throw new NotImplementedException("Removing a chunk that closes a loop invalidates ChunkDistances");
                }
            };
        }

        public void Finished()
        {
            if (Templates != null)
            {
                foreach (Chunk template in Templates)
                {
                    UnityEngine.Object.DestroyImmediate(template.gameObject);
                }
                Templates = null;
            }

            self.ActiveBoundaries.OnAdd -= addNewestBoundary;
        }

        #region Templates
        List<Chunk> instantiateChunkTemplates()
        {
            List<Chunk> ChunkTemplates = new List<Chunk>();
            foreach (GameObject chunkPrefab in LevelKit.ChunkPrefabs)
            {
                Chunk chunk = UnityEngine.Object.Instantiate<GameObject>(chunkPrefab).GetComponent<Chunk>();
                if (chunk == null)
                {
                    Debug.LogWarning($"{chunkPrefab.name} has no Chunk monobehavior");
                    continue;
                }
                ChunkTemplates.Add(chunk);
            }

            return ChunkTemplates;
        }

        public List<Chunk> ChooseRandomTemplates(int count)
        {
            count = Mathf.Min(count, Templates.Count);

            List<Chunk> picks = new List<Chunk>();
            for (int n = 0; n < count; n++)
            {
                // n <= choice < Templates.Count
                int choice = Random.Next(n, Templates.Count);

                Chunk temp = Templates[choice];
                Templates[choice] = Templates[n];
                Templates[n] = temp;

                picks.Add(temp);
            }

            return picks;
        }

        #endregion

        #region Chunks

        //TODO: Call this if we remove a chunk with multiple neighbors
        void initializeChunkDistances()
        {
            ChunkDistances = new TriangleArray<int>(self.ChunkBundles.Count);

            foreach (KeyValuePair<TriPair, int> pair in ChunkDistances)
            {
                ChunkDistances[pair.Key] = int.MaxValue;
            }

            // First find out which chunks are adjacent
            foreach (ChunkBoundary chunkBoundary in self.ChunkBoundaries)
            {
                if (self.GridEdgeOccupancy.TryGetValue(chunkBoundary.LevelEdge.Sibling, out int siblingIndex))
                {
                    TriPair thisAdjacency = new TriPair(chunkBoundary.ParentChunkIndex, self.ChunkBoundaries[siblingIndex].ParentChunkIndex);
                    ChunkDistances[thisAdjacency] = 1;
                }
            }

            List<int> visited = new List<int>();

            // BFS for each node based on our adjacency
            for (int myIndex = 0; myIndex < self.ChunkBundles.Count - 1; myIndex++)
            {
                visited.Clear();
                visited.Add(myIndex);
                int checkStartIndex = 0;
                int checkEndIndex = 0;
                int distance = 0;

                // while there's more nodes to check in the BFS
                while (checkEndIndex != visited.Count)
                {
                    // Update which nodes in visited to check, and the distance
                    distance++;
                    checkStartIndex = checkEndIndex;
                    checkEndIndex = visited.Count;

                    // iterate through the nodes in visited
                    for (int n = checkStartIndex; n < checkEndIndex; n++)
                    {
                        // find neighbors of visited (ignore nodes of smaller index)
                        int visitingIndex = visited[n];
                        for (int neighborIndex = 0; neighborIndex < self.ChunkBundles.Count; neighborIndex++)
                        {
                            // if this is a neighbor of check and hasn't already been visited
                            if (visitingIndex != neighborIndex
                                && ChunkDistances[visitingIndex, neighborIndex] == 1
                                && !visited.Contains(neighborIndex))
                            {
                                // mark its distance
                                ChunkDistances[myIndex, neighborIndex] = Math.Min(distance, ChunkDistances[myIndex, neighborIndex]);

                                // add it to visited
                                visited.Add(neighborIndex);
                            }
                        }
                    }
                }
            }
        }

        public int GetChunkDistance(int index1, int index2)
        {
            return ChunkDistances[index1, index2];
        }

        public List<int> GetAdjacentChunks(int chunkIndex)
        {
            List<int> adjacentChunks = new List<int>();

            for (int otherChunkIndex = 0; otherChunkIndex < self.ChunkBundles.Count; otherChunkIndex++)
            {
                if (chunkIndex == otherChunkIndex) continue;

                if (ChunkDistances[chunkIndex, otherChunkIndex] == 1)
                {
                    adjacentChunks.Add(otherChunkIndex);
                }
            }

            return adjacentChunks;
        }

        void addNewestChunk(object sender, EventArgs e)
        {
            int myIndex = self.ChunkBundles.Count - 1;

            // first, make sure ChunkDistances for the new chunk match up
            if (ChunkDistances.Size < self.ChunkBundles.Count)
            {
                ChunkDistances.Grow();
            }

            // set all the rows for this bundle to int.maxvalue
            for (int n = 0; n < myIndex; n++)
            {
                // note: this will fail if newIndex isn't the newest ChunkBundle (fine for now)
                ChunkDistances.SetAtUnsafe(n, myIndex, int.MaxValue);
            }

            // First find out which chunks are adjacent
            for (int n = self.ChunkBundles[myIndex].boundaryIndexOffset; n < self.ChunkBoundaries.Count; n++)
            {
                if (self.GridEdgeOccupancy.TryGetValue(self.ChunkBoundaries[n].LevelEdge.Sibling, out int siblingIndex))
                {
                    TriPair thisAdjacency = new TriPair(myIndex, self.ChunkBoundaries[siblingIndex].ParentChunkIndex);
                    ChunkDistances[thisAdjacency] = 1;
                }
            }

            // go through all neighbors and find/apply their best distances
            for (int neighborIndex = 0; neighborIndex < self.ChunkBundles.Count - 1; neighborIndex++)
            {
                // note: this will fail if newIndex isn't the newest ChunkBundle (fine for now)

                if (ChunkDistances[myIndex, neighborIndex] == 1)
                {
                    for (int otherIndex = 0; otherIndex < self.ChunkBundles.Count - 1; otherIndex++)
                    {
                        // note: this will fail if newIndex isn't the newest ChunkBundle (fine for now)

                        // if this isn't already our neighbor and the distance isn't undefined,
                        if (neighborIndex != otherIndex && ChunkDistances[neighborIndex, otherIndex] != int.MaxValue)
                        {
                            // copy the distance + 1 if its shorter than what we already have
                            ChunkDistances[otherIndex, myIndex] = Math.Min(ChunkDistances[otherIndex, myIndex], ChunkDistances[neighborIndex, otherIndex] + 1);
                        }
                    }
                }
            }

            // account for improved routes through newChunk
            for (int neighborIndex = 0; neighborIndex < self.ChunkBundles.Count - 1; neighborIndex++)
            {
                // note: this will fail if newIndex isn't the newest CHunkBundle (fine for now)
                int distanceToNeighbor = ChunkDistances.AtUnsafe(neighborIndex, myIndex);
                for (int otherIndex = 0; otherIndex < self.ChunkBundles.Count - 1; otherIndex++)
                {
                    if (otherIndex != neighborIndex && ChunkDistances[myIndex, otherIndex] != int.MaxValue)
                    {
                        int currentDistance = ChunkDistances[neighborIndex, otherIndex];
                        ChunkDistances[neighborIndex, otherIndex] = Math.Min(currentDistance, ChunkDistances[myIndex, otherIndex] + distanceToNeighbor);
                    }
                }
            }
        }

        public List<ChunkBundle> ChooseRandomChunks(int count)
        {
            count = Mathf.Min(count, self.ChunkBundles.Count);

            ChunkBundle[] bundles = self.ChunkBundles.ToArray();

            List<ChunkBundle> picks = new List<ChunkBundle>();
            for (int n = 0; n < count; n++)
            {
                // n <= choice < Templates.Count
                int choice = Random.Next(n, self.ChunkBundles.Count);

                ChunkBundle temp = bundles[choice];
                bundles[choice] = bundles[n];
                bundles[n] = temp;

                picks.Add(temp);
            }

            return picks;
        }

        #endregion

        #region Edges

        public List<int> ChooseRandomValidBoundaries(int count)
        {
            // first we should see if we have any REQUIRED boundaries
            (List<int> fulfillingBoundaries, List<int> validBoundaries) = TestBoundaryRules();


            int[] boundariesCopy = fulfillingBoundaries.Count > 0 ? fulfillingBoundaries.ToArray()
                                                                  : validBoundaries.ToArray();
            count = Mathf.Min(count, boundariesCopy.Length);
            List<int> results = new List<int>();

            for (int n = 0; n < count; n++)
            {
                // n <= choice < count
                int choice = Random.Next(n, boundariesCopy.Length);

                int temp = boundariesCopy[choice];
                boundariesCopy[choice] = boundariesCopy[n];
                boundariesCopy[n] = temp;

                results.Add(temp);
            }

            return results;
        }

        public List<int> GetBestBoundaries(int count)
        {
            // first we should see if we have any REQUIRED boundaries
            (List<int> fulfillingBoundaries, List<int> validBoundaries) = TestBoundaryRules();

            if (fulfillingBoundaries.Count > 0)
            {
                if (fulfillingBoundaries.Count <= count) return fulfillingBoundaries;
                fulfillingBoundaries.RemoveRange(count - 1, fulfillingBoundaries.Count - count);
                return fulfillingBoundaries;
            }

            // If we need more boundaries than there are active ones, just return all our active ones
            if (count > validBoundaries.Count)
            {
                return validBoundaries;
            }
            else
            {
                List<TriPair> bestPairs = GetSortedTriPairs();

                bool[] indexIsValidChoice = new bool[self.ActiveBoundaries.Count]; // = false
                foreach (int validBoundaryIndex in validBoundaries)
                {
                    indexIsValidChoice[validBoundaryIndex] = true;
                }

                List<int> bestIndexesInOrder = new List<int>();

                foreach (TriPair pair in bestPairs)
                {
                    if (indexIsValidChoice[pair.m])
                    {
                        if (indexIsValidChoice[pair.n] && Random.Next() % 2 == 1)
                        {
                            bestIndexesInOrder.Add(pair.n);
                            indexIsValidChoice[pair.n] = false;
                        }
                        else
                        {
                            bestIndexesInOrder.Add(pair.m);
                            indexIsValidChoice[pair.m] = false;
                        }
                    }
                    else if (indexIsValidChoice[pair.n])
                    {
                        bestIndexesInOrder.Add(pair.n);
                        indexIsValidChoice[pair.m] = false;
                    }

                    // we only care about the first $count indexes
                    if (bestIndexesInOrder.Count >= count) break;
                }

                return bestIndexesInOrder;
            }
            
        }

        private (List<int> fulfillingBoundaries, List<int> validBoundaries) TestBoundaryRules()
        {
            bool[][] boundaryOverlaps = new bool[self.ChunkBundles.Count][];
            List<int> fulfillingBoundaries = new List<int>();
            List<int> validBoundaries = new List<int>();

            foreach(int activeBoundaryIndex in self.ActiveBoundaries)
            {
                ChunkBoundary activeBoundary = self.ChunkBoundaries[activeBoundaryIndex];

                int chunkIndex = activeBoundary.ParentChunkIndex;
                ChunkBundle chunkBundle = self.ChunkBundles[chunkIndex];
                int localActiveBoundaryIndex = chunkBundle.Chunk.GetIndexOfBoundary(activeBoundary);

                if (boundaryOverlaps[chunkIndex] == null)
                {
                    // lazily initialize boundaryOverlaps for this chunk
                    boundaryOverlaps[chunkIndex] = new bool[chunkBundle.Chunk.ChunkBoundaries.Count];
                    for(int n = 0; n < chunkBundle.Chunk.ChunkBoundaries.Count; n++)
                    {
                        var sibling = chunkBundle.Chunk.ChunkBoundaries[n].LevelEdge.Sibling;
                        boundaryOverlaps[chunkIndex][n] = self.GridEdgeOccupancy.ContainsKey(sibling);
                    }
                }

                bool violatesRule = false;
                bool fulfillsRule = false;
                // check if using this boundary would solve an AtLeastOne rule and wouldn't break any AtMostOne rules
                foreach (ChunkRule rule in chunkBundle.Chunk.Rules)
                {
                    bool relatedToActive = false;
                    int relatedOverlaps = 0;
                    foreach(int localBoundaryIndex in rule.RelatedBoundaries)
                    {
                        if (boundaryOverlaps[chunkIndex][localBoundaryIndex])
                        {
                            relatedOverlaps++;
                        }
                        if (localBoundaryIndex == localActiveBoundaryIndex)
                            relatedToActive = true;
                    }

                    if (!relatedToActive) continue;

                    switch (rule.Rule)
                    {
                        case ChunkRule.RuleType.AtLeastOne:
                            if (relatedOverlaps == 0) fulfillsRule = true;
                            break;
                        case ChunkRule.RuleType.AtMostOne:
                            if (relatedOverlaps > 0) violatesRule = true;
                            break;
                    }
                }

                if (!violatesRule)
                {
                    if (fulfillsRule)
                    {
                        fulfillingBoundaries.Add(activeBoundaryIndex);
                    }
                    validBoundaries.Add(activeBoundaryIndex);
                }
            }

            return (fulfillingBoundaries, validBoundaries);
        }

        public List<TriPair> GetSortedTriPairs()
        {
            int count = self.ActiveBoundaries.Count * (self.ActiveBoundaries.Count + 1) / 2;
            LiteSortedList<int, TriPair> bestPairs = new LiteSortedList<int, TriPair>();

            // for each pair of boundaries in ActiveBoundaries
            for (int nIndex = 0; nIndex < self.ActiveBoundaries.Count - 1; nIndex++)
            {
                int n = self.ActiveBoundaries[nIndex];
                for (int mIndex = nIndex + 1; mIndex < self.ActiveBoundaries.Count - 1; mIndex++)
                {
                    int m = self.ActiveBoundaries[mIndex];
                    TriPair thisPair = new TriPair(n, m);
                    int edgeHeuristic = edgeHeuristicResults.AtUnsafe(n, m);
                    int distanceBonus = ChunkDistances[
                        self.ChunkBoundaries[n].ParentChunkIndex,
                        self.ChunkBoundaries[m].ParentChunkIndex];

                    int thisResult = edgeHeuristic + (distanceBonus == int.MaxValue ? self.ChunkBoundaries.Count : distanceBonus);
                    if (bestPairs.Count < count)
                    {
                        bestPairs.OrderedInsert(thisResult, thisPair);
                    }
                    else if (thisResult < bestPairs.Last.Key)
                    {
                        bestPairs.OrderedInsert(thisResult, thisPair);
                        bestPairs.DropLast();
                    }
                }
            }

            return bestPairs.ToList().Select(p => p.Value).ToList();
        }

        public List<int> getBestBoundariesFromPairs(List<TriPair> orderedTriPairs, int count)
        {
            List<int> picks = new List<int>();

            foreach(TriPair t in orderedTriPairs)
            {
                if (picks.Count >= count) return picks;

                if (picks.Contains(t.n))
                {
                    if (picks.Contains(t.m))
                    {
                        continue;
                    }
                    else
                    {
                        picks.Add(t.m);
                    }
                }
                else
                {
                    picks.Add(t.n);
                }
            }

            return picks;
        }

        void initializeEdgeHeuristic()
        {
            edgeHeuristicResults = new TriangleArray<int>(self.ChunkBoundaries.Count);

            for (int nIndex = 0; nIndex < self.ActiveBoundaries.Count - 1; nIndex++)
            {
                int n = self.ActiveBoundaries[nIndex];
                for (int mIndex = nIndex + 1; mIndex < self.ActiveBoundaries.Count - 1; mIndex++)
                {
                    int m = self.ActiveBoundaries[mIndex];
                    calculateEdgeHeuristic(n, m);
                }
            }
        }

        private void addNewestBoundary(object sender, EventArgs e)
        {
            int newIndex = self.ChunkBoundaries.Count - 1;

            if (edgeHeuristicResults.Size < self.ChunkBoundaries.Count)
            {
                edgeHeuristicResults.Grow();
            }

            for (int n = 0; n < newIndex - 1; n++)
            {
                if (!self.ActiveBoundaries.Contains(n)) continue;
                calculateEdgeHeuristic(n, newIndex);
            }
        }

        void calculateEdgeHeuristic(int n, int m)
        {
            edgeHeuristicResults[n, m] = LevelKit.CalculateEdgeHeuristic(
                        self.ChunkBoundaries[n].LevelEdge,
                        self.ChunkBoundaries[m].LevelEdge.Sibling);
        }
        #endregion

        #region destruction

        ~LevelGenerationContext()
        {
            if (Templates != null)
            {
                foreach (Chunk template in Templates)
                {
                    UnityEngine.Object.DestroyImmediate(template.gameObject);
                }
                Templates = null;
            }
        }

        #endregion

        #region Random
        public int[] ChooseRandomInts(int count)
        {
            // generate a sequential list of numbers 0->count-1
            int[] picks = new int[count];
            for (int n = 0; n < count; n++)
            {
                picks[n] = n;
            }

            // shuffle it
            for (int n = 0; n < count; n++)
            {
                // n <= choice < count
                int choice = Random.Next(n, count);

                int temp = picks[choice];
                picks[choice] = picks[n];
                picks[n] = temp;
            }

            return picks;
        }
        #endregion
    }
}
