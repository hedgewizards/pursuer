using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Entity.Effects;

[CustomEditor(typeof(EffectBasic))]
public class EffectBasicEditor : Editor
{
    EffectBasic self;

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Test"))
        {
            self = target as EffectBasic;
            EffectDataStruct e = new EffectDataStruct();
            e.position = self.transform.position;
            e.normal = Vector3.up;
            e.origin = Vector3.zero;
            e.parent = self.transform.parent;
            self.Perform(e);
        }

        DrawDefaultInspector();
    }
}
