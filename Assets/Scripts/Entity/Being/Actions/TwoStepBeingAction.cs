using Entity.Being.Dancer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Entity.Being.Actions
{
    public abstract class TwoStepBeingAction : BeingAction
    {
        public float MidStepTime;
        public float Duration;

        public override bool ActionIsInstantaneous => false;

        float startTime;

        bool midStepPassed;

        protected abstract void MidStep();

        public override void Start()
        {
            startTime = Time.time;
            midStepPassed = false;

            self.OnThink += think;

            base.Start();
        }

        private void think(object sender, EventArgs e)
        {
            if (!midStepPassed && startTime + MidStepTime < Time.time)
            {
                midStepPassed = true;
                MidStep();
            }

            if (startTime + Duration < Time.time)
            {
                self.OnThink -= think;
                OnFinish?.Invoke(this, EventArgs.Empty);
                OnFinish = null;
            }
        }
    }
}
