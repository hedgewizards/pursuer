using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Being.Triggers
{
    public class TriggerDecision : Trigger, IDecideable
    {
        float weight = 1;
        public float Weight => weight;

        public TriggerDecision(float weight)
        {
            this.weight = weight;
        }

        public override void Initialize(BeingActor self)
        {
            base.Initialize(self);
            self.Table.RegisterDecision(this);
        }

        public override void Detatch()
        {
            base.Detatch();
            self.Table.UnregisterDecision(this);
        }

        public void Decide()
        {
            Activate();
        }
    }
}
