using UnityEngine;

namespace Entity.Being.Dancer
{
    public class BlendTreeStrider : MonoBehaviour
    {
        public Animator TargetAnimator;
        public BeingDancer TargetDancer;
        public bool HasRunAnimation = true;

        public string ParametricRunFieldName = "WalkTreeControl";
        public string RunMultiplierFieldName = "WalkTreeMultiplier";

        public float WalkAnimationSpeed = 3;
        public float RunAnimationSpeed = 6;
        public float MaxRunAnimationSpeed = 12;
        public float MaxRunAnimationMultiplier = 2;

        private void Update()
        {
                                        // 0 - 0.5 if in walking animation range
            float t = HasRunAnimation ? Mathf.Lerp(0, 0.5f, TargetDancer.CurrentSpeed / WalkAnimationSpeed)
                                         // 0.5 - 1.0 if in running animation range
                                         + Mathf.Lerp(0f, 0.5f, (TargetDancer.CurrentSpeed - WalkAnimationSpeed) / (RunAnimationSpeed - WalkAnimationSpeed))
                                      // or if there's no running animation, 0.0 - 1.0 if in walking animation range
                                      : Mathf.Lerp(0, 1f, TargetDancer.CurrentSpeed / WalkAnimationSpeed);

            float tMultiplier = Mathf.Lerp(1, MaxRunAnimationMultiplier, (TargetDancer.CurrentSpeed - RunAnimationSpeed) / (MaxRunAnimationSpeed - RunAnimationSpeed));

            TargetAnimator.SetFloat(ParametricRunFieldName, t);
            TargetAnimator.SetFloat(RunMultiplierFieldName, tMultiplier);
        }
    }
}
