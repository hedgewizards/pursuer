using Combat;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "new attack", menuName = "ScriptableObjects/Weapon/Attacks/WeaponAttackHurtbox", order = 1)]
public class WeaponAttackHurtbox : WeaponAttack
{
    public float Damage;
    public DamageInfo.TYPE AttackType;

    public AudioClip AttackSound;

    public override void Perform(PlayerController performer)
    {
        DamageInfo d = new DamageInfo(Damage, AttackType, performer.transform.position);
        //performer.ActiveWeapon.SetHurtboxDamageInfo(d);

        if (AttackSound != null) performer.AUD.PlayOnce(AttackSound);
    }
}
