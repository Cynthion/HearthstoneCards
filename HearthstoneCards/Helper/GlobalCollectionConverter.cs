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
                if (property.Name.Equals("Debug") || property.Name.Equals("Credits"))
                {
                    continue;
                }
                var set = new CardSet(property.Name)
                {
                    Cards = JsonConvert.DeserializeObject<Card[]>(property.Value.ToString())
                };
                // provide Set
                foreach (var card in set.Cards)
                {
                    card.Set = ExtractSet(set.SetName);
                }
                globalCollection.CardSets.Add(set);
            }

            return globalCollection;
        }

        private static Set ExtractSet(string name)
        {
            switch (name)
            {
                case "Classic": return Set.Classic;
                case "Basic": return Set.Basic;
                case "Naxxramas": return Set.Naxxramas;
                case "Goblin Vs Gnomes": return Set.GoblinVsGnomes;
                case "Blackrock Mountain": return Set.BlackrockMountain;
                case "The Grand Tournament": return Set.TheGrandTournament;
                case "League Of Explorers": return Set.LeagueOfExplorers;
                default: return Set.Classic;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(GlobalCollection));
        }
    }
}
