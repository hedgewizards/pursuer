using Combat;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAttack : ScriptableObject
{
    public bool IsAutomatic;
    public float Duration;
    public string animationTriggerName = "attack1";

    public enum AttackFailReason
    {
        unknown,
        noAmmo,
    }

    public abstract void Perform(PlayerController performer);

    /// <summary>
    /// Can the player perform this attack?
    /// </summary>
    /// <param name="performer"></param>
    /// <returns></returns>
    public bool CanPerform(PlayerController performer)
    {
        AttackFailReason _failReason;
        return CanPerform(performer, out _failReason);
    }
    /// <summary>
    /// Can the player perform this attack?
    /// </summary>
    /// <param name="performer"></param>
    /// <param name=failReason">The reason for failure</param>
    /// <returns></returns>
    public virtual bool CanPerform(PlayerController performer, out AttackFailReason failReason)
    {
        failReason = AttackFailReason.unknown;
        return true;
    }

    /// <summary>
    /// Fires a hitscan bullet that does some damage to the first target hit.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="damageInfo"></param>
    public void FireBullet(Vector3 origin, Vector3 effectOrigin, Vector3 direction, DamageInfo damageInfo)
    {
        const int maxDistance = 200;
        RaycastHit hitInfo;
        bool didHit = Physics.Raycast(origin, direction, out hitInfo, maxDistance, LAYERMASK.BULLET_MASK);

        //EffectManager.Instance.DoBulletEffect(effectOrigin, didHit ? hitInfo.point : origin + direction * maxDistance, hitInfo.collider, hitInfo.normal);

        if(didHit)
        {
            if(hitInfo.collider.gameObject.layer == LAYER.hitbox)
            {
                HitBox hitBox = hitInfo.collider.GetComponent<HitBox>();
                if (hitBox != null) hitBox.ApplyDamage(damageInfo);
            }
        }
    }

    public virtual void Think(PlayerController performer)
    {

    }
}