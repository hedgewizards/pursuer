using System;
using UnityEngine;
using Combat;

namespace Entity.Weapon
{
    public class BasicBulletAttack : WeaponAttack
    {
        public int AmmoPerShot = 1;
        public int NumberOfBullets = 1;
        public float Damage = 1;
        public string MuzzleFlashEffectName;

        /// <summary>
        /// shots per second
        /// </summary>
        public float RoundsPerMinute = 400;

        float finishTime;


        public override bool Interrupt(bool dryRun, bool force)
        {
            if (!force) return false;

            if (!dryRun)
            {
                finish();
            }
            return true;
        }

        public override bool Start(bool dryRun, bool force)
        {
            // don't fire if we don't have any ammo
            if (self.Table.CurrentClip < AmmoPerShot) return false;

            if (!dryRun)
            {
                start();
            }

            base.Start(dryRun, force);
            return true;
        }

        void start()
        {
            self.Table.CurrentClip -= AmmoPerShot;
            finishTime = Time.time + 1f / RoundsPerMinute * 60f;

            DamageInfo damageInfo = new DamageInfo();
            damageInfo.Damage = Damage;
            damageInfo.DamageType = DamageInfo.TYPE.projectile;
            damageInfo.OriginPoint = self.Origin;

            FireBulletsBasic(NumberOfBullets, damageInfo, CalculateSpread());
            ApplyAccuracyAttackPenalty();

            if (OverrideActionName != null && OverrideActionName != "")
            {
                self.OnAction?.Invoke(this, new WeaponActionEventArgsTrigger(OverrideActionName));
            }
            else
            {
                self.OnAction?.Invoke(this, new WeaponActionEventArgsTrigger(WeaponActionEventArgs.Names.Attack1));
            }

            if (MuzzleFlashEffectName != null && MuzzleFlashEffectName != "")
            {
                EffectDataStruct data = new EffectDataStruct()
                {
                    normal = Vector3.up,
                };
                WeaponActionEffectEventArgs args = new WeaponActionEffectEventArgs
                (
                    MuzzleFlashEffectName,
                    data,
                    WeaponActionEffectEventArgs.EffectOption.PositionToWeaponOrigin
                    + WeaponActionEffectEventArgs.EffectOption.ParentToWeaponOrigin
                );

                self.OnAction?.Invoke(this, args);

            }

            self.OnThink += CheckAttackProgress;
        }

        void finish()
        {
            self.OnThink -= CheckAttackProgress;
        }

        public void CheckAttackProgress(object sender, EventArgs e)
        {
            if (Time.time > finishTime)
            {
                FinishAct();
                finish();
            }
        }
    }
}
