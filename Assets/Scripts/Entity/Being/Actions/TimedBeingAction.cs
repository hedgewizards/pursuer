using System;
using UnityEngine;

namespace Entity.Being.Actions
{
    public abstract class TimedBeingAction : BeingAction
    {
        public override bool ActionIsInstantaneous => Duration <= 0;
        public float Duration;
        float endTime;

        public override void Start()
        {
            base.Start();

            if (Duration > 0)
            {
                endTime = Time.time + Duration;
                self.OnThink += CheckFinished;
            }
        }
        private void CheckFinished(object sender, EventArgs e)
        {
            if (Time.time > endTime)
            {
                self.OnThink -= CheckFinished;
                OnFinish?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}