using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Effects.Replacement;
using UnityEngine;

namespace Entity.Effects
{
    public class EffectReplacer : MonoBehaviour
    {
        public EffectReplacementTable EffectReplacementTable;

        public string GetReplacement(string effectName)
        {
            var table = EffectReplacementTable;
            if (table != null)
            {
                foreach (EffectReplacement replacement in table.Replacements)
                {
                    if (replacement.Original.name == effectName)
                    {
                        return replacement.Replacement.name;
                    }
                }
            }

            return effectName;
        }
    }
}
