using Combat;
using System;
using UnityEngine;

namespace Entity.Weapon
{
    public class WeaponUIController : MonoBehaviour
    {
        protected WeaponActor self;

        protected bool initialized = false;

        public void Initialize(WeaponActor actor)
        {
            self = actor;

            actor.OnDeploy += OnDeploy;
            actor.OnHolster += OnHolster;

            gameObject.SetActive(false);

            initialized = true;
        }

        public virtual void OnDeploy(object sender, EventArgs e)
        {
            gameObject.SetActive(true);
        }

        public virtual void OnHolster(object sender, EventArgs e)
        {
            gameObject.SetActive(false);
        }
    }
}
