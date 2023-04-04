using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entity.Being;
using UnityEngine;

namespace Entity.Being.Triggers
{
    public class TriggerAcquireMeleeTarget : Trigger
    {
        public float TestInterval = 0.2f;
        public float Radius = 1;

        float lastTest;

        public override void Initialize(BeingActor self)
        {
            base.Initialize(self);

            self.OnThink += onThink;
        }

        private void onThink(object sender, EventArgs e)
        {
            if (lastTest + TestInterval < Time.time)
            {
                lastTest = Math.Max(Time.time, lastTest + TestInterval);
                test();
            }
        }

        void test()
        {
            if (self.AcquireMeleeTarget(Radius) != null)
            {
                Activate();
            }
        }
    }
}