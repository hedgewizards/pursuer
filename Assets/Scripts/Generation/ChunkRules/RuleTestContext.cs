using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation
{
    public class RuleTestContext
    {
        LevelGenerator self;
        TestSettings testSettings;

        /// <summary>
        /// the prospective occupancy of the boundaries of changed chunks in self<br/>
        /// If a chunk is a key in this dictionary, its value is the indexes of all its paired boundaries
        /// </summary>
        Dictionary<Chunk, List<int>> UpdatedChunks;

        public RuleTestContext(LevelGenerator self, TestSettings testSettings)
        {
            this.self = self;
            this.testSettings = testSettings;
            UpdatedChunks = new Dictionary<Chunk, List<int>>();
        }

        /// <summary>
        /// check if the rules are still followed given our prospective chunk pairings
        /// </summary>
        /// <returns></returns>
        public TestValidationResult Validate()
        {
            TestValidationResult result = new TestValidationResult() { Validated = true };

            // For each chunk that's been changed, check if we violate any of their rules
            foreach(KeyValuePair<Chunk, List<int>> kv in UpdatedChunks)
            {
                var chunk = kv.Key;
                var occupiedBoundaryIndexes = kv.Value;

                // if this particular chunk has no rules, skip it
                if (chunk.Rules.Length == 0) continue;

                // how many "at least one" rules have we violated for this chunk? we can stand to violate 1 and not worry about it (we will have to fix it when we place our next chunk)
                int violatedAtLeastOneRules = 0;
                
                // for each rule
                for (int ruleIndex = 0; ruleIndex < chunk.Rules.Length; ruleIndex++)
                {
                    int occupiedCount = 0;

                    // check each related boundary to see if it's occupied
                    for (int m = 0; m < chunk.Rules[ruleIndex].RelatedBoundaries.Length; m++)
                    {

                        for (int n = 0; n < occupiedBoundaryIndexes.Count; n++)
                        {
                            if (occupiedBoundaryIndexes[n] == chunk.Rules[ruleIndex].RelatedBoundaries[m])
                            {
                                occupiedCount++;
                                break;
                            }

                        }
                    }

                    // check if we violated it
                    switch (chunk.Rules[ruleIndex].Rule)
                    {
                        case ChunkRule.RuleType.AtLeastOne:
                            if (occupiedCount < 1)
                            {
                                violatedAtLeastOneRules++;
                                result.SoftViolatesAtLeastOneRule = true;
                                if (violatedAtLeastOneRules > testSettings.MaxAtLeastRuleBreaks)
                                {
                                    result.Validated = false;
                                    return result;
                                };
                            }
                            break;
                        case ChunkRule.RuleType.AtMostOne:
                            if (occupiedCount > 1)
                            {
                                result.Validated = false;
                                return result;
                            }
                            break;
                    }
                }

            }

            return result;
        }

        public void AddChunkPairing(Chunk chunkOne, ChunkBoundary boundaryOne, Chunk chunkTwo, ChunkBoundary boundaryTwo)
        {
            addOneWayChunkPairing(chunkOne, boundaryOne, boundaryTwo);
            addOneWayChunkPairing(chunkTwo, boundaryTwo, boundaryOne);
        }

        void addOneWayChunkPairing(Chunk targetChunk, ChunkBoundary targetBoundary, ChunkBoundary other)
        {
            int targetBoundaryIndex = targetChunk.GetIndexOfBoundary(targetBoundary);

            // get a list of all currently paired boundaries for this chunk
            if (!UpdatedChunks.TryGetValue(targetChunk, out var indexes))
            {
                indexes = new List<int>();
                UpdatedChunks[targetChunk] = indexes;

                // seed indexes with target's already occupied edges
                // If this is the new chunk we don't have to worry about this step
                if (self.GetChunkIndex(targetChunk) != -1)
                {
                    for (int n = 0; n < targetChunk.ChunkBoundaries.Count; n++)
                    {
                        var boundary = targetChunk.ChunkBoundaries[n];
                        if (self.GetChunkBoundaryAt(boundary.LevelEdge.Sibling) != null)
                        {
                            indexes.Add(n);
                        }
                    }
                }

            }

            // add this new pairing
            if (!indexes.Contains(targetBoundaryIndex))
            {
                indexes.Add(targetBoundaryIndex);
            }
        }

        public class TestValidationResult
        {
            public bool Validated;
            public bool SoftViolatesAtLeastOneRule;
        }
    }
}
