using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Being.Actions
{
    public class SelfBuff : TimedBeingAction
    {
        public bool PerformableAtMaxStacks = true;
        public BeingStatusEffect StatusEffect;
        public int StacksToApply = 1;

        public override void Start()
        {
            base.Start();

            self.ApplyStatus(StatusEffect, StacksToApply);
        }
    }
}
