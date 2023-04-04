using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

namespace Generation
{
    [CustomEditor(typeof(ChunkBoundary))]
    class ChunkBoundaryEditor : Editor
    {
        ChunkBoundary self;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as ChunkBoundary;
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
