﻿using Windows.UI.Xaml;
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
            this.NavigationCacheMode = NavigationCacheMode.Required;

            // initialize app
            Initializer.Initialize();

            // set data context
            _mainVm = SingletonLocator.Get<MainViewModel>();
            DataContext = _mainVm;

            Loaded += MainPage_OnLoaded;
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
    }
}
