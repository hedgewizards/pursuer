using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBundleParticles : EffectBundle
{
    public ParticleSystem[] particleSystems;

    public override void PerformEffect()
    {
        foreach(ParticleSystem p in particleSystems)
        {
            p.Play();
        }
    }
}
