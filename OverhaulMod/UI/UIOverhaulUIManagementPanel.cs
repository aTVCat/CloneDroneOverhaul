using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIOverhaulUIManagementPanel : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElement("UIDisplay", false)]
        private readonly ModdedObject _uiDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform _uiDisplayContainer;

        [UIElementAction(nameof(OnEnableAllButtonClicked))]
        [UIElement("EnableAllButton")]
        private readonly Button _enableAllButton;

        [UIElementAction(nameof(OnDisableAllButtonClicked))]
        [UIElement("DisableAllButton")]
        private readonly Button _disableAllButton;

        private List<Toggle> _instantiatedToggles;

        protected override void OnInitialized()
        {
            _instantiatedToggles = new List<Toggle>();
        }

        public override void Show()
        {
            base.Show();

            if (_uiDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_uiDisplayContainer);

            _instantiatedToggles.Clear();
            instantiateToggles();
        }

        private void instantiateToggles()
        {
            foreach (ModSetting setting in ModSettingsManager.Instance.GetSettings(ModSetting.Tags.UISetting))
            {
                InstantiateToggle(setting.Name);
            }
        }

        public void InstantiateToggle(string settingId)
        {
            ModSetting setting = ModSettingsManager.Instance.GetSetting(settingId);
            if (setting == null || setting.ValueType != ModSetting.ValueTypes.Bool)
                return;

            ModdedObject moddedObject = Instantiate(_uiDisplayPrefab, _uiDisplayContainer);
            moddedObject.gameObject.SetActive(true);
            moddedObject.GetObject<Text>(0).text = LocalizationManager.Instance.GetTranslatedString(setting.Name);
            moddedObject.GetObject<Button>(2).onClick.AddListener(delegate
            {
                GUIUtility.systemCopyBuffer = setting.Name;
                ModUIUtils.MessagePopupOK("Copied localization ID", "", false);
            });

            UIElementOverhaulUIInfo info = moddedObject.gameObject.AddComponent<UIElementOverhaulUIInfo>();
            info.PreviewFile = Path.Combine(ModCore.TexturesFolder, "uiPreviews", $"{setting.Name.Replace("ModUI_UI", string.Empty).Replace("Rework", string.Empty)}.png");
            info.InitializeElement();

            bool isOn = (bool)setting.GetFieldValue();

            Toggle toggle = moddedObject.GetComponent<Toggle>();
            toggle.isOn = isOn;
            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                setting.SetBoolValue(value);
            });

            _instantiatedToggles.Add(toggle);
        }

        public void OnEnableAllButtonClicked()
        {
            if (_instantiatedToggles.IsNullOrEmpty())
                return;

            ModUIUtils.MessagePopup(true, LocalizationManager.Instance.GetTranslatedString("settings_configure_overhaul_mod_uis_header_enable_all_modded_uis"), "", 100f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
            {
                foreach (Toggle toggle in _instantiatedToggles)
                    toggle.isOn = true;
            });
        }

        public void OnDisableAllButtonClicked()
        {
            if (_instantiatedToggles.IsNullOrEmpty())
                return;

            ModUIUtils.MessagePopup(true, LocalizationManager.Instance.GetTranslatedString("settings_configure_overhaul_mod_uis_header_disable_all_modded_uis"), "", 100f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
            {
                foreach (Toggle toggle in _instantiatedToggles)
                    toggle.isOn = false;
            });
        }
    }
}
