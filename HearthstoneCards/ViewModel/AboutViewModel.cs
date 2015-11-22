using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HearthstoneCards.Helper;
using HearthstoneCards.Model;
using WPDevToolkit;
using WPDevToolkit.Interfaces;

namespace HearthstoneCards.ViewModel
{
    public class AboutViewModel : AsyncLoader, ILocatable
    {
        public string Version { get; private set; }

        private IList<PurchaseItem> _donationAmounts;

        public AboutViewModel()
        {
            Version = PhoneInteraction.GetAppVersion();
        }

        protected override async Task<LoadResult> DoLoadAsync()
        {
            DonationAmounts = await Storage.LoadPurchasesAsync();
            return LoadResult.Success;
        }

        public async Task BuyDonationAsync(PurchaseItem purchaseItem)
        {
            var success = await WindowsStoreManager.BuyFeatureAsync(purchaseItem.Id);
            if (success)
            {
                purchaseItem.IsPurchased = true;
                await Messaging.ShowMessage(string.Format("{0} received! Thank you very much for your contribution.",  purchaseItem.Price), "Thank you!");
            }

            // store purchase items
            await Storage.StorePurchasesAsync(_donationAmounts);
        }

        public void HandleRating()
        {

        }

        public void HandleFeedback()
        {

        }

        public void HandleUserVoice()
        {

        }

        public void HandleTwitter()
        {

        }

        public void HandleBugReport()
        {

        }

        public IList<PurchaseItem> DonationAmounts
        {
            get { return _donationAmounts; }
            private set
            {
                if (_donationAmounts != value)
                {
                    _donationAmounts = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
