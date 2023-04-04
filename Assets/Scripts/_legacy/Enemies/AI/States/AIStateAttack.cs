using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Perform an attack
/// </summary>
public class AIStateAttack : AIState
{
    public float damage;
    public DamageInfo.TYPE damageType;

    public string AnimationTriggerName;
    public EnemyHurtbox hurtbox;

    public override AIStateData OnEnter(AIStateData data)
    {
        data = base.OnEnter(data);

        self.animator.SetTrigger(AnimationTriggerName);

        DamageInfo d = new DamageInfo();
        d.Damage = damage;
        d.DamageType = damageType;
        d.Afflictions = new Affliction[0];
        hurtbox.cachedDamageInfo = d;

        return data;
    }
}