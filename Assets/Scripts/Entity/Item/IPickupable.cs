using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Entity
{
    /// <summary>
    /// Can be made into a pickup for use with <see cref="PickupController"/>
    /// </summary>
    public interface IPickupable
    {
        public AudioClip PickupSound { get; } 
        public bool AllowInstantPickup { get; }

        public abstract bool Pickup(PlayerController player, bool dryRun, bool force);
    }
}
