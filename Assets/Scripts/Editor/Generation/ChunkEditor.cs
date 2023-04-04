using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Generation
{
    [CustomEditor(typeof(Chunk))]
    class ChunkEditor : Editor
    {
        Chunk self;

        public override void OnInspectorGUI()
        {
            self = target as Chunk;
            DrawDefaultInspector();

            drawRulesSection();
            drawGridSection();
            drawChunkBoundariesSection();
        }

        bool rulesToggle = false;
        ChunkRule currentEditingRule;
        private void drawRulesSection()
        {
            rulesToggle = EditorGUILayout.BeginFoldoutHeaderGroup(rulesToggle, "Rules");
            if (rulesToggle)
            {
                ChunkRule ruleToDelete = null;
                foreach(ChunkRule rule in self.Rules)
                {
                    GUILayout.BeginHorizontal();
                    {
                        var listString = "Boundaries ";
                        if (rule.RelatedBoundaries == null) rule.RelatedBoundaries = new int[0];
                        for (int n = 0; n < rule.RelatedBoundaries.Length; n++)
                        {
                            int index = rule.RelatedBoundaries[n];
                            listString += n == rule.RelatedBoundaries.Length - 1 ? index.ToString()
                                : $"{index}, ";
                        }
                        if (listString == "Boundaries ") listString = "No Boundaries";
                        EditorGUILayout.LabelField(listString);

                        rule.Rule = (ChunkRule.RuleType)EditorGUILayout.EnumPopup(rule.Rule);

                        if (currentEditingRule == rule)
                        {
                            if (GUILayout.Button("Stop Editing"))
                            {
                                currentEditingRule = null;
                                EditorUtility.SetDirty(self);
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("Edit"))
                            {
                                currentEditingRule = rule;
                                EditorUtility.SetDirty(self);
                            }
                        }
                        if (GUILayout.Button("x", GUILayout.ExpandWidth(false)))
                        {
                            // mark this rule to delete after we finish drawing the GUI
                            ruleToDelete = rule;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                {
                    System.Array.Resize<ChunkRule>(ref self.Rules, self.Rules.Length + 1);
                    self.Rules[self.Rules.Length - 1] = new ChunkRule();
                }
                if (ruleToDelete != null)
                {
                    ArrayHelper.DeleteAndResize(ref self.Rules, ruleToDelete);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        bool editingSeals = false;
        bool chunkBoundariesToggle = false;
        private void drawChunkBoundariesSection()
        {
            chunkBoundariesToggle = EditorGUILayout.BeginFoldoutHeaderGroup(chunkBoundariesToggle, "Chunk Boundaries");
            if (chunkBoundariesToggle)
            {
                if (editingSeals)
                {
                    if (GUILayout.Button("Stop Editing Seals"))
                    {
                        editingSeals = false;
                        EditorUtility.SetDirty(self);
                    }
                }
                else
                {
                    if (GUILayout.Button("Edit Seals"))
                    {
                        editingSeals = true;
                        EditorUtility.SetDirty(self);
                    }
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Seal All"))
                        {
                            setAllSealed(true);
                        }
                        if (GUILayout.Button("Seal Random"))
                        {
                            setRandomSeal();
                        }
                        if (GUILayout.Button("Unseal All"))
                        {
                            setAllSealed(false);
                        }
                    }
                    GUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Regenerate"))
                {

                    Undo.SetCurrentGroupName("Regenerate Chunk Boundaries");
                    int group = Undo.GetCurrentGroup();
                    Undo.RecordObject(self.gameObject, "Regenerate Chunk Boundaries");
                    PrefabUtility.RecordPrefabInstancePropertyModifications(self.gameObject);

                    self.RegenerateChunkBoundaries();

                    foreach (ChunkBoundary c in self.ChunkBoundaries)
                    {
                        Undo.RecordObject(c, "Regenerate Chunk Boundaries");
                        PrefabUtility.RecordPrefabInstancePropertyModifications(c);
                    }

                    Undo.CollapseUndoOperations(group);
                }

                if (self.ChunkBoundaries != null)
                {
                    foreach (KeyValuePair<GridEdge, int> kv in self.GridEdgeOccupancy)
                    {
                        GridEdge edge = kv.Key;
                        ChunkBoundary boundary = self.ChunkBoundaries[kv.Value];

                        GUILayout.BeginHorizontal();
                        GUILayout.Label(kv.Value.ToString());
                        GUILayout.Label(edge.ToString());
                        EditorGUILayout.ObjectField(boundary, typeof(ChunkBoundary), true);
                        GUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        bool editingGrid = false;
        bool gridToggle = false;
        private void drawGridSection()
        {
            gridToggle = EditorGUILayout.BeginFoldoutHeaderGroup(gridToggle, "Grid");
            if (gridToggle)
            {
                string editGridText = editingGrid ? "Stop Editing Grid" : "Edit Grid";
                if (GUILayout.Button(editGridText))
                {
                    if (!editingGrid && self.LocalGridSpaces == null || self.LocalGridSpaces.Count == 0)
                    {
                        self.LocalGridSpaces = new List<Vector3Int>() { Vector3Int.zero };
                    }
                    editingGrid = !editingGrid;
                    EditorUtility.SetDirty(self);
                }

                if (GUILayout.Button("Auto Generate Grid"))
                {
                    Undo.RecordObject(self, "Auto Generate Grid");
                    PrefabUtility.RecordPrefabInstancePropertyModifications(self);
                    self.AutoGenerateGridSpaces();
                    self.RegenerateChunkBoundaries();
                    EditorUtility.SetDirty(self);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }


        #region Scene GUI
        private Transform handleTransform;
        private Quaternion handleRotation;

        private void OnSceneGUI()
        {
            self = target as Chunk;
            handleTransform = self.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                handleTransform.rotation : Quaternion.identity;

            drawRuleEditor();
            if (editingSeals) drawSealToggles();
            if (editingGrid) drawGridEditor();
        }

        void drawRuleEditor()
        {
            if (currentEditingRule == null || self.ChunkBoundaries == null || currentEditingRule.RelatedBoundaries == null) return;

            int boundaryToToggle = -1;
            bool toggleBoundaryAlreadyIncluded = false;

            for (int n = 0; n < self.ChunkBoundaries.Count; n++)
            {
                bool isIncluded = false;

                foreach(int ruleIndex in currentEditingRule.RelatedBoundaries)
                {
                    if (n == ruleIndex)
                    {
                        isIncluded = true;
                        break;
                    }
                }
                ChunkBoundary boundary = self.ChunkBoundaries[n];

                Color color = isIncluded ? Color.green : Color.red;
                System.Action onPressed = () =>
                {
                    boundaryToToggle = n;
                    toggleBoundaryAlreadyIncluded = isIncluded;
                };
               createWallButton(boundary.LocalEdge, color, onPressed);
            }
            if (boundaryToToggle != -1)
            {
                if (toggleBoundaryAlreadyIncluded)
                {
                    // remove boundary from the rule's related boundaries
                    ArrayHelper.DeleteAndResize(ref currentEditingRule.RelatedBoundaries, boundaryToToggle);
                }
                else
                {
                    // add boundary to the rule's related boundaries
                    System.Array.Resize(ref currentEditingRule.RelatedBoundaries, currentEditingRule.RelatedBoundaries.Length + 1);
                    currentEditingRule.RelatedBoundaries[currentEditingRule.RelatedBoundaries.Length - 1] = boundaryToToggle;
                }
            }
        }

        void drawSealToggles()
        {
            if (self.ChunkBoundaries == null) return;
            foreach (ChunkBoundary boundary in self.ChunkBoundaries)
            {
                Color color = boundary.IsSealed ? Color.red : Color.green;
                createWallButton(boundary.LocalEdge, color, () => boundary.IsSealed = !boundary.IsSealed);
            }
        }

        void drawGridEditor()
        {
            if (self.LocalGridSpaces == null)
            {
                return;
            }
            var gridSpacesClone = self.LocalGridSpaces.ToArray();
            foreach(Vector3Int point in gridSpacesClone)
            {
                System.Action onDeletePressed = () => self.LocalGridSpaces.Remove(point);
                System.Func<Direction, bool> shouldDrawArrow = (direction) => !self.LocalGridSpaces.Contains(point + direction.Vector);
                System.Action<Direction> addInDirection = (direction) => self.LocalGridSpaces.Add(point + direction.Vector);
                createGrowGridButton(point, onDeletePressed, shouldDrawArrow, addInDirection);
            }
        }

        Quaternion lookUpQuat => new Quaternion(0.707107f, 0, 0, 0.707107f);


        void createFloorSquareButton(Vector3Int localGridSpace, Color color, System.Action OnPressed)
        {
            Vector3 point = handleTransform.TransformPoint(self.LevelKit.GridToPosition(localGridSpace));
            float size = self.LevelKit.GridDimensions.x * 0.4f; // buttons should fill the grid they occupy but leave a little room at the edge
            Handles.color = color;
            if (Handles.Button(point, handleRotation * lookUpQuat, size, size, Handles.RectangleHandleCap))
            {
                OnPressed.Invoke();
                Repaint();
            }
        }

        void createWallButton(GridEdge localGridEdge, Color color, System.Action OnPressed)
        {
            Vector3 localWallPoint = self.LevelKit.GridToPosition(localGridEdge.Position)
                + self.LevelKit.GridToPosition(localGridEdge.Direction.Vector) / 2
                + new Vector3(0, self.LevelKit.GridDimensions.y / 2, 0);
            Vector3 point = handleTransform.TransformPoint(localWallPoint);

            float size = Mathf.Min(self.LevelKit.GridDimensions.x * 0.4f, self.LevelKit.GridDimensions.y * 0.4f);
            Handles.color = color;
            if (Handles.Button(point, handleRotation * localGridEdge.Direction.GetQuaternionFromForward(), size, size, Handles.RectangleHandleCap))
            {
                OnPressed.Invoke();
                Repaint();
            }
        }
        
        void createGrowGridButton(Vector3Int localGridSpace, System.Action OnPressed, System.Func<Direction, bool> shouldDrawArrow, System.Action<Direction> OnDirectionPressed)
        {
            Vector3 localPoint = self.LevelKit.GridToPosition(localGridSpace);
            Vector3 point = handleTransform.TransformPoint(localPoint);
            float absoluteSize = HandleUtility.GetHandleSize(point) * 0.04f;
            float squareSize = self.LevelKit.GridDimensions.x * 0.4f; // buttons should fill the grid they occupy but leave a little room at the edge

            Handles.color = Color.gray;
            Handles.Button(point, handleRotation * lookUpQuat, squareSize, 0, Handles.RectangleHandleCap);
            Handles.color = Color.red;
            if (Handles.Button(point, handleRotation * lookUpQuat, absoluteSize, absoluteSize, Handles.DotHandleCap))
            {
                OnPressed.Invoke();
                Repaint();
            }

            Handles.color = Color.green;
            foreach(Direction direction in Direction.all)
            {
                if (shouldDrawArrow(direction))
                {
                    float dimension = self.LevelKit.SizeOfDimension(direction);
                    Vector3 arrowPoint = localPoint + (Vector3)direction.Vector * dimension * 0.05f;
                    float size = dimension / 4;
                    if (Handles.Button(arrowPoint, handleRotation * direction.GetQuaternionFromForward(), size, size, Handles.ArrowHandleCap))
                    {
                        OnDirectionPressed(direction);
                    }
                }
            }
        }
        #endregion

        void setAllSealed(bool isSealed)
        {
            if (self.ChunkBoundaries != null)
            {
                foreach (ChunkBoundary boundary in self.ChunkBoundaries)
                {
                    boundary.IsSealed = isSealed;
                }
            }
        }

        void setRandomSeal()
        {
            if (self.ChunkBoundaries != null)
            {
                foreach (ChunkBoundary boundary in self.ChunkBoundaries)
                {
                    boundary.IsSealed = Random.value > 0.5f;
                }
            }
        }
    }
}
