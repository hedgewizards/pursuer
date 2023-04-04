using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public interface IDamageable
    {
        public void ApplyDamage(DamageInfo damageInfo);
    }
}