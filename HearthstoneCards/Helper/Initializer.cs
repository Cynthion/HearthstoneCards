using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HearthstoneCards.Model;
using WPDevToolkit;

namespace HearthstoneCards.Helper
{
    public static class Initializer
    {
        public static async Task InitializeAsync()
        {
            var settings = new AppSettings();

            // Note: initialize as soon as a new setting is added
            if (settings.IsFirstRun || IsNewerAppVersion(settings))
            {
                settings.AppVersion = PhoneInteraction.GetAppVersion();
                settings.ItemsControlViewInfoIndex = 0;
                settings.IsSortedAscending = true;
                settings.IsAttackFilterEnabled = false;
                settings.IsCostFilterEnabled = false;
                settings.SortOptionSelection = new[] { true, false, false };
                // 9 classes, 1 neutral
                settings.ClassSelection = new [] { true, true, true, true, true, true, true, true, true, true };
                // 7 sets
                settings.SetSelection = new[] { true, true, true, true, true, true, true };
                // 5 rarities
                settings.RaritySelection = new [] { true, true, true, true, true };
                // 24 mechanics
                settings.MechanicsSelection = new[]
                {
                    true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true,
                    true, true, true, true, true, true, true
                };
                settings.IsAnyMechanismChecked = true;

                settings.IsFirstRun = false;

                await AddPurchasesAsync();
            }
        }

        private static bool IsNewerAppVersion(BaseSettings settings)
        {
            var currentVersion = PhoneInteraction.GetAppVersion();
            var storedVersion = settings.AppVersion;

            return string.Compare(currentVersion, storedVersion, StringComparison.Ordinal) > 0;
        }

        private static async Task AddPurchasesAsync()
        {
            var purchases = new List<PurchaseItem>
            {
                new PurchaseItem("donation1", 1.00),
                new PurchaseItem("donation2", 2.00),
                new PurchaseItem("donation3", 3.00),
                new PurchaseItem("donation4", 4.00),
                new PurchaseItem("donation5", 5.00)
            };
            await Storage.StorePurchasesAsync(purchases);
        }
    }
}
