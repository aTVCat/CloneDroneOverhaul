using OverhaulMod.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIMessagePopup : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _closeButton;

        [UIElement("Header")]
        private readonly Text _headerText;

        [UIElement("Description")]
        private readonly Text _descriptionText;

        [UIElementAction(nameof(OnOkButtonClicked))]
        [UIElement("OKButton", false)]
        private readonly Button _okButton;

        [UIElementAction(nameof(OnYesButtonClicked))]
        [UIElement("YesButton", false)]
        private readonly Button _yesButton;

        [UIElementAction(nameof(OnNoButtonClicked))]
        [UIElement("NoButton", false)]
        private readonly Button _noButton;

        [UIElement("OKButtonText")]
        private readonly Text _okButtonText;

        [UIElement("YesButtonText")]
        private readonly Text _yesButtonText;

        [UIElement("NoButtonText")]
        private readonly Text _noButtonText;

        [UIElement("Panel")]
        private readonly CanvasGroup _panelCanvasGroup;

        [UIElement("Panel")]
        private readonly RectTransform _panelTransform;

        [UIElementIgnoreIfMissing]
        [UIElement("ScrollRect")]
        private readonly GameObject _scrollRectObject;

        [UIElementIgnoreIfMissing]
        [UIElement("ScrollRect")]
        private readonly ScrollRect _scrollRect;

        public bool IsFullscreen;

        private bool _shouldRefreshText;

        public override bool RefreshOnlyCursor => true;

        private Action _okButtonAction;

        private Action _yesButtonAction;

        private Action _noButtonAction;

        public override void Hide()
        {
            base.Hide();
        }

        public override void Update()
        {
            base.Update();
            if (_shouldRefreshText)
            {
                _shouldRefreshText = false;

                Vector2 sizeDelta = _descriptionText.rectTransform.sizeDelta;
                sizeDelta.y = _descriptionText.preferredHeight + 30f;
                _descriptionText.rectTransform.sizeDelta = sizeDelta;

                ModActionUtils.DoInFrames(refreshScrollRect, 2);
            }
        }

        private void refreshScrollRect()
        {
            if (_scrollRect) _scrollRect.verticalNormalizedPosition = 1f; // for some reason it sometimes auto scrolls to anywhere, but the top
        }

        private void clearCallbacks()
        {
            _okButtonAction = null;
            _yesButtonAction = null;
            _noButtonAction = null;
        }

        private void clearCallbackIfNotVisible()
        {
            if (!IsVisible) clearCallbacks();
        }

        public void OnOkButtonClicked()
        {
            Hide();
            if(_okButtonAction != null) _okButtonAction();
            clearCallbackIfNotVisible();
        }

        public void OnYesButtonClicked()
        {
            Hide();
            if (_yesButtonAction != null) _yesButtonAction();
            clearCallbackIfNotVisible();
        }

        public void OnNoButtonClicked()
        {
            Hide();
            if (_noButtonAction != null) _noButtonAction();
            clearCallbackIfNotVisible();
        }

        public void SetTexts(string header, string description)
        {
            _headerText.text = header;
            _descriptionText.text = description;
            refreshTextHeightNextFrame();

            if (_scrollRectObject)
            {
                _scrollRectObject.SetActive(!description.IsNullOrEmpty());
            }
        }

        public void SetHeight(float height)
        {
            Vector2 sizeDelta = _panelTransform.sizeDelta;
            sizeDelta.y = Mathf.Max(100f, height);
            _panelTransform.sizeDelta = sizeDelta;
        }

        public void SetButtonLayout(MessageMenu.ButtonLayout buttonLayout)
        {
            _okButton.gameObject.SetActive(buttonLayout == MessageMenu.ButtonLayout.OkButton);
            _yesButton.gameObject.SetActive(buttonLayout == MessageMenu.ButtonLayout.EnableDisableButtons);
            _noButton.gameObject.SetActive(buttonLayout == MessageMenu.ButtonLayout.EnableDisableButtons);
        }

        public void SetButtonActions(Action ok, Action yes, Action no)
        {
            _okButtonAction = ok;
            _yesButtonAction = yes;
            _noButtonAction = no;
        }

        public void SetButtonTexts(string okText, string yesText, string noText)
        {
            if (okText == null)
                okText = string.Empty;
            if (yesText == null)
                yesText = string.Empty;
            if (noText == null)
                noText = string.Empty;

            if (yesText.ToLower() == "yes")
                yesText = LocalizationManager.Instance.GetTranslatedString("button_yes");
            if (noText.ToLower() == "no")
                noText = LocalizationManager.Instance.GetTranslatedString("button_no");

            _okButtonText.text = okText;
            _yesButtonText.text = yesText;
            _noButtonText.text = noText;
        }

        private void refreshTextHeightNextFrame()
        {
            _shouldRefreshText = true;
        }
    }
}
