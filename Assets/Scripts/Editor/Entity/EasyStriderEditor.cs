using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Entity.Dancer;
using UnityEngine.Animations.Rigging;

namespace Entity.Inspector
{
    [CustomEditor(typeof(IKStrider))]
    class EasyStriderEditor : Editor
    {
        IKStrider self;

        public override void OnInspectorGUI()
        {
            self = target as IKStrider;
            DrawDefaultInspector();

            if (self.Legs != null && self.Legs.Length == 0 && GUILayout.Button("Set Up"))
            {
                Undo.RecordObject(self, "Set Up");
                PrefabUtility.RecordPrefabInstancePropertyModifications(self);

                List<EasyStriderLeg> newLegs = new List<EasyStriderLeg>();
                float offset = 0;
                foreach(TwoBoneIKConstraint constraint in self.transform.GetComponentsInChildren<TwoBoneIKConstraint>())
                {
                    EasyStriderLeg newLeg = new EasyStriderLeg(offset);
                    newLeg.Target = constraint.transform.GetChild(0).gameObject;
                    newLeg.Hint = constraint.transform.GetChild(1).gameObject;
                    newLeg.Parent = constraint.gameObject;
                    newLegs.Add(newLeg);

                    offset += 0.5f;
                }
                self.Legs = newLegs.ToArray();
            }
        }
    }
}