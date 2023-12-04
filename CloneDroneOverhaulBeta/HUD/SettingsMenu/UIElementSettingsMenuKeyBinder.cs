using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UIElementSettingsMenuKeyBinder : OverhaulBehaviour
    {
        [UIElementReference("BindedKeyLabel")]
        private readonly Text m_BindKeyLabel;

        public UnityAction<KeyCode> onValueChanged { get; set; }

        public override void Awake()
        {
            UIController.AssignVariables(this);
            Button button = base.GetComponent<Button>();
            button.AddOnClickListener(StartBindingAKey);
        }

        public void BindKey(KeyCode key)
        {
            onValueChanged?.Invoke(key);

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
