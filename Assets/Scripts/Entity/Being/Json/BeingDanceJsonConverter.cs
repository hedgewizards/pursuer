using Entity.Being.Dancer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Entity.Being.Json
{
    public class BeingDanceJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BeingDance);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            JObject jo = JObject.Load(reader);

            BeingDance beingDance = new BeingDance();
            serializer.Populate(jo.CreateReader(), beingDance);

            // convert the AnimationParameter if applicable
            switch (beingDance.AnimationType)
            {
                case BeingDance.AnimatorParameterType.Bool:
                    beingDance.AnimationParameter = jo["AnimationParameter"].ToObject<bool>();
                    break;
                case BeingDance.AnimatorParameterType.Float:
                    beingDance.AnimationParameter = jo["AnimationParameter"].ToObject<float>();
                    break;
                default:
                    break;
            }

            // convert the sound path if applicable
            JToken soundPathObj = jo["SoundPath"];
            if (soundPathObj != null)
            {
                //todo: if i know all sounds have unique paths maybe i can cache them seomehow instead of loading multiple times
                beingDance.Sound = Resources.Load<AudioClip>((string)soundPathObj);
                if (beingDance.Sound == null)
                {
                    Debug.LogWarning($"Could not find sound {(string)soundPathObj}");
                }
            }

            return beingDance;

        }

        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
