using System;
using UnityEngine;

namespace Entity.Effects
{
    public abstract class Effect : MonoBehaviour
    {
        [HideInInspector] public string effectID;
        public float lifetime;
        [HideInInspector] public bool idle = true;
        float finishTime;
        [HideInInspector] public bool IsLastingEffect;

        public EventHandler OnFinish;

        public virtual void Perform(EffectDataStruct effectData)
        {
            idle = false;
            finishTime = Time.time + lifetime;
        }

        public virtual void Update()
        {
            if (!idle && Time.time > finishTime)
            {
                idle = true;
                OnFinish?.Invoke(this, EventArgs.Empty);
                FinishEffect();
            }
        }

        protected virtual void FinishEffect()
        {
            EffectManager.EffectFinished(this);
        }

        public virtual void ApplyTemplate(EffectTemplate Template)
        {
            IsLastingEffect = Template.IsLastingEffect;
        }
    }
}