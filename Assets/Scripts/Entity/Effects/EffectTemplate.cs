using System.Collections;
using UnityEngine;

namespace Entity.Effects
{
    public abstract class EffectTemplate : ScriptableObject
    {
        public GameObject Prefab;
        public bool RegisterEffect;
        [Tooltip("for things that should linger in the world as long as possible after the effect finishes (like bullet holes)")]
        public bool IsLastingEffect;
    }
}
