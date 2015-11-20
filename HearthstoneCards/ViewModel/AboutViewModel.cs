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
    }
}
