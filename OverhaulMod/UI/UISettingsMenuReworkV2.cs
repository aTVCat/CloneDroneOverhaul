using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UISettingsMenuReworkV2 : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _closeButton;

        [UIElement("DevField", false)]
        private readonly GameObject _devField;

        [UIElementAction(nameof(OnStartEditingSettingsClicked))]
        [UIElement("StartEditingButton")]
        private readonly Button _startEditingButton;

        [UIElementAction(nameof(OnStopEditingSettingsClicked))]
        [UIElement("StopEditingButton")]
        private readonly Button _stopEditingButton;

        [UIElementAction(nameof(OnSaveElementDescriptionsClicked))]
        [UIElement("SaveButton")]
        private readonly Button _saveButton;

        public override bool HideTitleScreen => true;

        private bool _isInEditorMode;

        protected override void OnInitialized()
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.populateSettings();

            _devField.SetActive(ModUserInfo.IsDeveloper);
            _startEditingButton.gameObject.SetActive(true);
            _stopEditingButton.gameObject.SetActive(false);
            _saveButton.gameObject.SetActive(false);
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
            ModSettingsDataManager.Instance.Save();
        }

        public bool IsInEditorMode() => _isInEditorMode;

        public void OnStartEditingSettingsClicked()
        {
            _startEditingButton.gameObject.SetActive(false);
            _stopEditingButton.gameObject.SetActive(true);
            _saveButton.gameObject.SetActive(true);

            _isInEditorMode = true;
        }

        public void OnStopEditingSettingsClicked()
        {
            _startEditingButton.gameObject.SetActive(true);
            _stopEditingButton.gameObject.SetActive(false);
            _saveButton.gameObject.SetActive(false);

            _isInEditorMode = false;
        }

        public void OnSaveElementDescriptionsClicked()
        {
            ModSettingsManager.Instance.SaveElementDescriptions();

            _saveButton.interactable = false;
            DelegateScheduler.Instance.Schedule(delegate
            {
                if (_saveButton) _saveButton.interactable = true;
            }, 3f);
        }
    }
}