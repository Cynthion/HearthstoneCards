using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearthstoneCards.Helper
{
    public static class Initializer
    {
        public static void Initialize()
        {
            var settings = new AppSettings();
            if (settings.IsFirstRun)
            {
                settings.ClassSelection = new [] { true, true, true, true, true, true, true, true, true };
                settings.SetSelection = new[] { true, true, true, true, true, true, true, true, true, true, true, true, true, true };
                settings.RaritySelection = new [] { true, true, true, true, true };
            }
            settings.IsFirstRun = false;
        }
    }
}
