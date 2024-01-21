using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorSavePanel : PersonalizationEditorUIElement
    {
        [ActionReference(nameof(onSaveButtonClicked))]
        [ObjectReference("SaveButton")]
        private readonly Button m_SaveButton;

        [ActionReference(nameof(onAutoSaveToggleClicked))]
        [ObjectReference("AutoSaveToggle")]
        private readonly Toggle m_AutoSaveToggle;

        [ObjectReference("NeedsSavingIndicator")]
        private readonly GameObject m_NeedsSaveIndicator;

        public bool NeedsToSave
        {
            get => m_NeedsSaveIndicator.activeSelf;
            set => m_NeedsSaveIndicator.SetActive(value);
        }

        public override void Start()
        {
            base.Start();

            m_AutoSaveToggle.isOn = PersonalizationEditor.IsAutoSaveEnabled;
            NeedsToSave = false;
        }

        private void onAutoSaveToggleClicked(bool newValue)
        {
            OverhaulSettingsController.SetSettingValue("Player.P_Editor.AutoSave", newValue);
        }

        private void onSaveButtonClicked()
        {
            PersonalizationEditor.SaveEditingItem(true);
            NeedsToSave = false;
        }
    }
}
