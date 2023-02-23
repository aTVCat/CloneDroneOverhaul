using System.Collections;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class PlayerOutfitController : ModController, IPlayerOutfitController
    {
        IWeaponSkinItemDefinition IPlayerOutfitController.GetAccessoryItem(string skinName, out ItemNullResult error)
        {
            throw new System.NotImplementedException();
        }

        IWeaponSkinItemDefinition[] IPlayerOutfitController.GetAccessoryItems(ItemFilter filter)
        {
            throw new System.NotImplementedException();
        }

        IWeaponSkinItemDefinition[] IPlayerOutfitController.GetAccessoryItems(FirstPersonMover firstPersonMover)
        {
            throw new System.NotImplementedException();
        }

        IWeaponSkinItemDefinition IPlayerOutfitController.NewAccessoryItem(MechBodyPartType partType, string itemName, ItemFilter filter)
        {
            throw new System.NotImplementedException();
        }
    }
}