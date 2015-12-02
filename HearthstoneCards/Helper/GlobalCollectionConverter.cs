using System;
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
                // only sets that are of interest
                if (property.Name.Equals("Debug") 
                    || property.Name.Equals("Credits")
                    || property.Name.Equals("Hero Skins")
                    || property.Name.Equals("Missions")
                    || property.Name.Equals("Promotion")
                    || property.Name.Equals("Reward")
                    || property.Name.Equals("System")
                    || property.Name.Equals("Tavern Brawl"))
                {
                    continue;
                }
                var set = new CardSet(property.Name)
                {
                    Cards = JsonConvert.DeserializeObject<Card[]>(property.Value.ToString())
                };
                // provide Set
                var setEnum = ExtractSet(set.SetName);
                foreach (var card in set.Cards)
                {
                    card.Set = setEnum;
                }
                globalCollection.CardSets.Add(set);
            }

            return globalCollection;
        }

        private static Set ExtractSet(string name)
        {
            // TODO merge with other name-to-enum methods
            switch (name)
            {
                case "Classic": return Set.Classic;
                case "Basic": return Set.Basic;
                case "Curse of Naxxramas": return Set.Naxxramas;
                case "Goblins vs Gnomes": return Set.GoblinVsGnomes;
                case "Blackrock Mountain": return Set.BlackrockMountain;
                case "The Grand Tournament": return Set.TheGrandTournament;
                case "League of Explorers": return Set.LeagueOfExplorers;
                default: return Set.Classic;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(GlobalCollection));
        }
    }
}
