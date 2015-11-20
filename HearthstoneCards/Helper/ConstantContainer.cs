namespace HearthstoneCards.Helper
{
    public class ConstantContainer
    {
        public int MinAttack { get; private set; }
        public int MaxAttack { get; private set; }

        public ConstantContainer()
        {
            MinAttack = 0;
            MaxAttack = 30;
        }
    }
}
