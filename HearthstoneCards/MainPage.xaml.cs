using System;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
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
        private readonly ItemsPanelTemplate[] _itemsPanelTemplates;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            // initialize app
            Initializer.Initialize(); // TODO move to App.cs?
            
            // set data context
            _mainVm = SingletonLocator.Get<MainViewModel>();
            DataContext = _mainVm;

            // provide items panel templates
            _itemsPanelTemplates = new ItemsPanelTemplate[2];
            _itemsPanelTemplates[0] = Application.Current.Resources["StackPanelItemsPanelTemplate"] as ItemsPanelTemplate;
            _itemsPanelTemplates[1] = Application.Current.Resources["WrapPanelItemsPanelTemplate"] as ItemsPanelTemplate;
            
            // set initial items panel template (from settings)
            var iptIndex = new AppSettings().ItemsPanelTemplateIndex;
            _mainVm.SelectedItemsPanelTemplate = _itemsPanelTemplates[iptIndex];

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

        private async void AttackTextBox_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null)
            {
                if (tb.Tag.Equals("from"))
                {
                    _mainVm.SelectedAttackFromOption = Convert.ToInt32(tb.Text);
                }
                else if (tb.Tag.Equals("to"))
                {
                    _mainVm.SelectedAttackToOption = Convert.ToInt32(tb.Text);
                }
                await _mainVm.OnQueryChangedAsync();
            }
        }

        private void ItemsPanelTemplateMenuFlyoutItem_OnClick(object sender, RoutedEventArgs e)
        {
            var mfi = sender as MenuFlyoutItem;
            if (mfi != null)
            {
                switch (mfi.Text)
                {
                    case "list":
                        _mainVm.SelectedItemsPanelTemplate = _itemsPanelTemplates[0];
                        break;
                    case "wrap":
                        _mainVm.SelectedItemsPanelTemplate = _itemsPanelTemplates[1];
                        break;
                    default:
                        _mainVm.SelectedItemsPanelTemplate = _itemsPanelTemplates[0];
                        break;
                }
            }
        }

        private void ItemsPanelTemplateButton_OnClick(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
    }
}
