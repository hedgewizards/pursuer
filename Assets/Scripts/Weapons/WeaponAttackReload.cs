using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new reload", menuName = "ScriptableObjects/Weapon/Attacks/WeaponAttackReload", order = 1)]

public class WeaponAttackReload : WeaponAttack
{
    [Tooltip("Time in seconds after reload animation starts that the ammo is replenished")]
    public float ReloadTime = 1;

    public AudioClip ReloadSound = null;

    float startTime = 0;
    bool ammoReloaded = false;

    public override bool CanPerform(PlayerController performer, out AttackFailReason failReason)
    {
        bool canPerform = true;
        failReason = AttackFailReason.unknown;

        /*
        if(performer.ActiveWeapon.CurrentClip >= performer.ActiveWeapon.ClipSize)
        {
            canPerform = false;
        }
        */

        return canPerform;
    }

    public override void Perform(PlayerController performer)
    {
        performer.AUD.PlayOnce(ReloadSound);
        startTime = Time.time;
        ammoReloaded = false;
    }

    public override void Think(PlayerController performer)
    {
        if(!ammoReloaded && Time.time > startTime + ReloadTime)
        {
            ammoReloaded = true;
            DoReload(performer);
        }

        base.Think(performer);
    }

    void DoReload(PlayerController performer)
    {
        //AmmoTank 
        //performer.ActiveWeapon.CurrentClip = performer.ActiveWeapon.ClipSize;
    }
}
