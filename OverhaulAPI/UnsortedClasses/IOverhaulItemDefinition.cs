namespace OverhaulAPI
{
    public interface IOverhaulItemDefinition
    {
        string ItemName();

        bool IsUnlocked(bool forceTrue);
    }
}
