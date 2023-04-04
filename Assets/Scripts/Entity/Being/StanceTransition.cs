using Entity.Being.Triggers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Being
{
    [JsonConverter(typeof(Json.StanceTransitionJsonConverter))]
    public class StanceTransition
    {
        public Trigger Trigger;

        internal static Trigger makeDefaultTrigger(float weight = 1)
        {
            return new TriggerDecision(weight);
        }
    }
}