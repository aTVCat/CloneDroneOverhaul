using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.AppDomainInitializerInfo;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationItemDisplay : OverhaulUIBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public const string ITEM_UNEQUIPPED_BG_COLOR = "#404040";
        public const string ITEM_EQUIPPED_BG_COLOR = "#305EE0";

        public const string ITEM_UNEQUIPPED_NAME_BG_COLOR = "#272727";
        public const string ITEM_EQUIPPED_NAME_BG_COLOR = "#10204F";

        [UIElement("NameBG")]
        private Image m_nameBg;

        [UIElement("Frame")]
        private Image m_frame;

        [UIElement("NewIndicator")]
        private GameObject m_newIndicator;

        [UIElement("FavoriteIndicator")]
        private GameObject m_favoriteIndicator;

        private Button m_button;

        private Image m_bg;

        private UIPersonalizationItemsBrowser m_browser;

        private RectTransform m_rectTransform;

        public PersonalizationItemInfo ItemInfo;

        private float m_timeForDoubleClick;

        protected override void OnInitialized()
        {
            m_rectTransform = base.GetComponent<RectTransform>();
            m_bg = base.GetComponent<Image>();
            m_button = base.GetComponent<Button>();
            m_button.onClick.AddListener(onClicked);

            GlobalEventManager.Instance.AddEventListener(PersonalizationManager.ITEM_EQUIPPED_OR_UNEQUIPPED, RefreshDisplays);
            RefreshDisplays();

            m_timeForDoubleClick = -1f;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            GlobalEventManager.Instance.RemoveEventListener(PersonalizationManager.ITEM_EQUIPPED_OR_UNEQUIPPED, RefreshDisplays);
        }

        public bool IsDoubleClicked() => m_timeForDoubleClick != -1f && Time.unscaledTime < m_timeForDoubleClick;

        public void SetBrowserUI(UIPersonalizationItemsBrowser itemsBrowser)
        {
            m_browser = itemsBrowser;
        }

        public void RefreshDisplays()
        {
            PersonalizationItemInfo itemInfo = ItemInfo;
            if (itemInfo == null)
                return;

            PersonalizationUserInfo personalizationUserInfo = PersonalizationManager.Instance.userInfo;

            bool isDiscovered = personalizationUserInfo.IsItemDiscovered(itemInfo);
            bool isFavorite = personalizationUserInfo.IsItemFavorite(itemInfo);
            bool equipped = itemInfo.IsEquipped();

            Color bgColor = ModParseUtils.TryParseToColor(equipped ? ITEM_EQUIPPED_BG_COLOR : ITEM_UNEQUIPPED_BG_COLOR, Color.white);
            Color nameBgColor = ModParseUtils.TryParseToColor(equipped ? ITEM_EQUIPPED_NAME_BG_COLOR : ITEM_UNEQUIPPED_NAME_BG_COLOR, Color.white);

            m_bg.color = bgColor;
            m_nameBg.color = nameBgColor;
            m_frame.color = nameBgColor;
            m_favoriteIndicator.SetActive(isFavorite);
            m_newIndicator.SetActive(!isDiscovered);
        }

        private void onClicked()
        {
            PersonalizationItemInfo itemInfo = ItemInfo;
            if (itemInfo == null || !itemInfo.IsUnlocked())
                return;

            PersonalizationUserInfo userInfo = PersonalizationManager.Instance.userInfo;
            if (!userInfo.IsItemDiscovered(itemInfo))
            {
                userInfo.SetIsItemDiscovered(itemInfo);
                m_newIndicator.SetActive(false);
            }

            PersonalizationManager.Instance.EquipItem(itemInfo);
            m_browser.MakeDefaultSkinButtonInteractable();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            PersonalizationItemInfo itemInfo = ItemInfo;
            if (itemInfo == null)
                return;

            m_browser.ShowDescriptionBox(itemInfo, m_rectTransform);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_browser.ShowDescriptionBox(null, null);
        }
    }
}
