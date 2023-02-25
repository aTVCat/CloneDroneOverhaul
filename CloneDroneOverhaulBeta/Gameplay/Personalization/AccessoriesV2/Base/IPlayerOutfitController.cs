namespace CDOverhaul.Gameplay
{
    public interface IPlayerOutfitController
    {
        IPlayerAccessoryItemDefinition NewAccessoryItem(MechBodyPartType partType, string itemName, ItemFilter filter);

        IPlayerAccessoryItemDefinition GetAccessoryItem(string skinName, out ItemNullResult error);

        IPlayerAccessoryItemDefinition[] GetAccessoryItems(ItemFilter filter);

        IPlayerAccessoryItemDefinition[] GetAccessoryItems(FirstPersonMover firstPersonMover);
    }
}
