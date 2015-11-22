using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using HearthstoneCards.Helper;
using WPDevToolkit;
using WPDevToolkit.Interfaces;
using WPDevToolkit.ViewModel;

namespace HearthstoneCards.ViewModel
{
    public class AboutViewModel : BaseViewModel, ILocatable
    {
        public string Version { get; private set; }

        public AboutViewModel()
        {
            Version = PhoneInteraction.GetAppVersion();
        }

        public async Task HandleDonationAsync()
        {
            var purchased = await WindowsStoreManager.RequestFeatureAsync("donation1");
            if (purchased)
            {
                await Messaging.ShowMessage("Thank you very much for your contribution.", "Thank you!");
            }
        }

        public void HandleRating()
        {
            throw new System.NotImplementedException();
        }

        public void HandleFeedback()
        {
            throw new System.NotImplementedException();
        }

        public void HandleUserVoice()
        {
            throw new System.NotImplementedException();
        }

        public void HandleTwitter()
        {
            throw new System.NotImplementedException();
        }

        public void HandleBugReport()
        {
            throw new System.NotImplementedException();
        }
    }
}
