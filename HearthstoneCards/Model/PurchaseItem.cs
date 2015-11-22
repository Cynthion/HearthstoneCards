using System;
using System.Globalization;
using WPDevToolkit;

namespace HearthstoneCards.Model
{
    public class PurchaseItem : BaseNotifyPropertyChanged
    {
        public string Id { get; private set; }
        public double Price { get; private set; }

        private bool _isPurchased;

        public PurchaseItem(string id, double price)
        {
            Id = id;
            Price = price;
        }

        public bool IsPurchased
        {
            get { return _isPurchased; }
            set
            {
                if (_isPurchased != value)
                {
                    _isPurchased = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override string ToString()
        {
            return Price.ToString("C", CultureInfo.CurrentCulture);
        }
    }
}
