using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Effects
{
    public class TracerEffect : Effect
    {
        bool effectActive = false;
        float effectEndTime = 0;
        Vector3 endPosition;

        float speed;
        public float MinSpeed;
        TrailRenderer trailRenderer;

        private void Awake()
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }

        public override void Update()
        {
            base.Update();

            if (effectActive)
            {
                if (effectEndTime < Time.time)
                {
                    effectActive = false;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, endPosition, speed * Time.deltaTime);
                }
            }
        }


        public override void Perform(EffectDataStruct effectData)
        {
            base.Perform(effectData);

            float distance = (effectData.position - effectData.origin).magnitude;
            speed = Mathf.Max(MinSpeed, distance / lifetime);
            float effectDuration = Mathf.Min(lifetime, distance / MinSpeed);

            trailRenderer.enabled = false;
            endPosition = effectData.position;
            transform.position = effectData.origin;
            effectActive = true;
            effectEndTime = Time.time + effectDuration;
            trailRenderer.enabled = true;
        }

        public override void ApplyTemplate(EffectTemplate Template)
        {
            base.ApplyTemplate(Template);
            TracerEffectTemplate tracerTemplate = Template as TracerEffectTemplate;

            TrailRenderer trailRenderer = GetComponent<TrailRenderer>();

            trailRenderer.time = tracerTemplate.Lifetime * tracerTemplate.TracerTailSize;
            MinSpeed = tracerTemplate.MinSpeed;
            lifetime = tracerTemplate.Lifetime;
        }
    }
}