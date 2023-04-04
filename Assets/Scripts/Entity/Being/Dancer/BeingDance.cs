using Entity.Being.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Entity.Being.Dancer
{
    [JsonConverter(typeof(BeingDanceJsonConverter))]
    public class BeingDance
    {
        public enum AnimatorParameterType
        {
            None,
            Trigger,
            Bool,
            Float
        }
        public string Name;
        public string AnimationName;
        public bool AnimationIsOverride;
        public AnimatorParameterType AnimationType = AnimatorParameterType.None;
        [JsonIgnore]
        public object AnimationParameter;

        [JsonIgnore]
        public AudioClip Sound;

        public void DoAnimations(Animator animator, ref EventHandler onDanceFinished)
        {
            if (AnimationIsOverride)
            {
                setAnimationOverride(animator, ref onDanceFinished);
            }

            switch (AnimationType)
            {
                case AnimatorParameterType.Trigger:
                    animator.SetTrigger(AnimationName);
                    return;
                case AnimatorParameterType.Bool:
                    setAnimationBool(animator, ref onDanceFinished);
                    return;
                case AnimatorParameterType.Float:
                    setAnimationFloat(animator, ref onDanceFinished);
                    return;
                default:
                    return;
            }
        }

        private void setAnimationOverride(Animator animator, ref EventHandler onDanceFinished)
        {
            int index = animator.GetLayerIndex("Override");
            float originalValue = animator.GetLayerWeight(index);
            animator.SetLayerWeight(index, 1);

            // create an anonymous event listener that removes itself after its called
            EventHandler cleanup = null;
            cleanup = (sender, e) =>
            {
                // return the parameter to its original value
                animator.SetLayerWeight(index, originalValue);
            };

            onDanceFinished += cleanup;
            animator.SetBool(AnimationName, (AnimationParameter as bool?) ?? true);
        }

        void setAnimationBool(Animator animator, ref EventHandler onDanceFinished)
        {
            // create an anonymous event listener that removes itself after its called
            EventHandler cleanup = null;
            bool originalValue = animator.GetBool(AnimationName);
            cleanup = (sender, e) =>
            {
                // return the parameter to its original value
                animator.SetBool(AnimationName, originalValue);
            };

            onDanceFinished += cleanup;
            animator.SetBool(AnimationName, (AnimationParameter as bool?)?? true);
        }

        void setAnimationFloat(Animator animator, ref EventHandler onDanceFinished)
        {
            // create an anonymous event listener that removes itself after its called
            EventHandler cleanup = null;
            float originalValue = animator.GetFloat(AnimationName);
            cleanup = (sender, e) =>
            {
                // that event listener returns the parameter to its original value
                animator.SetFloat(AnimationName, originalValue);
            };

            onDanceFinished += cleanup;
            animator.SetFloat(AnimationName, (AnimationParameter as float?) ?? 1f);
        }

        private void Finish(object sender, EventArgs e)
        {
            if (AnimationType == AnimatorParameterType.Bool)
            {

            }
        }
    }
}
