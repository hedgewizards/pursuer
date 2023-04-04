using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectSlow : StatusEffect
{
    float slowAmount; //fraction of topspeed to remove

    /// <summary>
    /// 
    /// </summary>
    /// <param name="duration">time in seconds effect lasts</param>
    /// <param name="slowAmount">fraction of topspeed to remove</param>
    public StatusEffectSlow(PlayerController target, float duration, float _slowAmount)
        : base(target, duration)
    {
        slowAmount = _slowAmount;
    }

    //get/set
    public float GetSlowAmount()
    {
        return slowAmount;
    }

    //Required Hooks
    protected override void OnApplyEffect()
    {
        player.slowModifier = 1-slowAmount;
    }
    protected override bool OnInteract(StatusEffect eff)
    {
        //newest slow always applies
        DoRemoveEffect();
        return false;
    }
    protected override void OnRemoveEffect()
    {
        player.slowModifier = 1;
    }
}
