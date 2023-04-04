using Entity.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public static class CombatManager
    {
        /// <summary>
        /// Fires a hitscan bullet that does some damage to the first target hit.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="damageInfo"></param>
        public static void FireBullet(Vector3 origin, Vector3 effectOrigin, Vector3 direction, DamageInfo damageInfo)
        {
            const int maxDistance = 200;
            RaycastHit hitInfo;
            bool didHit = Physics.Raycast(origin, direction, out hitInfo, maxDistance, LAYERMASK.BULLET_MASK);

            // perform tracer effect
            EffectDataStruct tracerEffectData = new EffectDataStruct();
            tracerEffectData.origin = origin;
            tracerEffectData.position = didHit ? hitInfo.point : origin + direction * maxDistance;
            EffectManager.CreateEffect("tracer", tracerEffectData);

            if (didHit)
            {
                if (hitInfo.collider.gameObject.layer == LAYER.hitbox)
                {
                    HitBox hitBox = hitInfo.collider.GetComponent<HitBox>();
                    hitBox.ApplyDamage(damageInfo);
                }
            }
        }
    }
}