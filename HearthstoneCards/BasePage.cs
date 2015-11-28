using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using HearthstoneCards.Common;

namespace HearthstoneCards
{
    public abstract class BasePage : Page
    {
        private readonly NavigationHelper _navigationHelper;

        protected BasePage()
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelper_LoadState;
            _navigationHelper.SaveState += NavigationHelper_SaveState;
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {

        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        protected void TextBox_SelectAll_OnLGotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null)
            {
                tb.SelectAll();
            }
        }
    }
}
