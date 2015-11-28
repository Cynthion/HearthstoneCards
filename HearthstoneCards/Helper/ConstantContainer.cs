using Windows.ApplicationModel.Email;

namespace HearthstoneCards.Helper
{
    public class ConstantContainer
    {
        public static string AppName { get; private set; }
        public static string AppNameShort { get; private set; }
        public static string TwitterHashtag { get; private set; }
        public static string FeedbackEmailBody { get; private set; }
        public static EmailRecipient FeedbackEmailRecipient { get; private set; }
        public static string BugReportEmailBody { get; private set; }
        public static EmailRecipient BugReportEmailRecipient { get; private set; }

        public static int MinAttack { get; private set; }
        public static int MaxAttack { get; private set; }

        public static string IsCommandBarVisibleTag { get; private set; }

        // static constructor
        static ConstantContainer()
        {
            Initialize();
        }

        public ConstantContainer()
        {
            Initialize();
        }

        private static void Initialize()
        {
            AppName = "Hearthstone Cards";
            AppNameShort = "HS Cards";
            TwitterHashtag = "#hs-cards";
            FeedbackEmailBody = string.Format("Hey Chris,\n\nI wanted to provide some feedback about the '{0}' app:\n\n", AppName);
            FeedbackEmailRecipient = new EmailRecipient("chris.windev@outlook.com", "Christian Lüthold");
            BugReportEmailBody = string.Format("Hey Chris,\n\nI wanted to let you know that I found a bug in the '{0}' app:\n\n", AppName);
            BugReportEmailRecipient = FeedbackEmailRecipient;

            MinAttack = 0;
            MaxAttack = 30;

            IsCommandBarVisibleTag = "IsCommandBarVisible";
        }
    }
}
