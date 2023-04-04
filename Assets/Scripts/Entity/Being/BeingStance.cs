using Entity.Being.Dancer;
using Entity.Being.Stances;
using JsonKnownTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Being
{
    [JsonConverter(typeof(JsonKnownTypesConverter<BeingStance>))]
    [JsonDiscriminator(Name = "Type")]
    [JsonKnownType(typeof(IdleStance), nameof(IdleStance))]
    [JsonKnownType(typeof(ChaseStance), nameof(ChaseStance))]
    public abstract class BeingStance : BeingComponent
    {
        public string Name;

        public BeingDance EntryDance;

        public StanceTransition[] Transitions;

        public virtual float CheckModdableValue(string key)
        {
            throw new KeyNotFoundException($"Missing key \"{key}\" for {self.name} stance {Name}");
        }
        public virtual void SetModdableValue(string key, float newValue)
        {
            throw new KeyNotFoundException($"Missing key \"{key}\" for {self.name} stance {Name}");
        }

        public virtual void Enter()
        {
            if (EntryDance != null)
            {
                self.Dancer.PerformDance(EntryDance, ref onExitTransient);
            }

            OnEnter?.Invoke(this,EventArgs.Empty);
        }

        public virtual void Exit()
        {
            OnExit?.Invoke(this, EventArgs.Empty);
            onExitTransient?.Invoke(this, EventArgs.Empty);
            onExitTransient = null;
        }

        #region Events

        public EventHandler OnEnter;
        public EventHandler OnExit;

        EventHandler onExitTransient;

        #endregion endregion
    }
}
