using Entity.Dancer;
using Entity.Weapon;
using UnityEditor;
using UnityEngine;

namespace Entity.Inspector
{
    [CustomEditor(typeof(WeaponDancer))]
    class WeaponDancerEditor : Editor
    {
        WeaponDancer self;

        public override void OnInspectorGUI()
        {
            self = target as WeaponDancer;
            DrawDefaultInspector();
        }
    }
}