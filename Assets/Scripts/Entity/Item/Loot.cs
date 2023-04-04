using System;
using UnityEngine;

namespace Entity
{
    public class Loot : MonoBehaviour
    {
        private bool looted = false;
        protected bool Looted
        {
            get => looted;
            set
            {
                looted = value;
                if (looted)
                {
                    OnLooted?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public EventHandler OnLooted;
        public EventHandler OnRespawned;

        public virtual void Respawn()
        {
            Looted = false;
            OnRespawned?.Invoke(this, EventArgs.Empty);
        }
    }
}
