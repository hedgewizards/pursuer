using UnityEngine;
using System;
using Newtonsoft.Json;
using JsonKnownTypes;
using Entity.Being.Triggers;

namespace Entity.Being
{
    [JsonConverter(typeof(JsonKnownTypesConverter<Trigger>))]
    [JsonDiscriminator(Name = "Type")]
    [JsonKnownType(typeof(TriggerStagger), nameof(TriggerStagger))]
    [JsonKnownType(typeof(TriggerDecision), nameof(TriggerDecision))]
    public class Trigger : BeingComponent
    {
        public EventHandler OnTriggered;

        public void Activate()
        {
            OnTriggered?.Invoke(this, EventArgs.Empty);
        }
    }
}