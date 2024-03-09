using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementKeyBindSetter : OverhaulUIBehaviour
    {
        [UIElement("Text")]
        private Text m_keyBindText;

        [UIElementAction(nameof(OnSetBindButtonClicked))]
        [UIElement("SetBindButton")]
        private Button m_setBindButton;

        [UIElementAction(nameof(OnSetDefaultButtonClicked))]
        [UIElement("SetDefaultBindButton")]
        private Button m_setDefaultBindButton;

        private KeyCode m_key;
        public KeyCode key
        {
            get
            {
                return m_key;
            }
            set
            {
                m_key = value;
                m_keyBindText.text = value.ToString().Replace("Alpha", string.Empty);
                onValueChanged.Invoke(value);
            }
        }

        private KeyCode m_defaultKey;
        public KeyCode defaultKey
        {
            get
            {
                return m_defaultKey;
            }
            set
            {
                m_defaultKey = value;
            }
        }

        public KeyCodeChangedEvent onValueChanged { get; set; } = new KeyCodeChangedEvent();

        public void OnSetBindButtonClicked()
        {
            ModUIUtils.KeyBinder(name, defaultKey, delegate (KeyCode kc)
            {
                key = kc;
            }, null);
        }

        public void OnSetDefaultButtonClicked()
        {
            key = defaultKey;
        }

        [Serializable]
        public class KeyCodeChangedEvent : UnityEvent<KeyCode>
        {
        }
    }
}
