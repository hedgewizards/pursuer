using Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Entity.Being.Triggers
{
    public class TriggerStagger : Trigger
    {
        public float DamageThreshold = 10f;
        public float RecoveryInterval = 0.2f;

        private float accruedDamage;
        private float lastDamageTime;

        public override void Initialize(BeingActor self)
        {
            base.Initialize(self);

            lastDamageTime = Time.time;
            accruedDamage = 0;

            self.OnTakeDamage += onTakeDamage;
        }

        private void onTakeDamage(object sender, DamageInfo e)
        {
            float timeSinceLastDamage = Time.time - lastDamageTime;
            lastDamageTime = Time.time;

            // regen stagger HP
            accruedDamage = Mathf.Max(0, accruedDamage - DamageThreshold * timeSinceLastDamage / RecoveryInterval);

            // take damage
            accruedDamage += e.FinalDamage;

            if (accruedDamage > DamageThreshold)
            {
                accruedDamage = 0;
                Activate();
            }
        }
    }
}
