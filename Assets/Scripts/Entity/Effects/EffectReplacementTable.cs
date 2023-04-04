using System.Collections;
using UnityEngine;

namespace Entity.Effects.Replacement
{
    [CreateAssetMenu(fileName = "new table", menuName = "ScriptableObjects/Effects/Effect Replacement Table", order = 1)]
    public class EffectReplacementTable : ScriptableObject
    {
        public EffectReplacement[] Replacements;
    }

    [System.Serializable]
    public class EffectReplacement
    {
        public EffectTemplate Original;
        public EffectTemplate Replacement;
    }
}
