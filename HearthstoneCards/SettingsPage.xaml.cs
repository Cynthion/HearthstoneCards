using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace HearthstoneCards
{
    public sealed partial class SettingsPage : BasePage
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void RefreshDbButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
