namespace OverhaulAPI
{
    public interface IOverhaulItemDefinition
    {
        void SetItemName(string newName);
        string GetItemName();

        void SetExclusivePlayerID(string id);
        string GetExclusivePlayerID();

        bool IsUnlocked(bool forceTrue);
    }
}
