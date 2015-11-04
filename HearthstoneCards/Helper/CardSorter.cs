using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HearthstoneCards.CollectionViewEx;
using HearthstoneCards.Model;

namespace HearthstoneCards.Helper
{
    // TODO extract to generic Sorter class
    public class CardSorter : IComparer<Card>
    {
        private IList<SortDescription> _sortDescriptions;

        public CardSorter(IList<SortDescription> sortDescriptions)
        {
            _sortDescriptions = sortDescriptions;
        }

        public int Compare(Card x, Card y)
        {
            throw new NotImplementedException();
        }
    }
}
