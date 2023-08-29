using CDOverhaul.Gameplay.Pets;

namespace CDOverhaul.HUD
{
    public class MountItemDisplay : OverhaulPersonalizationItemDisplay
    {
        public override string GetEquippedItemsString() => PetSystem.EquippedPets;

        public override void OnClicked()
        {
            ButtonComponent.OnDeselect(null);

            PetSystem.SetPetEquipped(Item, !IsSelected, true);
            RefreshDisplay();
        }
    }
}
