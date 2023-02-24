namespace CDOverhaul.Gameplay
{
    public class PlayerOutfitController : OverhaulController, IPlayerOutfitController
    {
        public override void Initialize()
        {

        }

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

        public override string[] Commands()
        {
            throw new System.NotImplementedException();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new System.NotImplementedException();
        }
    }
}