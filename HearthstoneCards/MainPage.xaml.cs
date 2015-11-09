using System.Collections.Generic;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using HearthstoneCards.Helper;
using HearthstoneCards.Model;
using HearthstoneCards.ViewModel;
using WPDevToolkit;

namespace HearthstoneCards
{
    public sealed partial class 
        MainPage : BasePage
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

            // provide items panel templates
            //_mainVm.ItemsPanelTemplates

            Loaded += MainPage_OnLoaded;

            // listen for back-button
            HardwareButtons.BackPressed += async (sender, args) =>
            {
                if (_mainVm.IsSortingControlVisible)
                {
                    _mainVm.ToggleSorterControlVisibility();
                    await _mainVm.ApplySortAsync();
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

        private async void SorterButton_OnClick(object sender, RoutedEventArgs e)
        {
            _mainVm.ToggleSorterControlVisibility();
            await _mainVm.ApplySortAsync();
        }

        private async void ApplySortButton_OnClick(object sender, RoutedEventArgs e)
        {
            _mainVm.ToggleSorterControlVisibility();
            await _mainVm.ApplySortAsync();
        }

        private void SortConfigurationElement_OnClicked(object sender, RoutedEventArgs e)
        {
            _mainVm.IsSortConfigurationChanged = true;
        }

        private async void AttackComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            if (cb != null)
            {
                if (cb.Tag.Equals("from"))
                {
                    _mainVm.SelectedAttackFromOption = (int) e.AddedItems[0];
                }
                else if (cb.Tag.Equals("to"))
                {
                    _mainVm.SelectedAttackToOption = (int) e.AddedItems[0];
                }
                await _mainVm.OnQueryChangedAsync();
            }
        }
    }
}
