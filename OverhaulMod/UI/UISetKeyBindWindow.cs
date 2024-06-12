using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UISetKeyBindWindow : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(OnCloseButtonClicked))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnDoneButtonClicked))]
        [UIElement("DoneButton")]
        private readonly Button m_doneButton;

        [UIElementAction(nameof(OnSetDefaultButtonClicked))]
        [UIElement("SetDefaultButton")]
        private readonly Button m_setDefaultButton;

        [UIElementAction(nameof(OnSetBindAgainButtonClicked))]
        [UIElement("SetBindAgainButton")]
        private readonly Button m_setBindAgainButton;

        [UIElement("PressAnyKeyText", false)]
        private readonly GameObject m_pressAnyKeyTextObject;

        [UIElement("Header")]
        private readonly Text m_header;

        [UIElement("KeyBindText", false)]
        private readonly Text m_keyBindText;

        public override bool refreshOnlyCursor => true;

        private KeyCode m_defaultKey;
        private KeyCode m_setKey;

        private bool m_isWaiting;

        public Action<KeyCode> callBack
        {
            get;
            set;
        }

        public override void Hide()
        {
            base.Hide();
            callBack?.Invoke((KeyCode)(-1));
            callBack = null;
        }

        public override void Update()
        {
            Event cEvent = Event.current;
            if (cEvent != null && cEvent.keyCode != KeyCode.None)
            {
                SetCurrentKeyBind(cEvent.keyCode);
            }
        }

        public void SetContents(string keyBindName, KeyCode defaultKey)
        {
            m_header.text = $"Set key bind for \"{keyBindName}\"";
            m_defaultKey = defaultKey;
            StartWaitingUser();
        }

        public void StartWaitingUser()
        {
            m_pressAnyKeyTextObject.SetActive(true);
            m_keyBindText.gameObject.SetActive(false);
            m_setBindAgainButton.interactable = false;
            m_doneButton.interactable = false;
            m_isWaiting = true;
        }

        public void SetCurrentKeyBind(KeyCode keyCode)
        {
            m_pressAnyKeyTextObject.SetActive(false);
            m_keyBindText.gameObject.SetActive(true);
            m_keyBindText.text = keyCode.ToString().Replace("Alpha", string.Empty);
            m_setBindAgainButton.interactable = true;
            m_doneButton.interactable = true;
            m_isWaiting = false;

            m_setKey = keyCode;
        }

        public void OnCloseButtonClicked()
        {
            Hide();
        }

        public void OnDoneButtonClicked()
        {
            callBack?.Invoke(m_setKey);
            callBack = null;
            Hide();
        }

        public void OnSetDefaultButtonClicked()
        {
            callBack?.Invoke(m_defaultKey);
            callBack = null;
            Hide();
        }

        public void OnSetBindAgainButtonClicked()
        {
            StartWaitingUser();
        }
    }
}
