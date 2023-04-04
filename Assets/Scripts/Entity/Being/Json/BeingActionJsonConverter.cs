using JsonKnownTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Entity.Being.Json
{
    public class BeingActionJsonConverter : JsonKnownTypesConverter<BeingAction>
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(BeingAction).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object actionObj = base.ReadJson(reader, objectType, existingValue, serializer);

            if (actionObj == null) return null;
            if (!(actionObj is BeingAction))
            {
                throw new FormatException($"Malformed BeingAction at {reader.Path}");
            }

            BeingAction action = actionObj as BeingAction;

            return action;
        }
    }
}