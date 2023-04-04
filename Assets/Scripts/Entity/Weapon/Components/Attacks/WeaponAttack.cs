using UnityEngine;
using Entity.Effects;
using System;
using Combat;

namespace Entity.Weapon
{
    public abstract class WeaponAttack : WeaponAction
    {
        public bool IsAutomatic = false;

        public AccuracySettings AccuracySettings;
        public string TracerEffectName = "PistolTracer";
        public string ImpactEffectName = "BasicBulletImpact";

        float baseAttackPenalty = 0;
        float accuracyAttackPenalty => Mathf.Pow(baseAttackPenalty, AccuracySettings.AttackPenaltyDecayPower) * AccuracySettings.AttackPenaltyScale * AccuracySettings.BaseAccuracy;

        /// <summary>
        /// The current spread of the weapon, accounting for external factors
        /// </summary>
        public virtual float CalculateSpread()
        {
            return AccuracySettings.BaseAccuracy
                + AccuracySettings.BaseAccuracy * AccuracySettings.MovementPenaltyScale * self.SpeedFraction
                + AccuracySettings.BaseAccuracy * AccuracySettings.JostlePenaltyScale * self.CurrentJostlePenalty
                + accuracyAttackPenalty;
        }

        protected void FireBulletsBasic(int bulletCount, DamageInfo damageInfo, float spread)
        {
            for (int n = 0; n < bulletCount; n++)
            {
                FireBullet(self.Origin, ApplySpread(self.AimDirection, spread), damageInfo, true);
            }
        }

        protected void FireBullet(Vector3 origin, Vector3 direction, DamageInfo damageInfo, bool createEffects)
        {
            const int maxDistance = 200;
            RaycastHit hitInfo;
            bool didHit = Physics.Raycast(origin, direction, out hitInfo, maxDistance, LAYERMASK.BULLET_MASK);

            //EffectManager.Instance.DoBulletEffect(effectOrigin, didHit ? hitInfo.point : origin + direction * maxDistance, hitInfo.collider, hitInfo.normal);

            if (didHit)
            {
                if (hitInfo.collider.gameObject.layer == LAYER.hitbox)
                {
                    HitBox hitBox = hitInfo.collider.GetComponent<HitBox>();
                    hitBox.ApplyDamage(damageInfo);
                }
            }

            if (createEffects)
            {
                if (TracerEffectName != "_NoEffect")
                {
                    EffectDataStruct data = new EffectDataStruct()
                    {
                        position = didHit ? hitInfo.point : origin + direction * maxDistance,
                    };
                    WeaponActionEffectEventArgs args = new WeaponActionEffectEventArgs
                    (
                        name: TracerEffectName,
                        data: data,
                        options: WeaponActionEffectEventArgs.EffectOption.OriginToWeaponOrigin
                    );
                    self.OnAction?.Invoke(this, args);
                }
                if (didHit && ImpactEffectName != "_NoEffect")
                {
                    EffectDataStruct data = new EffectDataStruct()
                    {
                        position = hitInfo.point,
                        normal = hitInfo.normal,
                        parent = hitInfo.transform
                    };
                    WeaponActionEffectEventArgs args = new WeaponActionEffectEventArgs
                    (
                        name: ImpactEffectName,
                        data: data,
                        options: WeaponActionEffectEventArgs.EffectOption.ParentCanReplace
                    );
                    self.OnAction?.Invoke(this, args);
                }
            }
        }

        public Vector3 ApplySpread(Vector3 baseDirection, float maxSpread)
        {
            float roll = UnityEngine.Random.value * 360;
            float deviation = UnityEngine.Random.value * maxSpread;

            // rotate to the right by some degree
            return Quaternion.AngleAxis(roll, baseDirection)
                // then rotate around original axis by some degree
                * Quaternion.AngleAxis(deviation, Vector3.up)
                // starting from the direction we're aiming
                * baseDirection;
        }

        protected void ApplyAccuracyAttackPenalty()
        {
            baseAttackPenalty = Mathf.Min(baseAttackPenalty + AccuracySettings.AttackPenaltyPerShot, 1);
        }

        void DecayAccuracyAttackPenalty(object sender, System.EventArgs e)
        {
            baseAttackPenalty = Mathf.Max(0, baseAttackPenalty - Time.deltaTime / AccuracySettings.AttackPenaltyDuration);
        }

        public override void OnDeploy()
        {
            self.OnThink += DecayAccuracyAttackPenalty;
            base.OnDeploy();
        }

        public override void OnHolster()
        {
            self.OnThink -= DecayAccuracyAttackPenalty;
            base.OnHolster();
        }
    }
}
