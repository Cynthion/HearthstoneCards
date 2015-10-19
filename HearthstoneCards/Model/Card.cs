using HearthstoneCards.Helper;
using Newtonsoft.Json;

namespace HearthstoneCards.Model
{
    [JsonConverter(typeof(CardConverter))]
    public class Card
    {
        public string CardId { get; set; }
        public string Name { get; set; }
        public string CardSet { get; set; }
        public string Type { get; set; }
        public string Faction { get; set; }
        public string Rarity { get; set; }
        public int Cost { get; set; }
        public int Attack { get; set; }
        public int Health { get; set; }
        public string Text { get; set; }
        public string Flavor { get; set; }
        public string Artist { get; set; }
        public bool IsCollectible { get; set; }
        public bool IsElite { get; set; }
        public string Race { get; set; }
        public string Img { get; set; }
        public string ImgGold { get; set; }
        public string Locale { get; set; }
    }
}
