using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            card.CardId = ExtractValue<string>(jo, "cardId");
            card.Name = ExtractValue<string>(jo, "name");
            card.CardSet = ExtractValue<string>(jo, "cardSet");
            card.Type = ExtractValue<string>(jo, "type");
            card.Faction = ExtractValue<string>(jo, "faction");
            card.Race = ExtractValue<string>(jo, "race");
            card.Rarity = ExtractValue<string>(jo, "rarity");
            card.Text = ExtractValue<string>(jo, "text");
            card.Flavor = ExtractValue<string>(jo, "flavor");
            card.Artist = ExtractValue<string>(jo, "artist");

            card.Cost = ExtractValue<int>(jo, "cost");
            card.Attack = ExtractValue<int>(jo, "attack");
            card.Health = ExtractValue<int>(jo, "health");

            card.IsCollectible = ExtractValue<bool>(jo, "collectible");
            card.IsElite = ExtractValue<bool>(jo, "elite");

            card.Img = ExtractValue<string>(jo, "img");
            card.ImgGold = ExtractValue<string>(jo, "imgGold");
            card.Locale = ExtractValue<string>(jo, "locale");
            card.Class = ExtractValue<string>(jo, "playerClass");
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
