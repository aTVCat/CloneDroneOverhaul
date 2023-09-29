using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UIElementSettingsMenuKeyBinder : OverhaulBehaviour
    {
        [UIElementReference("BindedKeyLabel")]
        private Text m_BindKeyLabel;

        private UnityAction<KeyCode> m_OnValueChanged;
        public UnityAction<KeyCode> onValueChanged
        {
            get => m_OnValueChanged;
            set => m_OnValueChanged = value;
        }

        public override void Awake()
        {
            UIController.AssignVariables(this);
            Button button = base.GetComponent<Button>();
            button.AddOnClickListener(StartBindingAKey);
        }

        public void BindKey(KeyCode key)
        {
            if(onValueChanged != null)
                onValueChanged(key);

            RefreshLabel(key);
        }

        public void RefreshLabel(KeyCode keyCode)
        {
            m_BindKeyLabel.text = keyCode.ToString();
        }

        public void StartBindingAKey()
        {
            UISettingsMenu settingsMenu = OverhaulUIManager.reference?.GetUI<UISettingsMenu>(UIConstants.UI_SETTINGS_MENU);
            if (!settingsMenu)
                return;

            settingsMenu.currentKeyBinder = this;
        }
    }
}
