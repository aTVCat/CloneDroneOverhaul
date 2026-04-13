namespace OverhaulMod.Content.Personalization
{
    public struct PersonalizationItemCreationArgs
    {
        public bool UsePersistentFolder;

        public string DirectoryName;

        public string ItemName;

        public string UniqueID;

        public PersonalizationCategory ItemCategory;

        public PersonalizationItemInfo Template;
    }
}
