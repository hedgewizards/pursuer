using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Dancer
{
    public class IKStrider : MonoBehaviour
    {
        public BezierSpline StrideSpline;
        public EasyStriderLeg[] Legs;

        public float StridesPerCycle;
        public float StrideRadius;
        public float StrideHeight;
        float strideDistance => StrideRadius * 4 / StridesPerCycle;


        Vector3 lastStridePosition;
        Vector3 strideBase => transform.position - transform.forward * StrideRadius;
        float currentDistanceInSet;
        int currentStrideInSet;

        private void Awake()
        {
            lastStridePosition = strideBase;
            currentStrideInSet = 0;
            currentDistanceInSet = 0;

            foreach (EasyStriderLeg leg in Legs)
            {
                leg.targetRotationBase = leg.Target.transform.localRotation;
            }
        }

        private void Update()
        {
            float distance = (strideBase - lastStridePosition).magnitude;

            if (distance > strideDistance)
            {
                TakeStep(distance);
            }

            float t = (currentDistanceInSet + distance) / (strideDistance * StridesPerCycle);

            foreach (EasyStriderLeg leg in Legs)
            {
                leg.SetParametric(t, StrideSpline, StrideRadius, StrideHeight);
            }
        }

        private void TakeStep(float addDistance)
        {
            lastStridePosition = strideBase;
            currentStrideInSet++;
            currentDistanceInSet += addDistance;
            if (currentStrideInSet >= StridesPerCycle)
            {
                currentStrideInSet = 0;
                currentDistanceInSet = currentDistanceInSet % strideDistance;
            }
        }

        #region Debug

        private void OnDrawGizmosSelected()
        {
            foreach (EasyStriderLeg leg in Legs)
            {
                Gizmos.color = new Color(1, 0, 0, .9f);
                Gizmos.DrawCube(leg.Target.transform.position, Vector3.one * 0.05f);

                Gizmos.color = new Color(0, 1, 0, .9f);
                Gizmos.DrawCube(leg.Hint.transform.position, Vector3.one * 0.05f);
            }        
        }

        #endregion
    }

    [System.Serializable]
    public class EasyStriderLeg
    {
        internal Quaternion targetRotationBase;

        public GameObject Parent;
        public GameObject Target;
        public GameObject Hint;

        [SerializeField]
        [Tooltip("from 0->1 what the initial offset should be, where 0 is fully back")]
        private float ParametricOffset;

        public EasyStriderLeg(float offset)
        {
            ParametricOffset = offset;
        }

        public void SetParametric(float t, BezierSpline spline, float strideRadius, float strideHeight)
        {
            Vector3 rawPoint = spline.GetLocalPoint((t + ParametricOffset) % 1).Scale(strideRadius, strideHeight, strideRadius);
            Vector3 worldPoint = Parent.transform.TransformPoint(rawPoint);
            SnapToPoint(Parent.transform.position + Vector3.up * strideHeight, worldPoint);
        }

        public void SnapToPoint(Vector3 origin, Vector3 targetPoint)
        {
            Ray ray = new Ray(origin, targetPoint - origin);
            float distance = (targetPoint - origin).magnitude;
            if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, LAYERMASK.SOLIDS_ONLY))
            {
                Vector3 footForward = Vector3.ProjectOnPlane(Target.transform.forward, hitInfo.normal);
                Target.transform.position = hitInfo.point;
                //Target.transform.rotation = Quaternion.LookRotation(footForward, hitInfo.normal);
            }
            else
            {
                Target.transform.position = targetPoint;
                //Target.transform.localRotation = Quaternion.identity;
            }
        }
    }
}

