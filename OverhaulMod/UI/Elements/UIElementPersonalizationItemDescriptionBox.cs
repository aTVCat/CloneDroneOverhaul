using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationItemDescriptionBox : OverhaulUIBehaviour
    {
        [UIElement("ItemName")]
        private Text m_itemNameText;

        [UIElement("ItemDescription")]
        private Text m_itemDescriptionText;

        [UIElement("EquipButton")]
        private GameObject m_unequippedIndicatorObject;

        [UIElement("EquippedLabel")]
        private GameObject m_equippedIndicatorObject;

        [UIElement("LockedOverlay")]
        private GameObject m_lockedOverlay;

        [UIElementAction(nameof(OnEquipButtonClicked))]
        [UIElement("EquipButton")]
        private Button m_equipButton;

        private PersonalizationItemInfo m_selectedItemInfo;

        private UIElementMouseEventsComponent m_mouseEvents;

        private UIPersonalizationItemsBrowser m_browser;

        private Animator m_animator;

        protected override void OnInitialized()
        {
            m_mouseEvents = base.gameObject.AddComponent<UIElementMouseEventsComponent>();
            m_animator = base.GetComponent<Animator>();
        }

        public override void Update()
        {
            if(Input.GetMouseButtonDown(0) && !m_mouseEvents.isMouseOverElement && !m_browser.IsMouseOverPanel())
            {
                Hide();
            }    
        }

        public void SetBrowserUI(UIPersonalizationItemsBrowser personalizationItemsBrowser)
        {
            m_browser = personalizationItemsBrowser;
        }

        public void ShowForItem(PersonalizationItemInfo itemInfo, RectTransform rectTransform)
        {
            base.Show();
            if (m_selectedItemInfo == itemInfo)
                return;

            m_animator.Play(string.Empty);
            m_selectedItemInfo = itemInfo;

            m_itemNameText.text = itemInfo.Name;
            m_itemDescriptionText.text = itemInfo.Description;

            bool equipped = itemInfo.IsEquipped();
            m_unequippedIndicatorObject.SetActive(!equipped);
            m_equippedIndicatorObject.SetActive(equipped);

            bool isLocked = !itemInfo.IsUnlocked();
            m_lockedOverlay.SetActive(isLocked);
            m_equipButton.interactable = !isLocked;

            Transform transform = base.transform;
            Vector3 vector = transform.position;
            vector.y = rectTransform.position.y;
            transform.position = vector;
        }

        public void OnEquipButtonClicked()
        {
            PersonalizationItemInfo item = m_selectedItemInfo;
            if (item == null || !item.IsUnlocked())
                return;

            if (!item.IsCompatibleWithMod())
            {
                ModUIUtils.MessagePopupOK("Incompatible item!", $"This item is made for the new version of Overhaul mod ({item.MinModVersion}).\nMake sure you're using the latest version of the mod.", 175f, true);
                return;
            }

            if(item.Category == PersonalizationCategory.WeaponSkins)
            {
                Character character = CharacterTracker.Instance.GetPlayer();
                if (character)
                {
                    PersonalizationController personalizationController = character.GetComponent<PersonalizationController>();
                    if (personalizationController)
                    {
                        personalizationController.EquipItem(item);
                    }
                }
            }
            else
            {
                throw new NotImplementedException($"DescriptionBox: Add support for {item.Category}");
            }

            m_unequippedIndicatorObject.SetActive(false);
            m_equippedIndicatorObject.SetActive(true);
        }
    }
}
