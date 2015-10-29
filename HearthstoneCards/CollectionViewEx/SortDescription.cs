namespace HearthstoneCards.CollectionViewEx
{
    public enum SortDirection
    {
        Ascending = 0,
        Descending = 1
    }

    public class SortDescription
    {
        public string PropertyName { get; set; }
        public SortDirection Direction { get; set; }

        public SortDescription(string propertyName, SortDirection direction)
        {
            PropertyName = propertyName;
            Direction = direction;
        }
    }
}
