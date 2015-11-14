using System.Collections.Generic;

namespace HearthstoneCards.Model
{
    public class CardSet
    {
        public string SetName { get; private set; }

        public IList<Card> Cards { get; set; }

        public CardSet(string setName)
        {
            SetName = setName;
            Cards = new List<Card>();
        }
    }
}
