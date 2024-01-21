using CDOverhaul.Gameplay.Outfits;

namespace CDOverhaul.HUD
{
    public class AccesoryItemDisplay : OverhaulPersonalizationItemDisplay
    {
        public override string GetEquippedItemsString() => OutfitsController.EquippedAccessories;

        public override void OnClicked()
        {
            ButtonComponent.OnDeselect(null);

            OutfitsController.SetAccessoryEquipped(Item, !IsSelected, true);
            // Todo: lock UI until player's accessories are refreshed
            RefreshDisplay();
        }
    }
}
