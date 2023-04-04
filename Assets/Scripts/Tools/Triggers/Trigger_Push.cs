using UnityEngine;
using System.Collections;

namespace World
{
    public class Trigger_Push : Trigger
    {
        [SerializeField] Vector3 PUSH_DIR; //direction to apply push in
        [SerializeField] float PUSH_FORCE = 0; //force to apply (m/s^2)
        [SerializeField] float PUSH_MAX = 0; //max speed attainable with the push

        public override void DoValidation()
        {
            base.DoValidation();
            if (PUSH_DIR.magnitude > 1)
            {
                PUSH_DIR /= PUSH_DIR.magnitude;
            }

        }
        void OnTriggerStay(Collider other)
        {
            other.gameObject.SendMessage("ApplyForce", new ForceInfo(transform.rotation * PUSH_DIR, PUSH_FORCE, PUSH_MAX));
        }
    }

    public struct ForceInfo
    {
        public Vector3 direction;
        public float magnitude;
        public float maxSpeed;

        public ForceInfo(Vector3 _direction, float _magnitude, float _maxSpeed)
        {
            direction = _direction;
            magnitude = _magnitude;
            maxSpeed = _maxSpeed;
        }

        public ForceInfo(Vector3 _force, float _maxSpeed)
        {
            direction = _force.normalized;
            magnitude = _force.magnitude;
            maxSpeed = _maxSpeed;
        }
    }
}