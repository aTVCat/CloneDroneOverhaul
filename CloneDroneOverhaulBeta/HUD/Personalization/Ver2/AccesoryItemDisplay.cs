using CDOverhaul.Gameplay.Outfits;

namespace CDOverhaul.HUD
{
    public class AccesoryItemDisplay : OverhaulPersonalizationItemDisplay
    {
        public override string GetEquippedItemsString() => OutfitsSystem.EquippedAccessories;

        public override void OnClicked()
        {
            ButtonComponent.OnDeselect(null);

            OutfitsSystem.SetAccessoryEquipped(Item, !IsSelected, true);
            // Todo: lock UI until player's accessories are refreshed
            RefreshDisplay();
        }
    }
}
