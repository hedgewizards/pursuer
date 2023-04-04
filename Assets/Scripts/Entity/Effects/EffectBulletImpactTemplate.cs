using System.Collections;
using UnityEngine;

namespace Entity.Effects
{
    [CreateAssetMenu(fileName = "new Bullet Impact Effect Template", menuName = "ScriptableObjects/Dancer/BulletImpactEffectTemplate", order = 1)]
    public class EffectBulletImpactTemplate : EffectTemplate
    {
        public float DecalScale = 1;
        public Material DecalMaterial;
    }
}