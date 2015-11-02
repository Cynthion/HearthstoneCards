using WPDevToolkit;

namespace HearthstoneCards.Helper
{
    public class AppSettings : BaseSettings
    {
        // settings
        public const string IsSortedAscendingKey = "sortDirection";
        public const string SortOptionsSelectionKey = "sortOptionSelection";
        public const string ClassSelectionKey = "classSelection";
        public const string SetSelectionKey = "setSelection";
        public const string RaritySelectionKey = "raritySelection";

        public bool IsSortedAscending
        {
            get { return Load<bool>(IsSortedAscendingKey); }
            set { Store(IsSortedAscendingKey, value); }
        }
        
        public bool[] SortOptionSelection
        {
            get { return Load<bool[]>(SortOptionsSelectionKey); }
            set { Store(SortOptionsSelectionKey, value); }
        }

        public bool[] ClassSelection
        {
            get { return Load<bool[]>(ClassSelectionKey); }
            set { Store(ClassSelectionKey, value); }
        }

        public bool[] SetSelection
        {
            get { return Load<bool[]>(SetSelectionKey); }
            set { Store(SetSelectionKey, value); }
        }

        public bool[] RaritySelection
        {
            get { return Load<bool[]>(RaritySelectionKey); }
            set { Store(RaritySelectionKey, value); }
        }
    }
}
