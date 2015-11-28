using System;
using System.Collections.Generic;
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
            card.Rarity = ExtractEnum<Rarity>(jo, "rarity");
            card.Faction = ExtractValue<string>(jo, "faction");
            card.Race = ExtractValue<string>(jo, "race");
            card.Class = jo["playerClass"] != null ? (string)jo["playerClass"] : "Neutral";
            card.Text = ExtractValue<string>(jo, "text");
            card.InPlayText = ExtractValue<string>(jo, "inPlayText");
            card.Mechanics = ExtractEnums<Mechanic>(jo, "mechanics");
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

        private static TEnum ExtractEnum<TEnum>(JObject jo, string key) where TEnum : struct
        {
            TEnum extractedEnum;
            Enum.TryParse(ExtractValue<string>(jo, key), true, out extractedEnum);
            return extractedEnum;
        }

        private static IList<TEnum> ExtractEnums<TEnum>(JObject jo, string key) where TEnum : struct
        {
            var enums = new List<TEnum>();
            if (jo[key] == null)
            {
                return enums;
            }
            var items = JsonConvert.DeserializeObject<IEnumerable<string>>(jo[key].ToString());
            if (items != null)
            {
                foreach (var item in items)
                {
                    TEnum extractedEnum;
                    Enum.TryParse(item, out extractedEnum);
                    enums.Add(extractedEnum);
                }
            }
            return enums;
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Card));
        }
    }
}
