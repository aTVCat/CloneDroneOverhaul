using OverhaulMod.Utils;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementKeyBindSetter : OverhaulUIBehaviour
    {
        [UIElement("Text")]
        private readonly Text _keyBindText;

        [UIElementAction(nameof(OnSetBindButtonClicked))]
        [UIElement("SetBindButton")]
        private readonly Button _setBindButton;

        [UIElementAction(nameof(OnSetDefaultButtonClicked))]
        [UIElement("SetDefaultBindButton")]
        private readonly Button _setDefaultBindButton;

        [UIElement("Description")]
        private readonly Text _description;

        private KeyCode _key;
        public KeyCode key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
                _keyBindText.text = value.ToString().Replace("Alpha", string.Empty);
                onValueChanged.Invoke(value);
            }
        }

        public KeyCode defaultKey { get; set; }

        public KeyCodeChangedEvent onValueChanged { get; set; } = new KeyCodeChangedEvent();

        public void OnSetBindButtonClicked()
        {
            ModUIUtils.KeyBinder(_description.text, defaultKey, delegate (KeyCode kc)
            {
                if (kc == (KeyCode)(-1))
                    return;

                key = kc;
            }, null);
        }

        public void OnSetDefaultButtonClicked()
        {
            key = defaultKey;
        }

        public void SetDescription(string text)
        {
            _description.text = text;
        }

        [Serializable]
        public class KeyCodeChangedEvent : UnityEvent<KeyCode>
        {
        }
    }
}
