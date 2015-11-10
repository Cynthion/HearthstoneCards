using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WPDevToolkit;

namespace HearthstoneCards.Helper
{
    /// <summary>
    /// Contains the necessary information for an <see cref="ItemsControl"/> that changes view information during runtime.
    /// </summary>
    public class ItemsControlViewInfo : BaseNotifyPropertyChanged
    {
        private Style _style;
        private ItemsPanelTemplate _itemsPanelTemplate;
        private DataTemplate _itemTemplate;

        public Style Style
        {
            get { return _style; }
            set
            {
                if (_style != value)
                {
                    _style = value;
                    NotifyPropertyChanged();
                }
            }
            
        }

        public ItemsPanelTemplate ItemsPanelTemplate
        {
            get { return _itemsPanelTemplate; }
            set
            {
                if (_itemsPanelTemplate != value)
                {
                    _itemsPanelTemplate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DataTemplate ItemTemplate
        {
            get { return _itemTemplate; }
            set
            {
                if (_itemTemplate != value)
                {
                    _itemTemplate = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
