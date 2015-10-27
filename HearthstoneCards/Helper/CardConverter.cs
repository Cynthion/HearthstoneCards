using System;
using HearthstoneCards.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WPDevToolkit;

namespace HearthstoneCards.Helper
{
    public class CardConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            var card = new Card();
            card.Name = ExtractValue<string>(jo, "name");
            card.Cost = ExtractValue<int>(jo, "cost");
            card.Type = ExtractValue<string>(jo, "type");
            card.Rarity = ExtractValue<string>(jo, "rarity");
            card.Faction = ExtractValue<string>(jo, "faction");
            card.Race = ExtractValue<string>(jo, "race");
            card.Class = ExtractValue<string>(jo, "playerClass");
            card.Text = ExtractValue<string>(jo, "text");
            card.InPlayText = ExtractValue<string>(jo, "inPlayText");
            // card.Mechanics = ExtractValue<List<string>>(jo, "mechanics"); // TODO fix
            card.Flavor = ExtractValue<string>(jo, "flavor");
            card.Artist = ExtractValue<string>(jo, "artist");
            card.Attack = ExtractValue<int>(jo, "attack");
            card.Health = ExtractValue<int>(jo, "health");
            card.Durability = ExtractValue<int>(jo, "durability");
            card.Id = ExtractValue<string>(jo, "id");
            card.IsCollectible = ExtractValue<bool>(jo, "collectible");
            card.IsElite = ExtractValue<bool>(jo, "elite");
            card.HowToGet = ExtractValue<string>(jo, "howToGet");
            card.HowToGetGold = ExtractValue<string>(jo, "howToGetGold");

            return card;
        }

        private static T ExtractValue<T>(JObject jo, string key) 
        {
            return jo[key] != null ? jo[key].CastToGeneric<T>() : default(T);
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Card));
        }
    }
}
