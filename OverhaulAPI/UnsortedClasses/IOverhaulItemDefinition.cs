namespace OverhaulAPI
{
    public interface IOverhaulItemDefinition
    {
        void SetItemName(string newName);

        string GetItemName();

        bool IsUnlocked(bool forceTrue);
    }
}
