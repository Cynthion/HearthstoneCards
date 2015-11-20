using System;
using WPDevToolkit;

namespace HearthstoneCards.Helper
{
    public static class Initializer
    {
        public static void Initialize()
        {
            var settings = new AppSettings();

            // Note: initialize as soon as a new setting is added
            if (settings.IsFirstRun || IsNewerAppVersion(settings))
            {
                settings.AppVersion = PhoneInteraction.GetAppVersion();
                settings.ItemsControlViewInfoIndex = 0;
                settings.IsSortedAscending = true;
                settings.IsAttackFilterEnabled = true;
                settings.SortOptionSelection = new[] { true, false, false };
                // 9 classes, 1 neutral
                settings.ClassSelection = new [] { true, true, true, true, true, true, true, true, true, true };
                // 7 sets
                settings.SetSelection = new[] { true, true, true, true, true, true, true };
                settings.RaritySelection = new [] { true, true, true, true, true };
                
                settings.IsFirstRun = false;
            }
        }

        private static bool IsNewerAppVersion(BaseSettings settings)
        {
            var currentVersion = PhoneInteraction.GetAppVersion();
            var storedVersion = settings.AppVersion;

            return string.Compare(currentVersion, storedVersion, StringComparison.Ordinal) > 0;
        }
    }
}
