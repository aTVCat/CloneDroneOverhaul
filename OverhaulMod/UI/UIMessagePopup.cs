using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIMessagePopup : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_closeButton;

        [UIElement("Header")]
        private readonly Text m_headerText;

        [UIElement("Description")]
        private readonly Text m_descriptionText;

        [UIElementAction(nameof(OnOkButtonClicked))]
        [UIElement("OKButton", false)]
        private readonly Button m_okButton;

        [UIElementAction(nameof(OnYesButtonClicked))]
        [UIElement("YesButton", false)]
        private readonly Button m_yesButton;

        [UIElementAction(nameof(OnNoButtonClicked))]
        [UIElement("NoButton", false)]
        private readonly Button m_noButton;

        [UIElement("OKButtonText")]
        private readonly Text m_okButtonText;

        [UIElement("YesButtonText")]
        private readonly Text m_yesButtonText;

        [UIElement("NoButtonText")]
        private readonly Text m_noButtonText;

        [UIElement("Panel")]
        private readonly CanvasGroup m_panelCanvasGroup;

        [UIElement("Panel")]
        private readonly RectTransform m_panelTransform;

        public override bool refreshOnlyCursor => true;

        public Action okButtonAction
        {
            get;
            private set;
        }

        public Action yesButtonAction
        {
            get;
            private set;
        }

        public Action noButtonAction
        {
            get;
            private set;
        }

        public override void Hide()
        {
            base.Hide();
            okButtonAction = null;
            yesButtonAction = null;
            noButtonAction = null;
        }

        public void OnOkButtonClicked()
        {
            okButtonAction?.Invoke();
            Hide();
        }

        public void OnYesButtonClicked()
        {
            yesButtonAction?.Invoke();
            Hide();
        }

        public void OnNoButtonClicked()
        {
            noButtonAction?.Invoke();
            Hide();
        }

        public void SetTexts(string header, string description)
        {
            m_headerText.text = header;
            m_descriptionText.text = description;
        }

        public void SetHeight(float height)
        {
            Vector2 sizeDelta = m_panelTransform.sizeDelta;
            sizeDelta.y = Mathf.Max(100f, height);
            m_panelTransform.sizeDelta = sizeDelta;
        }

        public void SetButtonLayout(MessageMenu.ButtonLayout buttonLayout)
        {
            m_okButton.gameObject.SetActive(buttonLayout == MessageMenu.ButtonLayout.OkButton);
            m_yesButton.gameObject.SetActive(buttonLayout == MessageMenu.ButtonLayout.EnableDisableButtons);
            m_noButton.gameObject.SetActive(buttonLayout == MessageMenu.ButtonLayout.EnableDisableButtons);
        }

        public void SetButtonActions(Action ok, Action yes, Action no)
        {
            okButtonAction = ok;
            yesButtonAction = yes;
            noButtonAction = no;
        }

        public void SetButtonTexts(string okText, string yesText, string noText)
        {
            if (okText == null)
                okText = string.Empty;
            if (yesText == null)
                yesText = string.Empty;
            if (noText == null)
                noText = string.Empty;

            m_okButtonText.text = okText;
            m_yesButtonText.text = yesText;
            m_noButtonText.text = noText;
        }
    }
}
