using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using WPDevToolkit;

namespace HearthstoneCards.Helper
{
    public static class WindowsStoreManager
    {
        private static readonly LicenseInformation LicenseInformation;

        static WindowsStoreManager()
        {
#if DEBUG
            LicenseInformation = CurrentAppSimulator.LicenseInformation;
#else
            _licenseInformation = CurrentApp.LicenseInformation; 
#endif
            LicenseInformation.LicenseChanged += LicenseInformation_OnLicenseChanged;   
        }

        private static void LicenseInformation_OnLicenseChanged()
        {
            // TODO test for all features/consumables
            if (LicenseInformation.ProductLicenses["Your Product ID"].IsActive)
            {
                // add code for purchased (e.g. property of a data class derived from INotifyPropertyChanged)
            }
            else
            {
                // add code for not yet purchased (e.g. property of a data class derived from INotifyPropertyChanged)
            }
        }

        public static async Task<bool> BuyFeatureAsync(string featureId)
        {
            if (!LicenseInformation.ProductLicenses[featureId].IsActive)
            {
                ExceptionDispatchInfo exception = null;
                try
                {
#if DEBUG
                    await ReloadCurrentAppSimulatorAsync();

                    var purchaseResult = await CurrentAppSimulator.RequestProductPurchaseAsync(featureId);
#else
                    var purchaseResult = await CurrentApp.RequestProductPurchaseAsync(featureId);
#endif
                    if (purchaseResult.Status == ProductPurchaseStatus.Succeeded)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    exception = ExceptionDispatchInfo.Capture(ex);
                }
                if (exception != null)
                {
                    await Messaging.ShowMessage("Something went wrong during the transaction. Please try again.", "Purchase unsucessful.");
                    return false;
                }
            }
            return false;
        }

        private static async Task ReloadCurrentAppSimulatorAsync()
        {
            var wspXml = await GetSimulatedWindowsStoreProxyAsync();
            await CurrentAppSimulator.ReloadSimulatorAsync(wspXml);
        }

        private static Task<StorageFile> GetSimulatedWindowsStoreProxyAsync()
        {
            return Package.Current.InstalledLocation.GetFileAsync(@"Assets\StoreSimulation\WindowsStoreProxy.xml").AsTask();
        }

        public static bool IsActive
        {
            get { return LicenseInformation.IsActive; }
        }

        public static bool IsTrial
        {
            get { return LicenseInformation.IsTrial; }
        }
    }
}
