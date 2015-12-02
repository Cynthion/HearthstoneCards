using Windows.ApplicationModel.Email;
using WPDevToolkit;

namespace HearthstoneCards.Helper
{
    public class Variables : BaseNotifyPropertyChanged
    {
        public static string AppName { get; private set; }
        public static string AppNameShort { get; private set; }
        public static string TwitterHashtag { get; private set; }
        public static string FeedbackEmailBody { get; private set; }
        public static EmailRecipient FeedbackEmailRecipient { get; private set; }
        public static string BugReportEmailBody { get; private set; }
        public static EmailRecipient BugReportEmailRecipient { get; private set; }

        private static int _minAttack;
        private static int _maxAttack;

        private static int _minCost;
        private static int _maxCost;

        public Variables()
        {
            AppName = "Hearthstone Cards";
            AppNameShort = "HS Cards";
            TwitterHashtag = "#hs-cards";
            FeedbackEmailBody = string.Format("Hey Chris,\n\nI wanted to provide some feedback about the '{0}' app:\n\n", AppName);
            FeedbackEmailRecipient = new EmailRecipient("chris.windev@outlook.com", "Christian Lüthold");
            BugReportEmailBody = string.Format("Hey Chris,\n\nI wanted to let you know that I found a bug in the '{0}' app:\n\n", AppName);
            BugReportEmailRecipient = FeedbackEmailRecipient;
        }

        public int MinAttack
        {
            get { return _minAttack; }
            set
            {
                if (_minAttack != value)
                {
                    _minAttack = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int MaxAttack
        {
            get { return _maxAttack; }
            set
            {
                if (_maxAttack != value)
                {
                    _maxAttack = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int MinCost
        {
            get { return _minCost; }
            set
            {
                if (_minCost != value)
                {
                    _minCost = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int MaxCost
        {
            get { return _maxCost; }
            set
            {
                if (_maxCost != value)
                {
                    _maxCost = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
