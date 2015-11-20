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
        }
    }
}
