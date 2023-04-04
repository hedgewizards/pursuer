

using Entity;
using Player;
using UnityEngine;

namespace Pursuer
{
    [CreateAssetMenu(fileName = "KeyItem", menuName = "ScriptableObjects/Entity/Key Item", order = 1)]
    public class KeyItem : ScriptableObject, IPickupable
    {
        [SerializeField]
        private AudioClip pickupSound;
        public AudioClip PickupSound => pickupSound;

        public bool AllowInstantPickup => true;

        public bool Pickup(PlayerController player, bool dryRun, bool force)
        {
            if (!dryRun)
            {
                PursuerGameManager.Instance?.PickUpKeyItem(this);
            }
            return true;
        }
    }
}
