using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Weapon.Modifiers
{
    public class SelfLaunchModifier : WeaponModifier
    {
        public float PrimaryFireKnockback = 0;
        public float AltFireKnockback = 0;

        public override void OnDeploy()
        {
            if (PrimaryFireKnockback > 0 && self.Table.PrimaryFire != null)
            {
                self.Table.PrimaryFire.OnActStart += ApplyPrimaryFireKnockback;
            }

            if (AltFireKnockback > 0 && self.Table.AltFire != null)
            {
                self.Table.AltFire.OnActStart += ApplyAltFireKnockback;

            }
        }

        private void ApplyPrimaryFireKnockback(object sender, EventArgs e)
        {
            self.WeaponHolder.ApplyForce(self.AimDirection * -1 * PrimaryFireKnockback);
        }

        private void ApplyAltFireKnockback(object sender, EventArgs e)
        {
            self.WeaponHolder.ApplyForce(self.AimDirection * -1 * AltFireKnockback);
        }
    }
}
