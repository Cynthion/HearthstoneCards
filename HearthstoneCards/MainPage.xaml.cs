using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using HearthstoneCards.Helper;
using HearthstoneCards.Model;
using HearthstoneCards.ViewModel;
using WPDevToolkit;

namespace HearthstoneCards
{
    public sealed partial class MainPage : BasePage
    {
        private readonly MainViewModel _mainVm;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            // initialize app
            Initializer.Initialize();

            // set data context
            _mainVm = SingletonLocator.Get<MainViewModel>();
            DataContext = _mainVm;

            Loaded += MainPage_OnLoaded;

            // listen for back-button
            HardwareButtons.BackPressed += (sender, args) =>
            {
                if (_mainVm.IsSortingControlVisible)
                {
                    _mainVm.ApplySort();
                    _mainVm.ToggleSorterControlVisibility();
                    args.Handled = true;
                }
            };
        }

        public async void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            // load UI
            await TaskHelper.HandleTaskObservedAsync(_mainVm.LoadAsync());
        }

        private async void MultiSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await _mainVm.OnQueryChangedAsync();
        }

        private void CardList_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var card = e.ClickedItem as Card;
            var listView = sender as ListView;
            if (card != null && listView != null)
            {
                Frame.Navigate(typeof (CollectionPage), listView.Items.IndexOf(card));
            }
        }

        private void SorterButton_OnClick(object sender, RoutedEventArgs e)
        {
            _mainVm.ToggleSorterControlVisibility();
            _mainVm.ApplySort();
        }

        private void ApplySortButton_OnClick(object sender, RoutedEventArgs e)
        {
            _mainVm.ApplySort();
        }

        private void SortConfigurationElement_OnClicked(object sender, RoutedEventArgs e)
        {
            _mainVm.IsSortConfigurationChanged = true;
        }
    }
}
