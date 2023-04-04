using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class HealthTank : MonoBehaviour, IDamageable
    {
        public float Health = 100;
        public float MaxHealth = 100;
        public HitBox[] HitBoxes;

        public EventHandler<DamageInfo> OnTakeDamage;
        public EventHandler OnZeroHealth;

        public void Awake()
        {
            foreach(HitBox hitbox in HitBoxes)
            {
                hitbox.Target = this;
            }
        }

        public void ApplyDamage(DamageInfo damageInfo)
        {
            float initialHealth = Health;
            damageInfo.FinalDamage = damageInfo.hitBoxType switch
            {
                HitBox.HitBoxType.critical => damageInfo.Damage * 2,
                _ => damageInfo.Damage,
            };

            Health -= damageInfo.FinalDamage;
            OnTakeDamage?.Invoke(this, damageInfo);
            if (initialHealth > 0 && Health <= 0)
            {
                OnZeroHealth?.Invoke(this, EventArgs.Empty);
            }
        }
    }

}