using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    public class ChunkBundle
    {
        public Chunk Chunk;
        public List<Vector3Int> LevelPositions;
        public List<(int chunkBoundaryIndex, GridEdge gridEdge)> LevelEdgePairs;
        public int boundaryIndexOffset;
        public bool SoftViolatesAtLeastOneRule;
    }
}
