using CDOverhaul.Gameplay.Pets;

namespace CDOverhaul.HUD
{
    public class MountItemDisplay : OverhaulPersonalizationItemDisplay
    {
        public override string GetEquippedItemsString() => PetsController.EquippedPets;

        public override void OnClicked()
        {
            ButtonComponent.OnDeselect(null);

            PetsController.SetPetEquipped(Item, !IsSelected, true);
            RefreshDisplay();
        }
    }
}
