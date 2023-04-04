using System;
using UnityEngine;

namespace Entity.Weapon
{
    public class BasicWeaponReloader : WeaponReloader
    {
        /// <summary>
        /// How long it takes to reload in seconds
        /// </summary>
        public float Duration = 1;
        /// <summary>
        /// How long it takes to refill ammo in seconds
        /// </summary>
        public float RefillDuration = 1;

        float finishTime;
        float refillTime;

        bool hasRefilled;

        public override bool Interrupt(bool dryRun, bool force)
        {
            if (!force) return false;

            if (!dryRun)
            {
                self.OnThink -= CheckReloadProgress;
            }

            return true;
        }

        public override bool Start(bool dryRun, bool force)
        {
            //  if we shouldn't force, don't reload if we aren't missing any ammo or there's no ammo to refill
            if (!force && (self.Table.CurrentClip >= self.Table.ClipSize || self.WeaponHolder.AmmoTank.GetAmmoCount(self.Table.AmmoType) == 0)) return false;

            if (!dryRun)
            {
                startReload();
            }

            base.Start(dryRun, force);
            return true;
        }

        void startReload()
        {
            finishTime = Time.time + Duration;
            refillTime = Time.time + RefillDuration;
            hasRefilled = false;

            // set up event listener to check reload progress
            self.OnThink += CheckReloadProgress;
            self.OnAction?.Invoke(this, new WeaponActionEventArgsTrigger(OverrideActionName ?? WeaponActionEventArgs.Names.Reload));
        }

        public void CheckReloadProgress(object sender, EventArgs e)
        {
            if (!hasRefilled && Time.time > refillTime)
            {
                hasRefilled = true;
                Refill(false);
            }

            if (Time.time > finishTime)
            {
                FinishAct();
                self.OnThink -= CheckReloadProgress;
            }
        }
    }
}
