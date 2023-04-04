using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AtmosphereManager))]
public class AtmosphereManagerEditor : Editor
{
    AtmosphereManager self;

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Apply Lighting Now"))
        {
            self = target as AtmosphereManager;

            self.mainCamera = Camera.main;
            self.SetSkyInstanced(self.initialSkyColor);
            self.SetFogInstanced(self.initialFogColor, self.initialFogStart, self.initialFogEnd);
        }

        base.OnInspectorGUI();
    }
}
