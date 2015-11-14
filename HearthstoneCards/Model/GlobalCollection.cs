using System.Collections.Generic;
using HearthstoneCards.Helper;
using Newtonsoft.Json;

namespace HearthstoneCards.Model
{
    [JsonConverter(typeof(GlobalCollectionConverter))]
    public class GlobalCollection
    {
        public IList<CardSet> CardSets { get; private set; }

        public GlobalCollection()
        {
            CardSets = new List<CardSet>();
        }
    }
}
