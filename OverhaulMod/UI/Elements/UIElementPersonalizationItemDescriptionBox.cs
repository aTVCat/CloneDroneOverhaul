using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationItemDescriptionBox : OverhaulUIBehaviour
    {
        [UIElement("ItemName")]
        private readonly Text m_itemNameText;

        [UIElement("ItemDescription")]
        private readonly Text m_itemDescriptionText;

        [UIElement("LockedOverlay")]
        private readonly GameObject m_lockedOverlay;

        [UIElement("NonVerifiedOverlay")]
        private readonly GameObject m_nonVerifiedOverlay;

        [UIElement("LockedNonVerifiedOverlay")]
        private readonly GameObject m_lockedNonVerifiedOverlay;

        [UIElement("NameHolder")]
        private readonly RectTransform m_nameHolder;

        private RectTransform m_boxTransform;

        private PersonalizationItemInfo m_selectedItemInfo;

        private UIElementMouseEventsComponent m_mouseEvents;

        private UIPersonalizationItemBrowser m_browser;

        private Animator m_animator;

        private bool m_refreshBoxNextFrame;

        protected override void OnInitialized()
        {
            m_boxTransform = base.transform as RectTransform;
            m_mouseEvents = base.gameObject.AddComponent<UIElementMouseEventsComponent>();
            m_animator = base.GetComponent<Animator>();
        }

        public override void Update()
        {
            if (Input.GetMouseButtonDown(0) && !m_mouseEvents.isMouseOverElement && !m_browser.IsMouseOverPanel())
            {
                Hide();
            }

            if (m_refreshBoxNextFrame)
            {
                m_refreshBoxNextFrame = false;

                float initialHeight = 95f - (m_lockedOverlay.activeSelf || m_lockedNonVerifiedOverlay.activeSelf || m_nonVerifiedOverlay.activeSelf ? 0f : 20f);
                float preferredTextHeight = m_itemDescriptionText.preferredHeight + 10f;

                RectTransform t = m_boxTransform;
                Vector2 sizeDelta = t.sizeDelta;
                sizeDelta.y = initialHeight + preferredTextHeight;
                t.sizeDelta = sizeDelta;

                RectTransform t2 = m_itemDescriptionText.rectTransform;
                Vector2 sizeDelta2 = t2.sizeDelta;
                sizeDelta2.y = preferredTextHeight;
                t2.sizeDelta = sizeDelta2;

                RectTransform t3 = m_nameHolder;
                Vector2 sizeDelta3 = t3.sizeDelta;
                sizeDelta3.x = Mathf.Min(m_itemNameText.preferredWidth + 10f, 251f);
                t3.sizeDelta = sizeDelta3;
            }
        }

        public void SetBrowserUI(UIPersonalizationItemBrowser personalizationItemsBrowser)
        {
            m_browser = personalizationItemsBrowser;
        }

        public void ShowForItem(PersonalizationItemInfo itemInfo, RectTransform rectTransform)
        {
            if (itemInfo == null || rectTransform == null)
            {
                Hide();
                return;
            }

            Show();
            if (m_selectedItemInfo == itemInfo)
                return;

            m_animator.Play(string.Empty);
            m_selectedItemInfo = itemInfo;

            bool noSpecificAuthor = false;
            string authorsString = itemInfo.GetAuthorsString(true);
            string prefix;
            if (authorsString == "vanilla")
            {
                noSpecificAuthor = true;
                prefix = LocalizationManager.Instance.GetTranslatedString("customization_vanilla");
            }
            else if (authorsString == "vanilla-hd")
            {
                noSpecificAuthor = true;
                prefix = LocalizationManager.Instance.GetTranslatedString("customization_vanilla_hd");
            }
            else
                prefix = $"{((itemInfo.Authors.IsNullOrEmpty() || itemInfo.Authors.Count <= 1) ? LocalizationManager.Instance.GetTranslatedString("customization_author") : LocalizationManager.Instance.GetTranslatedString("customization_authors"))} ";

            string authorsStringToDisplay;
            if (noSpecificAuthor)
            {
                authorsStringToDisplay = prefix;
            }
            else
            {
                authorsStringToDisplay = $"{prefix}{authorsString.AddColor(Color.white)}";
            }

            m_itemNameText.text = itemInfo.Name;
            m_itemDescriptionText.text = itemInfo.Description;

            bool isLocked = !itemInfo.IsUnlocked();
            m_lockedOverlay.SetActive(isLocked && itemInfo.IsVerified);
            m_nonVerifiedOverlay.SetActive(!itemInfo.IsVerified && !isLocked);
            m_lockedNonVerifiedOverlay.SetActive(!itemInfo.IsVerified && isLocked);

            Transform transform = base.transform;
            Vector3 vector = transform.position;
            vector.y = rectTransform.position.y;
            transform.position = vector;

            m_refreshBoxNextFrame = true;
        }
    }
}