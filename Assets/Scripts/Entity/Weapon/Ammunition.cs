using Entity;
using Player;
using System.Collections;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "Ammunition", menuName = "ScriptableObjects/Weapon/Ammunition", order = 1)]
    public class Ammunition : ScriptableObject, IPickupable
    {
        #region Identity

        public Color Color = Color.white;

        #endregion

        #region Stats

        /// <summary>
        /// How many bullets come in a resupply of this ammo type
        /// </summary>
        [Tooltip("How many bullets come in a resupply of this ammo type?")]
        public int ResupplyCount;

        /// <summary>
        /// How many bullets of this type can the player hold?
        /// </summary>
        [Tooltip("How many bullets can the player hold of this type at once?")]
        public int MaxSupply;


        [SerializeField]
        private AudioClip pickupSound;
        public AudioClip PickupSound => pickupSound;
        public bool AllowInstantPickup => true;

        public bool Pickup(PlayerController player, bool dryRun, bool force)
        {
            AmmoTank tank = player.GetComponent<AmmoTank>();
            if (tank.CanAddAmmo(this) || force)
            {
                if (!dryRun)
                {
                    tank.AddAmmo(this, ResupplyCount);
                }
                return true;
            }
            return false;
        }

        #endregion
    }
}