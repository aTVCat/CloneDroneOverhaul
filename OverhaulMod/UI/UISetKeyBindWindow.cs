using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UISetKeyBindWindow : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(OnCloseButtonClicked))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnDoneButtonClicked))]
        [UIElement("DoneButton")]
        private readonly Button _doneButton;

        [UIElementAction(nameof(OnSetDefaultButtonClicked))]
        [UIElement("SetDefaultButton")]
        private readonly Button _setDefaultButton;

        [UIElementAction(nameof(OnSetBindAgainButtonClicked))]
        [UIElement("SetBindAgainButton")]
        private readonly Button _setBindAgainButton;

        [UIElement("PressAnyKeyText", false)]
        private readonly GameObject _pressAnyKeyTextObject;

        [UIElement("Header")]
        private readonly Text _header;

        [UIElement("KeyBindText", false)]
        private readonly Text _keyBindText;

        public override bool refreshOnlyCursor => true;

        private KeyCode _defaultKey;
        private KeyCode _setKey;

        private bool _isWaiting;

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
            _header.text = $"{LocalizationManager.Instance.GetTranslatedString("set_keybind_for")} \"{keyBindName}\"";
            _defaultKey = defaultKey;
            StartWaitingUser();
        }

        public void StartWaitingUser()
        {
            _pressAnyKeyTextObject.SetActive(true);
            _keyBindText.gameObject.SetActive(false);
            _setBindAgainButton.interactable = false;
            _doneButton.interactable = false;
            _isWaiting = true;
        }

        public void SetCurrentKeyBind(KeyCode keyCode)
        {
            _pressAnyKeyTextObject.SetActive(false);
            _keyBindText.gameObject.SetActive(true);
            _keyBindText.text = keyCode.ToString().Replace("Alpha", string.Empty);
            _setBindAgainButton.interactable = true;
            _doneButton.interactable = true;
            _isWaiting = false;

            _setKey = keyCode;
        }

        public void OnCloseButtonClicked()
        {
            Hide();
        }

        public void OnDoneButtonClicked()
        {
            callBack?.Invoke(_setKey);
            callBack = null;
            Hide();
        }

        public void OnSetDefaultButtonClicked()
        {
            callBack?.Invoke(_defaultKey);
            callBack = null;
            Hide();
        }

        public void OnSetBindAgainButtonClicked()
        {
            StartWaitingUser();
        }
    }
}
