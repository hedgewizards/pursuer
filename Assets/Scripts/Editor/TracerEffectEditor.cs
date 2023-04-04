using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Entity.Effects;

[CustomEditor(typeof(TracerEffect))]
public class TracerEffectEditor : Editor
{
    private TracerEffect self;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        self = target as TracerEffect;

        if (GUILayout.Button("Test Tracer"))
        {
            // make sure tracer returns to initial position when finished
            Vector3 initialPosition = self.transform.position;
            self.OnFinish += (sender, e) =>
            {
                Debug.Log($"{(sender as Effect).transform.position} => {initialPosition}");
                (sender as Effect).transform.position = initialPosition;
            };

            EffectDataStruct effectData = new EffectDataStruct();
            effectData.origin = self.transform.position;
            effectData.position = effectData.origin + self.transform.forward * 10;

            self.Perform(effectData);
        }
    }
}
