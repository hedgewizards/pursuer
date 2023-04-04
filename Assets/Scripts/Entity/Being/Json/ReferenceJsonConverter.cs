using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Being.Json
{
    /// <summary>
    /// the way AmmunitionJsonConverter works but more generalized
    /// </summary>
    public abstract class ReferenceJsonConverter : JsonConverter
    {
        public abstract object Default { get; }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return Default;
            }
            else if (reader.TokenType == JsonToken.String)
            {
                return LoadFromIdentifier(reader.Value as string);
            }
            else
            {
                JObject jo = JObject.Load(reader);
                object result = Activator.CreateInstance(objectType);
                serializer.Populate(jo.CreateReader(), result);

                return result;
            }
        }

        protected abstract object LoadFromIdentifier(string Identifier);
    }
}
