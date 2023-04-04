using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MultiSegmentEnemyBuilder))]
public class MultiSegmentEnemyBuilderInspector : Editor
{
    private MultiSegmentEnemyBuilder self;

    public override void OnInspectorGUI()
    {
        self = target as MultiSegmentEnemyBuilder;

        if (GUILayout.Button("Build Bug"))
        {
            self.Build();
            Undo.RecordObject(self, "Build Bug");
            EditorUtility.SetDirty(self);
        }

        base.OnInspectorGUI();
    }
}
