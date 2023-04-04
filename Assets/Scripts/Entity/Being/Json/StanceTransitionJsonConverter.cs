using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Being.Json
{
    public class StanceTransitionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(StanceTransition);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            JObject jo = JObject.Load(reader);

            StanceTransition transition = new StanceTransition();
            serializer.Populate(jo.CreateReader(), transition);

            if (transition.Trigger == null)
            {
                JToken weightObj = jo["Weight"];
                float weight = (weightObj == null) ? 1 : ((float)jo["Weight"]);

                transition.Trigger = StanceTransition.makeDefaultTrigger(weight);
            }

            return transition;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
