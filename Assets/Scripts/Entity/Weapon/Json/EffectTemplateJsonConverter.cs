using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Combat;

namespace Entity.Effects
{
    public class EffectTemplateJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(EffectTemplate).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonToken.String)
            {
                throw new FormatException($"Unexpected token of type {reader.TokenType} at {reader.Path}");
            }

            string effectName = reader.Value as string;

            EffectTemplate effect = Resources.Load<EffectTemplate>($"Effects/{effectName}");
            if (effect == null)
            {
                throw new System.IO.FileNotFoundException($"Missing effect: Effects/{effectName}");
            }

            return effect;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite
        {
            get { return false; }
        }
    }
}
