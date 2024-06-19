using OverhaulMod.Content.Personalization;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationEditorUtilitiesPanel : OverhaulUIBehaviour
    {
        [ShowTooltipOnHighLight("Show player", 1f)]
        [UIElementAction(nameof(OnShowPlayerToggled))]
        [UIElement("ShowPlayerToggle")]
        private readonly Toggle m_showPlayerToggle;

        [ShowTooltipOnHighLight("Show weapon", 1f)]
        [UIElementAction(nameof(OnShowWeaponToggled))]
        [UIElement("ShowWeaponToggle")]
        private readonly Toggle m_showWeaponToggle;

        [ShowTooltipOnHighLight("Enable animations", 1f)]
        [UIElementAction(nameof(OnAnimationToggled))]
        [UIElement("EnableAnimationToggle")]
        private readonly Toggle m_enableAnimationToggle;

        [UIElement("PresetPreviewDropdown")]
        private readonly Dropdown m_presetPreviewDropdown;

        protected override void OnInitialized()
        {
            m_presetPreviewDropdown.options = PersonalizationEditorManager.Instance.GetConditionOptions();
            m_presetPreviewDropdown.onValueChanged.AddListener(OnPresetPreviewChanged);
        }

        public void OnShowPlayerToggled(bool value)
        {
            FirstPersonMover firstPersonMover = CharacterTracker.Instance?.GetPlayerRobot();
            if (!firstPersonMover)
                return;

            CharacterModel characterModel = firstPersonMover.GetCharacterModel();
            if (!characterModel)
                return;

            if (value)
                characterModel.ShowAllHiddenBodyPartsAndArmor();
            else
                characterModel.HideAllBodyPartsandArmor();
        }

        public void OnShowWeaponToggled(bool value)
        {
            FirstPersonMover firstPersonMover = CharacterTracker.Instance?.GetPlayerRobot();
            if (!firstPersonMover)
                return;

            if (value)
                firstPersonMover.ShowTemporarilyHiddenWeaponModels();
            else
                firstPersonMover.TemporarilyHideWeaponModels();
        }

        public void OnAnimationToggled(bool value)
        {
            FirstPersonMover firstPersonMover = CharacterTracker.Instance?.GetPlayerRobot();
            if (!firstPersonMover)
                return;

            CharacterModel characterModel = firstPersonMover.GetCharacterModel();
            if (!characterModel)
                return;

            characterModel.SetManualUpperAnimationEnabled(!value);
            characterModel.SetManualLegsAnimationEnabled(!value);
            if (!value)
            {
                characterModel.UpperAnimator.transform.localEulerAngles = Vector3.zero;
                characterModel.LegsAnimator.transform.localEulerAngles = Vector3.zero;
            }
        }

        public void OnBowStringsToggled(bool value)
        {

        }

        public void OnPresetPreviewChanged(int value)
        {
            PersonalizationEditorManager.Instance.previewPresetKey = (PersonalizationEditorObjectShowConditions)(value + 1);
            GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.PRESET_PREVIEW_CHANGED_EVENT);
        }
    }
}
