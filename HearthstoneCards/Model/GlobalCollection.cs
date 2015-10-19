using System.Collections.Generic;
using HearthstoneCards.Helper;
using Newtonsoft.Json;

namespace HearthstoneCards.Model
{
    [JsonConverter(typeof(GlobalCollectionConverter))]
    public class GlobalCollection
    {
        public IList<Set> Sets;

        public GlobalCollection()
        {
            Sets = new List<Set>(14);
        }
    }
}
