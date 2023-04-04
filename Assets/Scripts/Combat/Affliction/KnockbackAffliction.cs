using Combat;
using Entity.Being;
using JsonKnownTypes;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat.Afflictions
{
    [JsonKnownThisType(nameof(KnockbackAffliction))]
    public class KnockbackAffliction : Affliction
    {
        public bool ScaleForceWithDamage = false;

        /// <summary>
        /// 1 unit of Force is equal to 1 m/s movement speed applied to the player. value differs per monster"
        /// </summary>
        public float UpwardForceMagnitude = 0;
        /// <summary>
        /// "Force applied AWAY from origin point. a negative value is a force TOWARDS"
        /// </summary>
        public float DirectionalForceMagnitude = 1;

        /// <summary>
        /// Remove the vertical component of directional force
        /// </summary>
        public bool FlattenDirectionalForce = false;

        public override void ApplyToBeing(DamageInfo damageInfo, BeingActor being)
        {
            throw new System.NotImplementedException();
        }

        public override void ApplyToPlayer(DamageInfo damageInfo, PlayerController player)
        {
            Vector3 upwardForce = UpwardForceMagnitude * Vector3.up;
            Vector3 directionalForce = DirectionalForceMagnitude * (player.transform.position - damageInfo.OriginPoint).normalized;
            if (FlattenDirectionalForce)
            {
                directionalForce = directionalForce.Flatten();
            }


            player.ApplyForce(upwardForce + directionalForce);
        }
    }
}
