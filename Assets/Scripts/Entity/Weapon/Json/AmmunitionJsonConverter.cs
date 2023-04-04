using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Combat;

namespace Entity.Weapon.Json
{
    public class AmmunitionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Ammunition);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return Resources.Load<Ammunition>("Ammo/GenericAmmo");
            }

            if (reader.TokenType != JsonToken.String)
            {
                throw new FormatException($"Unexpected token of type {reader.TokenType} at {reader.Path}");
            }

            string ammoName = reader.Value as string;

            Ammunition ammo = Resources.Load<Ammunition>($"Ammo/{ammoName}");
            if (ammo == null)
            {
                throw new System.IO.FileNotFoundException($"Missing ammunition: Ammo/{ammoName}");
            }
            return ammo;
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
