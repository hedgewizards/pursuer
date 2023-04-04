using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Generation
{
    public class ChunkBoundary : MonoBehaviour, IPopulateable
    {
        public LevelKit LevelKit;

        public GridEdge LocalEdge;

        public GameObject OpenObject;
        public GameObject ClosedObject;

        [Dropdown("LevelKit.AllowedBoundaryTypes")]
        public string BoundaryType;

        [HideInInspector]
        private bool isSealed;
        public bool IsSealed
        {
            get => isSealed;
            set
            {
                isSealed = value;
                setSeal(value);
            }
        }


        public void UpdateLocalEdge(Chunk parent)
        {
            if (parent != null)
            {
                Vector3Int gridPos = parent.LevelKit.PositionToGrid(transform.localPosition);

                Direction direction = new Direction(parent.transform.InverseTransformDirection(transform.forward));

                LocalEdge = new GridEdge(gridPos, direction);
            }
        }

        public IEnumerable<IPopulateable> Populate(LevelGenerator generator)
        {
            int siblingIndex;
            if (generator.GridEdgeOccupancy.TryGetValue(LevelEdge.Sibling, out siblingIndex))
            {
                ChunkBoundary sibling = generator.ChunkBoundaries[siblingIndex];
                IsSealed = !LevelKit.BoundariesAreCompatible(this, sibling);
            }
            else
            {
                IsSealed = true;
            }

            return new IPopulateable[0];
        }

        void setSeal(bool isSealed)
        {
            if (OpenObject != null) OpenObject.SetActive(!isSealed);
            if (ClosedObject != null) ClosedObject.SetActive(isSealed);
        }

        #region Level Space

        bool parentedToLevel = false;
        public bool ParentedToLevel { get => parentedToLevel; }

        GridEdge levelEdge;
        public GridEdge LevelEdge { get => levelEdge; }

        public int ParentChunkIndex = -1;

        public void ApplyParent(GridEdge _levelEdge, int parentChunkIndex)
        {
            ParentChunkIndex = parentChunkIndex;
            levelEdge = _levelEdge;
            parentedToLevel = true;
        }
        #endregion
    }
}
