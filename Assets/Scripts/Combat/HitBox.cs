using Entity.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Effects.Replacement;

namespace Combat
{
    [RequireComponent(typeof(Collider))]
    public class HitBox : MonoBehaviour
    {
        public IDamageable Target;
        public enum HitBoxType
        {
            unassigned,
            normal,
            critical,
            leg
        }
        public HitBoxType Type = HitBoxType.normal;

        public void ApplyDamage(DamageInfo damageInfo)
        {
            damageInfo.hitBoxType = Type;
            Target.ApplyDamage(damageInfo);
        }

        public Vector3 GetClosestPoint(Vector3 p)
        {
            return GetComponent<BoxCollider>().ClosestPointOnBounds(p);
        }
    }
}