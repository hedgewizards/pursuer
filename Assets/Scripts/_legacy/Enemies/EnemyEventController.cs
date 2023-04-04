using Combat;
using Entity.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEventController : MonoBehaviour
{
    public EffectEntry[] effects;
    public HurtboxEntry[] hurtboxes;
    DamageInfo cachedDamageInfo;
    public EnemyController enemyController;

    public void PerformEffect(string effectName)
    {
        GetEffect(effectName).Perform(new EffectDataStruct());
    }
    public void ActivateHurtbox(string name)
    {
        GetHurtbox(name).Activate(enemyController);
    }
    public void DeactivateHurtbox(string name)
    {
        GetHurtbox(name).Deactivate();
    }

    public Effect GetEffect(string effectName)
    {
        foreach (EffectEntry entry in effects)
        {
            if (entry.name == effectName)
            {
                return entry.effect;
            }
        }

        throw new System.Exception("missing effect named " + effectName);
    }
    public EnemyHurtbox GetHurtbox(string hurtboxName)
    {
        foreach (HurtboxEntry entry in hurtboxes)
        {
            if (entry.name == hurtboxName)
            {
                return entry.hurtbox;
            }
        }

        throw new System.Exception("missing hurtbox named " + hurtboxName);
    }

    [System.Serializable] public struct EffectEntry
    {
        public string name;
        public Effect effect;
    }

    [System.Serializable] public struct HurtboxEntry
    {
        public string name;
        public EnemyHurtbox hurtbox;
    }
}