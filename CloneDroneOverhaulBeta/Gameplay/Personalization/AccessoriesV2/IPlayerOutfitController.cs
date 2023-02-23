using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public interface IPlayerOutfitController
    {
        IWeaponSkinItemDefinition NewAccessoryItem(MechBodyPartType partType, string itemName, ItemFilter filter);

        IWeaponSkinItemDefinition GetAccessoryItem(string skinName, out ItemNullResult error);

        IWeaponSkinItemDefinition[] GetAccessoryItems(ItemFilter filter);

        IWeaponSkinItemDefinition[] GetAccessoryItems(FirstPersonMover firstPersonMover);
    }
}
