using UnityEngine;

namespace Entity.Weapon
{
    public abstract class WeaponReloader : WeaponAction
    {
        /// <summary>
        /// Refill weapon's ammo
        /// </summary>
        public virtual void Refill(bool free)
        {
            if (free)
            {
                self.Table.CurrentClip = self.Table.ClipSize;
            }
            else
            {
                self.Table.CurrentClip += self.WeaponHolder.AmmoTank.TakeAmmo(self.Table.AmmoType, self.Table.ClipSize - self.Table.CurrentClip);
            }
        }
    }
}
