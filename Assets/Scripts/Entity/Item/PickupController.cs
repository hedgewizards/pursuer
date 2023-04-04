using Player;
using UnityEngine;

namespace Entity
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(AudioSource))]
    public abstract class PickupController : Loot
    {
        protected IPickupable item;

        public void OnTriggerStay(Collider other)
        {
            if (Looted) return;
            PlayerController player = other.GetComponent<PlayerController>();
            if (item.Pickup(player, false, false))
            {
                onPickUp();
            }
        }

        void onPickUp()
        {
            Looted = true;
            GetComponent<AudioSource>().PlayOneShot(item.PickupSound);
        }

        public override void Respawn()
        {
            base.Respawn();
        }
    }
}
