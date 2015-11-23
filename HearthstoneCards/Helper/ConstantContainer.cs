namespace HearthstoneCards.Helper
{
    public class ConstantContainer
    {
        public static string AppName { get; private set; }
        public static string TwitterHashtag { get; private set; }

        public static int MinAttack { get; private set; }
        public static int MaxAttack { get; private set; }

        public static string IsCommandBarVisibleTag { get; private set; }

        // static constructor
        static ConstantContainer()
        {
            AppName = "Hearthstone Cards";
            TwitterHashtag = "#hs-cards";

            MinAttack = 0;
            MaxAttack = 30;

            IsCommandBarVisibleTag = "IsCommandBarVisible";
        }
    }
}
