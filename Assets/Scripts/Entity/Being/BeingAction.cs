using Entity.Being.Actions;
using Entity.Being.Dancer;
using Entity.Being.Triggers;
using JsonKnownTypes;
using Newtonsoft.Json;
using System;

namespace Entity.Being
{
    [JsonConverter(typeof(Json.BeingActionJsonConverter))]
    [JsonDiscriminator(Name = "Type")]
    [JsonKnownType(typeof(SelfBuff), nameof(SelfBuff))]
    [JsonKnownType(typeof(BasicStagger), nameof(BasicStagger))]
    public abstract class BeingAction : BeingComponent
    {
        public EventHandler OnStart;
        public EventHandler OnFinish;

        public BeingDance StartDance;

        public string Name;

        /// <summary>
        /// The trigger which will cause this action.<br/>
        /// If not defined when initializing, it will be added as an AI decision
        /// </summary>
        public Trigger Trigger;

        public abstract bool ActionIsInstantaneous { get; }

        public override void Initialize(BeingActor self)
        {
            if (Trigger == null) Trigger = makeDefaultTrigger();
            base.Initialize(self);
            Trigger.Initialize(self);

            Trigger.OnTriggered += onTriggered;
        }

        private void onTriggered(object sender, EventArgs e)
        {
            self.Table.PerformAction(this);
        }

        public virtual void Start()
        {
            OnStart?.Invoke(this, EventArgs.Empty);

            if (StartDance != null)
            {
                self.Dancer?.PerformDance(StartDance, ref OnFinish);
            }

            if (ActionIsInstantaneous)
            {
                OnFinish?.Invoke(this, EventArgs.Empty);
            }
        }

        internal static Trigger makeDefaultTrigger(float weight = 1)
        {
            return new TriggerDecision(weight);
        }
    }
}
