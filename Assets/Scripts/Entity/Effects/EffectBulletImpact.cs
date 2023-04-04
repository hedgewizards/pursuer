using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Effects
{
    public class EffectBulletImpact : Effect
    {
        public GameObject BulletDecal;


        ParticleSystem[] systems;
        AudioSource audioSource;
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            systems = GetComponentsInChildren<ParticleSystem>();
        }

        public override void ApplyTemplate(EffectTemplate Template)
        {
            base.ApplyTemplate(Template);

            EffectBulletImpactTemplate impactTemplate = Template as EffectBulletImpactTemplate;

            if (BulletDecal != null)
            {
                BulletDecal.transform.localScale = Vector3.one * impactTemplate.DecalScale;
                BulletDecal.GetComponent<MeshRenderer>().material = impactTemplate.DecalMaterial;
            }
        }

        public override void Perform(EffectDataStruct effectData)
        {
            base.Perform(effectData);

            transform.position = effectData.position;
            transform.rotation = Quaternion.LookRotation(effectData.normal, Vector3.up);
            transform.parent = effectData.parent;
            transform.localScale = Vector3.one;

            if (audioSource != null) audioSource.Play();

            foreach (ParticleSystem p in systems)
            {
                p.Play();
            }
        }
    }
}