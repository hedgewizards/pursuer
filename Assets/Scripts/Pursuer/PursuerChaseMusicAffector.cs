
using Entity.Being;
using System;
using UnityEngine;

namespace Pursuer
{
    [RequireComponent(typeof(AudioSource))]
    public class PursuerChaseMusicAffector : MonoBehaviour
    {
        private AudioSource source;
        public BeingActor Target;

        public float MinimumPitch;
        public float MinimumPitchSpeed;

        public float MaximumPitch;
        public float MaximumPitchSpeed;

        private void Awake()
        {
            source = GetComponent<AudioSource>();

            PursuerGameManager.Instance.OnPauseStateChanged += OnPauseStateChanged;
        }

        bool paused = false;
        private void OnPauseStateChanged(object sender, bool nowPaused)
        {
            paused = nowPaused;
            if (paused && source)
            {
                source.Pause();
            }
            else
            {
                source.Play();
            }
        }

        private void Update()
        {
            float t = (Target.Table.SpeedStat.Value - MinimumPitchSpeed) / (MaximumPitchSpeed - MinimumPitchSpeed);
            source.pitch = Mathf.Lerp(MinimumPitch, MaximumPitch, t);
        }
    }
}
