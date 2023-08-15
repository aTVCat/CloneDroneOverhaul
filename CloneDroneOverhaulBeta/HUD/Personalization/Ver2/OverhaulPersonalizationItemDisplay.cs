using CDOverhaul.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulPersonalizationItemDisplay : OverhaulBehaviour
    {
        [ObjectReference("OutfitItemEntry")]
        protected Button ButtonComponent;
        [ObjectReference("OutfitItemEntry")]
        protected Animation AnimationComponent;

        [ObjectReference("Name")]
        protected Text ItemNameLabel;
        [ObjectReference("Author")]
        protected InputField AuthorLabel;

        [ObjectReference("ExclusivityIndicator")]
        protected GameObject ExclusiveIndicator;
        [ObjectReference("Selected")]
        protected GameObject SelectedIndicator;

        public PersonalizationItem Item
        {
            get;
            private set;
        }

        public bool IsExclusive { get; protected set; }
        public bool IsSelected { get; protected set; }

        public override void Start()
        {
            OverhaulUIVer2.AssignValues(this);
            ButtonComponent.AddOnClickListener(OnClicked);

            RefreshDisplay();
        }

        public virtual void Initialize(PersonalizationItem item)
        {
            Item = item;
        }

        public virtual string GetEquippedItemsString() => string.Empty;

        public virtual void RefreshDisplay()
        {
            if (Item == null)
            {
                DestroyGameObject();
                return;
            }

            IsExclusive = !Item.ExclusiveFor.IsNullOrEmpty();
            IsSelected = GetEquippedItemsString().Contains(Item.GetID());

            ItemNameLabel.text = Item.Name;
            AuthorLabel.text = Item.Author;

            AnimationComponent.enabled = IsExclusive;
            ButtonComponent.interactable = Item.IsUnlocked();

            ExclusiveIndicator.SetActive(IsExclusive);
            SelectedIndicator.SetActive(IsSelected);
        }

        public virtual void OnClicked()
        {
        }
    }
}
