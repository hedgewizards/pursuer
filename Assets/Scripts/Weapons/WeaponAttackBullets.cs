using Combat;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new attack", menuName = "ScriptableObjects/Weapon/Attacks/WeaponAttackBullets", order = 1)]
public class WeaponAttackBullets : WeaponAttack
{
    public float Damage = 24;
    [Tooltip("Max inaccuracy as an angle in degrees")]
    public float Spread = 2;
    public float BulletCount = 1;
    public int RequiredAmmo = 1;

    public AudioClip FireSound;

    public override void Perform(PlayerController performer)
    {
        Vector3 origin = performer.GetShootPos();
        Vector3 baseDirection = performer.GetAimDir();
        Vector3 effectOrigin = Vector3.zero; // performer.GetEffectPos();
        //performer.ActiveWeapon.CurrentClip -= RequiredAmmo;

        DamageInfo damageInfo = new DamageInfo(Damage, DamageInfo.TYPE.projectile, origin);
        for(int n = 0; n < BulletCount; n++)
        {
            FireBullet(origin, effectOrigin, applySpread(baseDirection, Spread), damageInfo);
        }

        performer.AUD.PlayOnce(FireSound);
    }

    public override bool CanPerform(PlayerController performer, out AttackFailReason failReason)
    {
        /*
        if(performer.ActiveWeapon.CurrentClip < RequiredAmmo)
        {
            failReason = AttackFailReason.noAmmo;
            return false;
        }
        */
        failReason = AttackFailReason.unknown;
        return true;
    }

    Vector3 applySpread(Vector3 baseDirection, float maxSpread)
    {
        float roll = Random.value * 360;
        float deviation = Random.value * maxSpread;

        // rotate to the right by some degree
        Vector3 offsetDirection = Quaternion.AngleAxis(deviation, Vector3.up) * baseDirection;

        // then rotate around original axis by some degree
        return Quaternion.AngleAxis(roll, baseDirection) * offsetDirection;
    }
}
