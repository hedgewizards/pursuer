using System;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Holds ammo of multiple types
    /// </summary>
    public class AmmoTank : MonoBehaviour
    {
        public bool InfiniteAmmo;
        public Dictionary<Ammunition, int> AmmoCounts = new Dictionary<Ammunition, int>();

        public void AddAmmo(Ammunition AmmoType, int addCount)
        {
            if (AmmoCounts.TryGetValue(AmmoType, out int oldCount))
            {
                AmmoCounts[AmmoType] = Mathf.Min(AmmoType.MaxSupply, oldCount + addCount);

                OnAmmoChanged?.Invoke(this, new AmmoChangedEventArgs(AmmoType, oldCount, AmmoCounts[AmmoType]));
            }
            else
            {
                AmmoCounts[AmmoType] = Mathf.Min(AmmoType.MaxSupply, addCount);
            }
        }

        public bool CanAddAmmo(Ammunition AmmoType)
        {
            if (InfiniteAmmo) return true;

            if (AmmoCounts.TryGetValue(AmmoType, out int count))
            {
                // as long as we are missing 1 bullet we can pick it up
                return count < AmmoType.MaxSupply;
            }
            else
            {
                return true;
            }
        }

        public int GetAmmoCount(Ammunition AmmoType)
        {
            if (InfiniteAmmo) return 9999;

            if (AmmoCounts.TryGetValue(AmmoType, out int count))
            {
                // as long as we are missing 1 bullet we can pick it up
                return count;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Attempt to take <paramref name="count"/> ammo of type <paramref name="AmmoType"/>
        /// </summary>
        /// <param name="AmmoType"></param>
        /// <param name="count"></param>
        /// <returns>the amount of ammo taken, up to <paramref name="count"/></returns>
        public int TakeAmmo(Ammunition AmmoType, int count)
        {
            if (InfiniteAmmo) return count;

            if (AmmoCounts.TryGetValue(AmmoType, out int oldCount))
            {
                int takeCount = Mathf.Min(count, oldCount);

                AmmoCounts[AmmoType] = oldCount - takeCount;

                OnAmmoChanged?.Invoke(this, new AmmoChangedEventArgs(AmmoType, oldCount, AmmoCounts[AmmoType]));

                return takeCount;
            }
            else
            {
                return 0;
            }
        }

        public EventHandler<AmmoChangedEventArgs> OnAmmoChanged;
        public class AmmoChangedEventArgs : EventArgs
        {
            public Ammunition AmmoType;
            public int PreviousAmount;
            public int NewAmount;

            public AmmoChangedEventArgs(Ammunition ammoType, int previousAmount, int newAmount)
            {
                AmmoType = ammoType;
                PreviousAmount = previousAmount;
                NewAmount = newAmount;
            }
        }
    }

}
