namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationItemSaveResult : OperationResult
    {
        public PersonalizationItemSaveResult() { }

        public PersonalizationItemSaveResult(string error)
        {
            Error = error;
        }
    }
}