using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Entity.Weapon;

namespace Entity.Inspector
{
    [CustomEditor(typeof(WeaponActor))]
    class WeaponActorEditor : Editor
    {
        WeaponActor self;

        public override void OnInspectorGUI()
        {
            self = target as WeaponActor;
            DrawDefaultInspector();

            if (GUILayout.Button("Update From Json"))
            {
                Undo.RecordObject(self, "Update From Json");
                PrefabUtility.RecordPrefabInstancePropertyModifications(self);

                self.UpdateTableFromJson();
            }
        }
    }
}