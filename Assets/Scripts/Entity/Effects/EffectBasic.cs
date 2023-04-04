using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Effects
{

    /// <summary>
    /// Plays an attached AudioSource, and all attached particleSystems on perform
    /// </summary>
    public class EffectBasic : Effect
    {
        public override void ApplyTemplate(EffectTemplate Template)
        {
            base.ApplyTemplate(Template);
        }

        public override void Perform(EffectDataStruct effectData)
        {
            base.Perform(effectData);

            transform.position = effectData.position;
            transform.rotation = Quaternion.LookRotation(effectData.normal, Vector3.up);
            transform.parent = effectData.parent;
            transform.localScale = Vector3.one;

            AudioSource a = GetComponent<AudioSource>();
            if (a != null) a.Play();

            foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
            {
                p.Play();
            }
        }
    }
}