namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationItemCreationResult : OperationResult
    {
        public PersonalizationItemInfo NewItem;

        public PersonalizationItemCreationResult() { }

        public PersonalizationItemCreationResult(PersonalizationItemInfo item)
        {
            NewItem = item;
        }

        public PersonalizationItemCreationResult(string error)
        {
            Error = error;
        }
    }
}