using WPDevToolkit.Selection;

namespace HearthstoneCards.Model
{
    public class ImageSelectionItem<T> : SelectionItem<T>
    {
        public string ImagePath { get; set; }

        public ImageSelectionItem(string key) : base(key)
        {
        }

        public ImageSelectionItem(string key, T value) : base(key, value)
        {
        }

        public ImageSelectionItem(string key, T value, string display) : base(key, value, display)
        {
        }
    }
}
