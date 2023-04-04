using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Entity.Being.Actions
{
    public class BasicStagger : TimedBeingAction
    {

        public override void Start()
        {
            base.Start();

            self.Table.SpeedStat.OnModifyStat += onApplySpeedModifier;

            OnFinish += finish;
        }

        private void finish(object sender, EventArgs e)
        {
            self.Table.SpeedStat.OnModifyStat -= onApplySpeedModifier;
        }

        private void onApplySpeedModifier(object sender, ScalarStat.OnModifyEventArgs e)
        {
            if (!e.ReadOnly)
            {
                e.Scale = 0;
                e.ReadOnly = true;
            }
        }
    }
}
