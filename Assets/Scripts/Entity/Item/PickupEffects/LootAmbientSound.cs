using System;
using System.Collections;
using UnityEngine;

namespace Entity.PickupEffects
{
    [RequireComponent(typeof(AudioSource))]
    public class LootAmbientSound : PickupEffect
    {
        AudioSource audioSource;

        [Range(0,1)]
        public float RegularVolume;
        [Range(0, 1)]
        public float DuckedVolume;
        public float FadeoutDuration;
        public float FadeinDuration;

        public override void Awake()
        {
            base.Awake();
            audioSource = GetComponent<AudioSource>();
            StartCoroutine(Fadein());
        }

        IEnumerator Fadeout()
        {
            audioSource.volume = DuckedVolume;

            float startTime = Time.time;
            while (Time.time < startTime + FadeoutDuration)
            {
                float t = (Time.time - startTime) / FadeoutDuration;
                audioSource.volume = Mathf.Lerp(RegularVolume, DuckedVolume, t);
                yield return null;
            }
            audioSource.Stop();
        }


        IEnumerator Fadein()
        {
            audioSource.volume = DuckedVolume;
            audioSource.Play();

            float startTime = Time.time;
            while(Time.time < startTime + FadeinDuration)
            {
                float t = (Time.time - startTime) / FadeinDuration;
                audioSource.volume = Mathf.Lerp(DuckedVolume, RegularVolume, t);
                yield return null;
            }
            audioSource.volume = RegularVolume;
        }

        protected override void OnRespawned(object sender, EventArgs e)
        {
            StartCoroutine(Fadein());
        }

        protected override void OnLooted(object sender, EventArgs e)
        {
            StartCoroutine(Fadeout());
        }
    }
}
