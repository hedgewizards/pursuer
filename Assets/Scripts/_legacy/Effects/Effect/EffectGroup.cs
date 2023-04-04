using Entity.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectGroup : Effect
{
    public Effect[] Effects;

    public override void ApplyTemplate(EffectTemplate Template)
    {
        base.ApplyTemplate(Template);
        throw new System.NotImplementedException();
    }

    public override void Perform(EffectDataStruct effectData)
    {
        foreach (Effect effect in Effects)
        {
            lifetime = Mathf.Max(lifetime, effect.lifetime);
            effect.OnFinish += (sender, e) =>
            {
                effect.transform.parent = transform;
                effect.transform.localScale = Vector3.one;
            };
            effect.Perform(effectData);
        }
    }
}
