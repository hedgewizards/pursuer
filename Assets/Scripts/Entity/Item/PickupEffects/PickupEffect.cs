using System;
using System.Collections;
using UnityEngine;

namespace Entity.PickupEffects
{
    public abstract class PickupEffect : MonoBehaviour
    {
        public Loot Parent;

        public virtual void Awake()
        {
            try
            {
                Parent.OnRespawned += OnRespawned;
                Parent.OnLooted += OnLooted;
            }
            catch(NullReferenceException e)
            {
                Debug.LogError("Missing Parent on PickupEffect", gameObject);
            }
        }



        protected abstract void OnRespawned(object sender, EventArgs e);

        protected abstract void OnLooted(object sender, EventArgs e);
    }
}
