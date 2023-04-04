using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Generation
{
    [CustomEditor(typeof(LevelGenerator))]
    class LevelGeneratorEditor : Editor
    {
        LevelGenerator self;

        bool testingToggle = false;

        public override void OnInspectorGUI()
        {
            self = target as LevelGenerator;
            DrawDefaultInspector();


            if (GUILayout.Button("Build Now"))
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(self);
                Undo.RecordObject(self, "Build Now");
                self.Build();
            }

            testingToggle = EditorGUILayout.BeginFoldoutHeaderGroup(testingToggle, "Testing");
            if (testingToggle)
            {
                if (GUILayout.Button("Build From Children"))
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(self);
                    Undo.RecordObject(self, "Build From Children");
                    self.BuildFromChildren();
                }

                if (GUILayout.Button("Clear"))
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(self);
                    Undo.RecordObject(self, "Clear");
                    self.gridOccupancy = new Dictionary<Vector3Int, int>();
                    self.GridEdgeOccupancy = new Dictionary<GridEdge, int>();
                }

                if (GUILayout.Button("Perform Test"))
                {
                    Undo.SetCurrentGroupName("Perform Test");
                    int group = Undo.GetCurrentGroup();

                    Undo.RecordObject(self, "Perform Test");
                    Undo.RecordObject(self.gameObject, "Perform Test");

                    self.BuildFromChildren();

                    LevelGenerationContext context = new LevelGenerationContext(self);

                    try
                    {
                        self.GenerateLoop(context, self.ChunkBundles[0].Chunk, 5, 4);
                        if (!self.GenerateLoop(context, self.ChunkBundles[0].Chunk, 4, 3))
                        {
                            self.GenerateLoop(context, null, 4, 3);
                        }

                    }
                    finally
                    {
                        context.Finished();
                    }

                    self.SealExits();

                    int startingChunkIndex = self.ChunkBundles.Count;
                    for (int n = startingChunkIndex; n < self.ChunkBundles.Count; n++)
                    {
                        Undo.RegisterCreatedObjectUndo(self.ChunkBundles[n].Chunk.gameObject, $"Create chunk {n}");
                    }
                    Undo.CollapseUndoOperations(group);
                }

                if (GUILayout.Button("Seal"))
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(self);
                    Undo.RecordObject(self, "Seal");
                    self.SealExits();
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

        }
    }
}
