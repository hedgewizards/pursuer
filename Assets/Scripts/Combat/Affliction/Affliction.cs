using Entity.Being;
using JsonKnownTypes;
using Newtonsoft.Json;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    [JsonConverter(typeof(JsonKnownTypesConverter<Affliction>))]
    [JsonDiscriminator(Name = "Type")]
    public abstract class Affliction
    {
        public abstract void ApplyToPlayer(DamageInfo damageInfo, PlayerController player);
        public abstract void ApplyToBeing(DamageInfo damageInfo, BeingActor being);
    }
}