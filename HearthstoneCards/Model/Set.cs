using System.Collections.Generic;

namespace HearthstoneCards.Model
{
    public class Set
    {
        public string Name { get; private set; }

        public IList<Card> Cards { get; set; }

        public Set(string name)
        {
            Name = name;
            Cards = new List<Card>();
        }
    }
}
