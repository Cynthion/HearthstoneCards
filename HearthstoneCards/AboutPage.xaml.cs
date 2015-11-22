using Windows.UI.Xaml.Controls;
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

        private async void SupportListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = sender as ListView;
            if (lv == null) return;

            var lvi = lv.SelectedItem as ListViewItem;
            if (lvi == null) return;

            switch ((string) lvi.Tag)
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
