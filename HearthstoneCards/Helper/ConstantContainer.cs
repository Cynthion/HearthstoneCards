namespace HearthstoneCards.Helper
{
    public class ConstantContainer
    {
        public string AppName { get; private set; }
        public string TwitterHashtag { get; private set; }

        public int MinAttack { get; private set; }
        public int MaxAttack { get; private set; }

        public ConstantContainer()
        {
            AppName = "Hearthstone Cards";
            TwitterHashtag = "#hs-cards";

            MinAttack = 0;
            MaxAttack = 30;
        }
    }
}
