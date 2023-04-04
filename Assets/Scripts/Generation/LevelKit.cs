using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    [CreateAssetMenu(fileName = "new LevelKit", menuName = "ScriptableObjects/Generation/LevelKit", order = 1)]
    public class LevelKit : ScriptableObject
    {
        public Vector3 GridDimensions = new Vector3(8, 6, 8);

        public string[] AllowedBoundaryTypes;

        public Vector3Int PositionToGrid(Vector3 LocalPosition)
        {
            var fuzzyGridPos = Vector3.Scale(LocalPosition, GridDimensions.Reciprocal());

            return new Vector3Int
            {
                x = Mathf.RoundToInt(fuzzyGridPos.x),
                y = Mathf.RoundToInt(fuzzyGridPos.y),
                z = Mathf.RoundToInt(fuzzyGridPos.z),
            };
        }
        public Vector3 GridToPosition(Vector3Int GridPosition)
        {
            return Vector3.Scale(GridDimensions, GridPosition);
        }

        public float SizeOfDimension(Direction direction)
        {
            return direction.Vector.x != 0 ? GridDimensions.x
                : direction.Vector.y != 0 ? GridDimensions.y
                : direction.Vector.z != 0 ? GridDimensions.z : 0;
        }

        public IEnumerable<GameObject> ChunkPrefabs => Resources.LoadAll<GameObject>($"Chunks/{name}");

        #region Chunk Boundaries
        public bool BoundariesAreCompatible(ChunkBoundary a, ChunkBoundary b)
        {
            return (a.BoundaryType == b.BoundaryType);
        }
        #endregion

        #region Edge Heuristic
        public int CalculateEdgeHeuristic(GridEdge start, GridEdge end)
        {
            GridEdge localEdge = end.ToLocal(start);

            int turningCost = calculateLocalTurningCost(localEdge);
            if (turningCost == int.MaxValue) return turningCost;

            int distanceCost = Mathf.Abs(localEdge.Position.x)
                + Mathf.Abs(localEdge.Position.y * 2)
                + Mathf.Abs(localEdge.Position.z);

            return turningCost + distanceCost;
        }

        /// <summary>
        /// Starting at an edge at (0,0,0) facing in direction (1,0,0), find the amount of horizontal turns needed to line up the edges
        /// </summary>
        /// <param name="position"></param>
        /// <param name="uDirection"></param>
        /// <returns></returns>
        int calculateLocalTurningCost(GridEdge targetEdge)
        {
            //TODO: Matrix Math optimization
            if (targetEdge.Direction == Direction.forward)
            {
                // If we're on top of eachother 
                // If we're behind our starting point we need to do a full 360
                if (targetEdge.Position.z < 0) return 4;
                // If we're directly in front of the starting point we don't need to turn
                else if (targetEdge.Position.x == 0) return 0;
                // If we're in line with the starting point but not in front we need to 360
                else if (targetEdge.Position.z <= 1) return 4;
                // otherwise, we can get there in 2 turns
                else return 2;
            }
            else if (targetEdge.Direction == Direction.back)
            {
                // these 2 positions are impossible to reach from this angle
                if (targetEdge.Position == new Vector3Int(0, 0, -1) || targetEdge.Position == new Vector3Int(0, 0, 1))
                    return int.MaxValue;
                // if we're in-line laterally we need to turn around AND jog back over
                else if (targetEdge.Position.x == 0) return 4;
                // otherwise, we can get there in 2 turns
                else return 2;
            }
            else
            {
                // these positions are impossible to reach from this angle
                if (targetEdge.Position == new Vector3Int(0, 0, 1) || targetEdge.Position == new Vector3Int(targetEdge.Direction.x, 0, 0))
                    return int.MaxValue;
                // inner-elbow moves only take 1 turn
                else if (targetEdge.Position.z >= 1 && targetEdge.Position.x * targetEdge.Direction.x >= 1) return 1;
                // outer-elbow moves take 3 turns
                else return 3;
            }
        }
        #endregion
    }
}

