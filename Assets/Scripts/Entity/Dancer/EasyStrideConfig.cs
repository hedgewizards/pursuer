using System.Collections;
using UnityEngine;

namespace Entity.Dancer
{
    //[CreateAssetMenu(fileName = "EasyStrideConfig", menuName = "ScriptableObjects/Dancer/EasyStrideConfig", order = 1)]
    [System.Obsolete]
    public class EasyStrideConfig : ScriptableObject
    {
        public float LegHeight;
        public float StrideRadius;
    }
}
