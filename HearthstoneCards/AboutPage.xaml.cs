using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using HearthstoneCards.ViewModel;
using WPDevToolkit;

namespace HearthstoneCards
{
    public sealed partial class AboutPage : BasePage
    {
        private readonly AboutViewModel _aboutVm;

        public AboutPage()
        {
            this.InitializeComponent();

            // set data context
            _aboutVm = SingletonLocator.Get<AboutViewModel>();
            DataContext = _aboutVm;
        }

        private async void SupportListViewItem_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var lvi = sender as ListViewItem;
            if (lvi == null) return;

            switch ((string)lvi.Tag)
            {
                case "donation":
                    await _aboutVm.HandleDonationAsync();
                    break;
                case "rating":
                    _aboutVm.HandleRating();
                    break;
                case "feedback":
                    _aboutVm.HandleFeedback();
                    break;
                case "uservoice":
                    _aboutVm.HandleUserVoice();
                    break;
                case "twitter":
                    _aboutVm.HandleTwitter();
                    break;
                case "bugreport":
                    _aboutVm.HandleBugReport();
                    break;
                default:
                    return;
                    break;
            }
        }
    }
}
