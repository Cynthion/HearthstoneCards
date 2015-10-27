using WPDevToolkit;

namespace HearthstoneCards.Helper
{
    public class AppSettings : BaseSettings
    {
        // settings
        private const string ClassSelectionKey = "classSelection";
        private const string SetSelectionKey = "setSelection";
        private const string RaritySelectionKey = "raritySelection";

        public bool[] ClassSelection
        {
            get { return Get<bool[]>(ClassSelectionKey); }
            set { Set(ClassSelectionKey, value); }
        }

        public bool[] SetSelection
        {
            get { return Get<bool[]>(SetSelectionKey); }
            set { Set(SetSelectionKey, value); }
        }

        public bool[] RaritySelection
        {
            get { return Get<bool[]>(RaritySelectionKey); }
            set { Set(RaritySelectionKey, value); }
        }
    }
}
