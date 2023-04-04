using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Generation
{
    public class Chunk : MonoBehaviour, ISerializationCallbackReceiver
    {
        public LevelKit LevelKit;

        [HideInInspector]
        public ChunkRule[] Rules;

        [HideInInspector]
        public List<Vector3Int> LocalGridSpaces;

        public Dictionary<GridEdge, int> GridEdgeOccupancy;

        [HideInInspector]
        public List<ChunkBoundary> ChunkBoundaries;

        public void AutoGenerateGridSpaces()
        {
            // get the big bounding box
            Collider collider = GetComponent<Collider>();
            Bounds bounds = collider != null ? collider.bounds : new Bounds();
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                bounds.Encapsulate(col.bounds);
            }

            // convert max and min point to local space
            Vector3Int start = LevelKit.PositionToGrid(transform.InverseTransformPoint(bounds.min));
            Vector3Int end = LevelKit.PositionToGrid(transform.InverseTransformPoint(bounds.max));

            // rearrange mins and maxes so they are in the right order after being rotated
            if ( end.x < start.x )
            {
                int temp = start.x;
                start.x = end.x;
                end.x = temp;
            }
            if (end.z < start.z)
            {
                int temp = start.z;
                start.z = end.z;
                end.z = temp;
            }

            List<Vector3Int> tempGridSpaces = new List<Vector3Int>();

            for (int x = Mathf.RoundToInt(start.x); x <= end.x; x++)
            {
                for (int z = Mathf.RoundToInt(start.z); z <= end.z; z++)
                {
                    Vector3 startPos = transform.TransformPoint(Vector3.Scale(new Vector3Int(x, end.y, z), LevelKit.GridDimensions)
                        + (Vector3.up * LevelKit.GridDimensions.y / 2));
                    float distance = LevelKit.GridDimensions.y * (end.y - start.y + 1);

                    if (Physics.Raycast(new Ray(startPos, Vector3.down), distance, LAYERMASK.ALL))
                    {
                        tempGridSpaces.Add(new Vector3Int(x, 0, z));
                    }
                }
            }
            LocalGridSpaces = tempGridSpaces;
        }
        public void RegenerateChunkBoundaries()
        {
            var newChunkBoundaries = GetComponentsInChildren<ChunkBoundary>();
            ChunkBoundaries = new List<ChunkBoundary>();
            GridEdgeOccupancy = new Dictionary<GridEdge, int>();

            foreach(ChunkBoundary newChunkBoundary in newChunkBoundaries)
            {
                newChunkBoundary.UpdateLocalEdge(this);

                if (GridEdgeOccupancy.ContainsKey(newChunkBoundary.LocalEdge))
                {
                    Debug.LogError($"Duplicate GridEdges detected in chunk {name}");
                    return;
                }

                int newIndex = ChunkBoundaries.Count;
                ChunkBoundaries.Add(newChunkBoundary);
                GridEdgeOccupancy[newChunkBoundary.LocalEdge] = newIndex;
            }
        }
        public void AlignToEdge(GridEdge targetEdge, ChunkBoundary aligningBoundary)
        {
            (Vector3Int gridPosition, RotationType rotation) = CalculateAlignment(targetEdge, aligningBoundary);
            transform.position = LevelKit.GridToPosition(gridPosition);
            transform.rotation = Quaternion.AngleAxis(rotation.ToAngle(), Vector3.up);
        }

        public (Vector3Int gridPosition, RotationType rotation) CalculateAlignment(GridEdge targetEdge, ChunkBoundary aligningBoundary)
        {
            GridEdge targetSibling = targetEdge.Sibling;
            RotationType rotation = targetSibling.Direction.GetRotationFrom(aligningBoundary.LocalEdge.Direction);
            Vector3Int gridPosition = targetSibling.Position - (aligningBoundary.LocalEdge.Position).Rotate(rotation);

            return (gridPosition, rotation);
        }

        public int GetIndexOfBoundary(ChunkBoundary boundary)
        {
            for (int n = 0; n < ChunkBoundaries.Count; n++)
            {
                if (boundary == ChunkBoundaries[n]) return n;
            }
            throw new KeyNotFoundException();
        }

        #region Debug
        /*
        Color spaceGizmoColor = new Color(1, 1, 0, 0.5f);
        Color boundaryGizmoColor = new Color(1, 0, 0, 0.5f);
        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            // Draw Boxes
            Gizmos.color = spaceGizmoColor;
            foreach (Vector3 gridSpace in LocalGridSpaces)
            {
                gridSpace.Scale(LevelKit.GridDimensions);
                Vector3 cubePos = gridSpace
                    + (Vector3.up * LevelKit.GridDimensions.y / 2);
                Gizmos.DrawCube(cubePos, LevelKit.GridDimensions);
            }

            // Draw Boundaries
            if (ChunkBoundaries != null)
            {
                Gizmos.color = boundaryGizmoColor;
                foreach (KeyValuePair<GridEdge, int> kv in GridEdgeOccupancy)
                {
                    GridEdge edge = kv.Key;
                    Vector3 pos = edge.Position
                        + Vector3.up * 0.5f
                        + (Vector3)edge.Direction.Vector * 0.5f;
                    pos = Vector3.Scale(pos, LevelKit.GridDimensions);
                    bool directionIsForwardOrBackward = edge.Direction.Vector.z != 0;
                    Gizmos.DrawCube(pos, new Vector3
                    {
                        x = LevelKit.GridDimensions.x * (directionIsForwardOrBackward ? 1 : 0.05f),
                        y = LevelKit.GridDimensions.y,
                        z = LevelKit.GridDimensions.z * (directionIsForwardOrBackward ? 0.05f : 1)
                    });

                }
            }
        }
        */
        #endregion

        #region Serialization
        [SerializeField]
        [HideInInspector]
        private GridEdge[] _serializedGridEdges;
        [SerializeField]
        [HideInInspector]
        private ChunkBoundary[] _serializedChunkBoundaries;

        public void OnBeforeSerialize()
        {
            int count = ChunkBoundaries.Count;
            _serializedGridEdges = new GridEdge[count];
            _serializedChunkBoundaries = new ChunkBoundary[count];

            foreach(var kv in GridEdgeOccupancy)
            {
                int index = kv.Value;
                _serializedGridEdges[index] = kv.Key;
                _serializedChunkBoundaries[index] = ChunkBoundaries[index];
            }
        }

        public void OnAfterDeserialize()
        {
            ChunkBoundary[] chunkBoundaryArray = new ChunkBoundary[_serializedGridEdges.Length];
            GridEdgeOccupancy = new Dictionary<GridEdge, int>();

            for (int n = 0; n < _serializedGridEdges.Length; n++)
            {
                chunkBoundaryArray[n] = _serializedChunkBoundaries[n];
                GridEdgeOccupancy[_serializedGridEdges[n]] = n;
            }


            ChunkBoundaries = chunkBoundaryArray.ToList();
            _serializedGridEdges = null;
            _serializedChunkBoundaries = null;
        }

        #endregion
    }
}
