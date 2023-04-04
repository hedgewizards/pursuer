using JsonKnownTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;


namespace Entity.Being.Json
{
    public class BeingStatusJsonConverter : JsonKnownTypesConverter<BeingStatusEffect>
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object statusEffectObj = base.ReadJson(reader, objectType, existingValue, serializer);

            if (statusEffectObj == null)
            {
                return null;
            }
            else if (!(statusEffectObj is BeingStatusEffect))
            {
                throw new FormatException($"Malformed BeingStatus at {reader.Path}");
            }

            BeingStatusEffect statusEffect = statusEffectObj as BeingStatusEffect;
            if (statusEffect.Identifier == null)
            {
                statusEffect.Identifier = reader.Path;
            }

            return statusEffect;
        }
    }
}
