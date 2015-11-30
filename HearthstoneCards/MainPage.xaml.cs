using System.Linq;
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

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            // set data context
            _mainVm = SingletonLocator.Get<MainViewModel>();
            DataContext = _mainVm;

            // set initial items panel template (from settings)
            var iptIndex = new AppSettings().ItemsControlViewInfoIndex;
            SetItemsPanelTemplate(iptIndex);

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

        private void SetItemsPanelTemplate(int index)
        {
            if (index == 1)
            {
                _mainVm.ItemsControlViewInfo = Application.Current.Resources["ImagesViewInfo"] as ItemsControlViewInfo;
            }
            else if (index == 2)
            {
                _mainVm.ItemsControlViewInfo = Application.Current.Resources["TableViewInfo"] as ItemsControlViewInfo;
            }
            // default
            else
            {
                _mainVm.ItemsControlViewInfo = Application.Current.Resources["VisualsListViewInfo"] as ItemsControlViewInfo;
            }
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

        private void ItemsPanelTemplateButton_OnClick(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private async void TextFilterTextBox_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null)
            {
                var txt = tb.Text;
                _mainVm.TextFilter = txt;
                _mainVm.IsTextFilterEnabled = !(string.IsNullOrEmpty(txt) || string.IsNullOrWhiteSpace(txt));
                await _mainVm.OnQueryChangedAsync();
            }
        }

        private void TextBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null)
            {
                tb.ActionOnEnter();
            }
        }

        private void AboutAppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (AboutPage));
        }

        //private void Hub_OnSectionsInViewChanged(object sender, SectionsInViewChangedEventArgs e)
        //{
        //    var hub = sender as Hub;
        //    if (hub != null && BottomAppBar != null)
        //    {
        //        var isCommandBarVisible = hub.SectionsInView.Any(s => s.Tag != null && s.Tag.Equals(Variables.IsCommandBarVisibleTag));
        //        BottomAppBar.Visibility = isCommandBarVisible ? Visibility.Visible : Visibility.Collapsed;
        //    }
        //}

        private async void AttackRangeBox_OnIsCheckedChanged(object sender, CheckedEventArgs e)
        {
            await _mainVm.OnQueryChangedAsync();
        }

        private async void AttackRangeBox_OnRangeValueChanged(RangeBox sender, RangeBoxEventArgs args)
        {
            await _mainVm.OnQueryChangedAsync();
        }
    }
}
