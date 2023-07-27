using CDOverhaul.Gameplay.Outfits;

namespace CDOverhaul.HUD
{
    public class AccesoryItemDisplay : OverhaulPersonalizationItemDisplay
    {
        public OutfitItem Item
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

        public void Initialize(OutfitItem outfitItem)
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
            IsSelected = OutfitsController.EquippedAccessories.Contains(Item.Name);

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

            OutfitsController.SetAccessoryEquipped(Item, !IsSelected, true);
            // Todo: lock UI until player's accessories are refreshed
            RefreshDisplay();
        }
    }
}
