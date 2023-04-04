using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Entity.Weapon.Modifiers;
using Entity.Weapon;

namespace Entity.Weapon.Json
{
    public class WeaponModifierJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(WeaponModifier).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            JObject jo = JObject.Load(reader);

            JToken typeObj = jo["Type"];
            string modType = (typeObj == null) ? null : ((string)jo["Type"]);
            WeaponModifier modifier;

            switch (modType)
            {
                case null:
                    modifier = null;
                    break;
                case nameof(SelfLaunchModifier):
                    modifier = buildSelfLaunchModifier(serializer, jo);
                    break;
                default:
                    throw new FormatException($"Invalid WeaponModifier type of {modType} at {typeObj.Path}");
            }

            return modifier;
        }

        private WeaponModifier buildSelfLaunchModifier(JsonSerializer serializer, JObject jo)
        {
            SelfLaunchModifier mod = new SelfLaunchModifier();
            serializer.Populate(jo.CreateReader(), mod);

            return mod;
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
