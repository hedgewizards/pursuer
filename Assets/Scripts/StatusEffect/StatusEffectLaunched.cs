using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectLaunched : StatusEffect
{

    public StatusEffectLaunched(PlayerController target, float duration)
        : base(target, duration)
    {

    }

    protected override void OnApplyEffect()
    {
        throw new System.NotImplementedException();
    }

    protected override bool OnInteract(StatusEffect eff)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnRemoveEffect()
    {
        throw new System.NotImplementedException();
    }
}
