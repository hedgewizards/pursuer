using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace World
{
    [RequireComponent(typeof(Collider))]
    public abstract class Trigger : MonoBehaviour
    {
        private void OnValidate()
        {
            if (gameObject.layer != LAYER.trigger)
            {
                Debug.LogWarning("Trigger may have improper layer");
            }
            GetComponent<Collider>().isTrigger = true;
            DoValidation();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnPlayerEnter(other.gameObject);
            }
            else if (other.CompareTag("NPC"))
            {
                OnNPCEnter(other.gameObject);
            }
            else
            {
                OnOtherEnter(other.gameObject);
            }
        }

        public virtual void Start() { }
        public virtual void OnPlayerEnter(GameObject other) { }
        public virtual void OnNPCEnter(GameObject other) { }
        public virtual void OnOtherEnter(GameObject other) { }
        public virtual void DoValidation() { }
    }
}
