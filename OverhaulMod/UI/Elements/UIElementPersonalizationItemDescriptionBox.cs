using OverhaulMod.Content.Personalization;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationItemDescriptionBox : OverhaulUIBehaviour
    {
        [UIElement("ItemName")]
        private readonly Text _itemNameText;

        [UIElement("ItemDescription")]
        private readonly Text _itemDescriptionText;

        [UIElement("ItemAuthor")]
        private readonly Text _itemAuthorText;

        [UIElement("LockedOverlay")]
        private readonly GameObject _lockedOverlay;

        [UIElement("NonVerifiedOverlay")]
        private readonly GameObject _nonVerifiedOverlay;

        [UIElement("LockedNonVerifiedOverlay")]
        private readonly GameObject _lockedNonVerifiedOverlay;

        [UIElement("NameHolder")]
        private readonly RectTransform _nameHolder;

        private RectTransform _boxTransform;

        private PersonalizationItemInfo _selectedItemInfo;

        private UIElementMouseEventsComponent _mouseEvents;

        private UIPersonalizationItemBrowser _browser;

        private Animator _animator;

        private bool _refreshBoxNextFrame;

        protected override void OnInitialized()
        {
            _boxTransform = base.transform as RectTransform;
            _mouseEvents = base.gameObject.AddComponent<UIElementMouseEventsComponent>();
            _animator = base.GetComponent<Animator>();
        }

        public override void Update()
        {
            if (Input.GetMouseButtonDown(0) && !_mouseEvents.IsMouseOverElement && !_browser.IsMouseOverPanel())
            {
                Hide();
            }

            if (_refreshBoxNextFrame)
            {
                _refreshBoxNextFrame = false;

                float initialHeight = 95f - (_lockedOverlay.activeSelf || _lockedNonVerifiedOverlay.activeSelf || _nonVerifiedOverlay.activeSelf ? 0f : 20f);
                float preferredTextHeight = _itemDescriptionText.preferredHeight + 10f;

                RectTransform t = _boxTransform;
                Vector2 sizeDelta = t.sizeDelta;
                sizeDelta.y = initialHeight + preferredTextHeight;
                t.sizeDelta = sizeDelta;

                RectTransform t2 = _itemDescriptionText.rectTransform;
                Vector2 sizeDelta2 = t2.sizeDelta;
                sizeDelta2.y = preferredTextHeight;
                t2.sizeDelta = sizeDelta2;

                RectTransform t3 = _nameHolder;
                Vector2 sizeDelta3 = t3.sizeDelta;
                sizeDelta3.x = Mathf.Min(_itemNameText.preferredWidth + 10f, 251f);
                t3.sizeDelta = sizeDelta3;
            }
        }

        public void SetBrowserUI(UIPersonalizationItemBrowser personalizationItemsBrowser)
        {
            _browser = personalizationItemsBrowser;
        }

        public void ShowForItem(PersonalizationItemInfo itemInfo, RectTransform rectTransform)
        {
            if (itemInfo == null || rectTransform == null)
            {
                Hide();
                return;
            }

            Show();
            if (_selectedItemInfo == itemInfo)
                return;

            _animator.Play(string.Empty);
            _selectedItemInfo = itemInfo;

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
                prefix = itemInfo.Authors.Count <= 1 ? LocalizationManager.Instance.GetTranslatedString("customization_author") : LocalizationManager.Instance.GetTranslatedString("customization_authors");

            string authorsStringToDisplay;
            if (noSpecificAuthor)
            {
                authorsStringToDisplay = prefix.AddColor(Color.yellow);
            }
            else
            {
                authorsStringToDisplay = $"{prefix} {authorsString.AddColor(Color.white)}";
            }

            _itemNameText.text = itemInfo.Name;
            _itemAuthorText.text = authorsStringToDisplay;
            _itemDescriptionText.text = itemInfo.Description;

            bool isLocked = !itemInfo.IsUnlocked();
            _lockedOverlay.SetActive(isLocked && itemInfo.IsVerified);
            _nonVerifiedOverlay.SetActive(!itemInfo.IsVerified && !isLocked);
            _lockedNonVerifiedOverlay.SetActive(!itemInfo.IsVerified && isLocked);

            Transform transform = base.transform;
            Vector3 vector = transform.position;
            vector.y = rectTransform.position.y;
            transform.position = vector;

            _refreshBoxNextFrame = true;
        }
    }
}