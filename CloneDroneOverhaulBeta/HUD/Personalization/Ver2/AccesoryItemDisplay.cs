using CDOverhaul.Gameplay.Outfits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.HUD
{
    public class AccesoryItemDisplay : OverhaulPersonalizationItemDisplay
    {
        public OutfitItem Item
        {
            get;
            private set;
        }

        private bool m_IsExclusive;
        public bool IsExclusive
        {
            get => m_IsExclusive;
        }

        private bool m_IsSelected;
        public bool IsSelected
        {
            get => m_IsSelected;
        }

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
            if(Item == null)
            {
                DestroyGameObject();
                return;
            }

            m_IsExclusive = !string.IsNullOrEmpty(Item.ExclusiveFor);
            m_IsSelected = OutfitsController.EquippedAccessories.Contains(Item.Name);

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

            OutfitsController.SetAccessoryEquipped(Item, !m_IsSelected, true);
            // Todo: lock UI until player's accessories are refreshed
            RefreshDisplay();
        }
    }
}
