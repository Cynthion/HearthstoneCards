using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
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
                await Messaging.ShowMessage(string.Format("{0} received!\n Thank you very much for your contribution.",  purchaseItem), "Thank you!");
            }

            // store purchase items
            await Storage.StorePurchasesAsync(_donationAmounts);
        }

        public void HandleRating()
        {

        }

        public Task HandleFeedbackAsync()
        {
            return SendEmailAsync("Feedback", Variables.FeedbackEmailBody);
        }

        public void HandleUserVoice()
        {

        }

        public void HandleTwitter()
        {

        }

        public Task HandleBugReportAsync()
        {
            return SendEmailAsync("Bug Report", Variables.BugReportEmailBody);
        }

        private async Task SendEmailAsync(string subject, string body)
        {
            var mail = new EmailMessage
            {
                Subject = string.Format("[{0}] {1}", Variables.AppNameShort, subject),
                Body = body
            };
            mail.To.Add(Variables.FeedbackEmailRecipient);
            await EmailManager.ShowComposeNewEmailAsync(mail);
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

        public IList<ReleaseNotes> ReleaseNotes
        {
            get { return Model.ReleaseNotes.GetReleaseNotes(); }
        }
    }
}
