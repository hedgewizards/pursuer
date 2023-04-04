using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Entity.Being;

namespace Entity.Being.Json
{
    public class BeingTableJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BeingTable);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            JObject jo = JObject.Load(reader);
            BeingTable table = new BeingTable();
            serializer.Populate(jo.CreateReader(), table);

            JToken InitialStanceToken = jo["InitialStance"];
            string initialStanceName = (InitialStanceToken == null) ? null : ((string)jo["InitialStance"]);

            if (initialStanceName == null)
            {
                table.InitialStance = table.Stances[0];
            }
            else
            {
                foreach (BeingStance stance in table.Stances)
                {
                    if (stance.Name == initialStanceName)
                    {
                        table.InitialStance = stance;
                        break;
                    }
                }
            }
            if (table.InitialStance == null)
            {
                throw new FormatException($"Could not find initial stance named {initialStanceName ?? "[NULL]"}");
            }

            return table;
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
