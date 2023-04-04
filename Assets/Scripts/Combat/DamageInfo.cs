using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public struct DamageInfo
    {
        public float FinalDamage;
        public float Damage;
        public TYPE DamageType;
        public HitBox.HitBoxType hitBoxType;
        public Vector3 OriginPoint;
        public Affliction[] Afflictions;
        public int TeamMask;

        public enum TYPE
        {
            projectile,
            melee
        }

        public DamageInfo(float Damage, TYPE DamageType, Vector3 OriginPoint, Affliction[] Afflictions = null, int TeamMask = Teams.Masks.DamageAll)
        {
            this.Damage = Damage;
            this.FinalDamage = Damage;
            this.DamageType = DamageType;
            this.OriginPoint = OriginPoint;
            this.Afflictions = Afflictions ?? new Affliction[0];
            hitBoxType = HitBox.HitBoxType.unassigned;
            this.TeamMask = TeamMask;
        }
    }
}