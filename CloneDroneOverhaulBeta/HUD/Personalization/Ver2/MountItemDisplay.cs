using CDOverhaul.Gameplay.Outfits;
using CDOverhaul.Gameplay.Pets;

namespace CDOverhaul.HUD
{
    public class MountItemDisplay : OverhaulPersonalizationItemDisplay
    {
        public PetItem Item
        {
            get;
            private set;
        }
        public bool IsExclusive { get; private set; }
        public bool IsSelected { get; private set; }

        public override void Start()
        {
            base.Start();
            ButtonComponent.AddOnClickListener(onClicked);

            RefreshDisplay();
        }

        public void Initialize(PetItem outfitItem)
        {
            Item = outfitItem;
        }

        public void RefreshDisplay()
        {
            if (Item == null)
            {
                DestroyGameObject();
                return;
            }

            IsExclusive = !string.IsNullOrEmpty(Item.ExclusiveFor);
            IsSelected = PetsController.EquippedPets.Contains(Item.Name);

            ItemNameLabel.text = Item.Name;
            AuthorLabel.text = Item.Author;

            AnimationComponent.enabled = IsExclusive;
            ButtonComponent.interactable = !IsExclusive;

            ExclusiveIndicator.SetActive(IsExclusive);
            SelectedIndicator.SetActive(IsSelected);
        }

        private void onClicked()
        {
            ButtonComponent.OnDeselect(null);

            PetsController.SetPetEquipped(Item, !IsSelected, true);
            RefreshDisplay();
        }
    }
}
