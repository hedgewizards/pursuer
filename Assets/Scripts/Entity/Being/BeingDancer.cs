using Entity.Being.Dancer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Entity.Being
{

    [RequireComponent(typeof(AudioSource))]
    public class BeingDancer : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float Volume = 1;

        #region Components
        public Animator Animator;
        AudioSource audioSource;
        public BeingActor self;
        #endregion

        public float CurrentSpeed => self.CurrentSpeed;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        internal void Initialize(BeingActor self)
        {
            this.self = self;
        }

        #region Dances
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dance"></param>
        /// <param name="onDanceFinished">listeners are added to this event to clean up animation stuff. this function does not clean up its events!!!</param>
        public void PerformDance(BeingDance dance, ref EventHandler onDanceFinished)
        {
            //Debug.Log($"Performing Dance {dance.Name?? "(unnamed)"}");
            dance.DoAnimations(Animator, ref onDanceFinished);

            if (dance.Sound != null)
            {
                audioSource.PlayOneShot(dance.Sound, Volume);
            }
        }
        #endregion

        internal void TriggerAnimation(string animationTrigger)
        {
            Animator.SetTrigger(animationTrigger);
        }

        internal void SetAnimationBool(string animationName, bool state)
        {
            Animator.SetBool(animationName, state);
        }
    }
}
