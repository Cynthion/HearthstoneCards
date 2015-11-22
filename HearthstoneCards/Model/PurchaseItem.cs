using WPDevToolkit;

namespace HearthstoneCards.Model
{
    // TODO save state of pi's in settings
    public class PurchaseItem : BaseNotifyPropertyChanged
    {
        public string Id { get; private set; }
        public double Price { get; private set; }

        private bool _isPurchased;

        public PurchaseItem(string id, double price)
        {
            Id = id;
            Price = price;
            //TODO IsPurchased = BaseSettings.Load<>()
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
                    // TODO save
                }
            }
        }
    }
}
