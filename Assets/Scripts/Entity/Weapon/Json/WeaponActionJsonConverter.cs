using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Entity.Weapon;

namespace Entity.Weapon.Json
{
    public class WeaponActionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(WeaponAction).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return new WeaponActionNone();
            }

            JObject jo = JObject.Load(reader);

            JToken typeObj = jo["Type"];
            string actType = (typeObj == null) ? null : ((string)jo["Type"]);
            WeaponAction act;

            switch (actType)
            {
                case null:
                    act = new WeaponActionNone();
                    break;
                case "BasicReloader":
                    act = buildBasicReloader(serializer, jo);
                    break;
                case "BasicBulletAttack":
                    act = buildBasicBulletAttack(serializer, jo);
                    break;
                default:
                    throw new FormatException($"Invalid WeaponAction type of {actType} at {typeObj.Path}");
            }

            return act;
        }

        private WeaponAction buildBasicReloader(JsonSerializer serializer, JObject jo)
        {
            BasicWeaponReloader reloader = new BasicWeaponReloader();
            serializer.Populate(jo.CreateReader(), reloader);

            return reloader;
        }
        private WeaponAction buildBasicBulletAttack(JsonSerializer serializer, JObject jo)
        {
            BasicBulletAttack attack = new BasicBulletAttack();
            serializer.Populate(jo.CreateReader(), attack);

            return attack;
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
