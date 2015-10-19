using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HearthstoneCards.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HearthstoneCards.Helper
{
    public class GlobalCollectionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            var globalCollection = new GlobalCollection();

            foreach (var property in jo.Properties())
            {
                var set = new Set(property.Name);
                set.Cards = JsonConvert.DeserializeObject<Card[]>(property.Value.ToString());
                globalCollection.Sets.Add(set);
            }

            return globalCollection;
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(GlobalCollection));
        }
    }
}
