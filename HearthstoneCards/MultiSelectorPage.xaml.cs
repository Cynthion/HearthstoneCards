using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WPDevToolkit.Selection;

namespace HearthstoneCards
{
    public sealed partial class MultiSelectorPage : BasePage, INotifyPropertyChanged
    {
        private MultiSelector _multiSelector;

        public MultiSelectorPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // provide the control itself as datacontext
            var ms = e.Parameter as MultiSelector;
            if (ms != null)
            {
                MultiSelector = ms;
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // forward to control
            _multiSelector.OnSelectionChanged(sender, e);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), null);
            Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);
        }

        private void AllButton_OnClick(object sender, RoutedEventArgs e)
        {
            XListView.SelectAll();
        }

        private void NoneButton_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO correct, so far only works with SelectionItem
            if (XListView.Items != null)
            {
                foreach (var item in XListView.Items)
                {
                    var selectionItem = item as ISelectionItem;
                    if (selectionItem != null)
                    {
                        selectionItem.IsSelected = false;
                    }
                }
            }
        }

        public MultiSelector MultiSelector
        {
            get { return _multiSelector; }
            private set
            {
                if (_multiSelector != value)
                {
                    _multiSelector = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChangedImpl(string propertyName)
        {
            var handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            NotifyPropertyChangedImpl(propertyName);
        }
    }
}
