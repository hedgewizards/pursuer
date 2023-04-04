using System.Collections;
using UnityEngine;

namespace Entity.Effects
{
    [CreateAssetMenu(fileName = "new Tracer Template", menuName = "ScriptableObjects/Dancer/TracerEffectTemplate", order = 1)]
    public class TracerEffectTemplate : EffectTemplate
    {
        public float Lifetime;
        public float MinSpeed;
        [Range(0,1)]
        public float TracerTailSize;
    }
}