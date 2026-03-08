using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationEditorUtilitiesPanel : OverhaulUIBehaviour
    {
        [ShowTooltipOnHighLight("Show player", 1f)]
        [UIElementAction(nameof(OnShowPlayerToggled))]
        [UIElement("ShowPlayerToggle")]
        private readonly Toggle _showPlayerToggle;

        [ShowTooltipOnHighLight("Show weapon", 1f)]
        [UIElementAction(nameof(OnShowWeaponToggled))]
        [UIElement("ShowWeaponToggle")]
        private readonly Toggle _showWeaponToggle;

        [ShowTooltipOnHighLight("Enable animations", 1f)]
        [UIElementAction(nameof(OnAnimationToggled))]
        [UIElement("EnableAnimationToggle")]
        private readonly Toggle _enableAnimationToggle;

        [ShowTooltipOnHighLight("Show original model (Over skin)", 1f)]
        [UIElementAction(nameof(OnOriginalModelToggled))]
        [UIElement("EnableOriginalModelToggle")]
        private readonly Toggle _enableOriginalModelToggle;

        [UIElementAction(nameof(OnPresetPreviewChanged))]
        [UIElement("PresetPreviewDropdown")]
        private readonly Dropdown _presetPreviewDropdown;

        [UIElement("FavoriteColorPreviewDropdown")]
        private readonly Dropdown _favoriteColorPreviewDropdown;

        protected override void OnInitialized()
        {
            _favoriteColorPreviewDropdown.options = HumanFactsManager.Instance.GetColorDropdownOptions();
            _favoriteColorPreviewDropdown.value = SettingsManager.Instance.GetCurrentOrCreateMultiplayerHumanSlot(out _).RootModel.ColorIndex;
            _favoriteColorPreviewDropdown.onValueChanged.AddListener(OnFavoriteColorPreviewDropdownChanged);
        }

        public Color GetFavoriteColor()
        {
            return HumanFactsManager.Instance.FavouriteColors[Mathf.Max(0, _favoriteColorPreviewDropdown.value - 1)].ColorValue;
        }

        public void SetRandomFavoriteColor()
        {
            _favoriteColorPreviewDropdown.value = Random.Range(0, _favoriteColorPreviewDropdown.options.Count);
        }

        public void SetConditionOptions(List<Dropdown.OptionData> options)
        {
            _presetPreviewDropdown.options = options;
            _presetPreviewDropdown.value = 0;
        }

        public void SetAnimationToggleOn()
        {
            _enableAnimationToggle.isOn = true;
        }

        public void SetPresetPreview(WeaponVariant2 weaponVariant)
        {
            List<Dropdown.OptionData> options = _presetPreviewDropdown.options;

            int i = 0;
            foreach (Dropdown.OptionData option in options)
            {
                if (option is DropdownWeaponVariantOptionData dropdownWeaponVariantOptionData && dropdownWeaponVariantOptionData.Value == weaponVariant)
                {
                    _presetPreviewDropdown.value = i;
                    return;
                }
                i++;
            }

            options.Add(new DropdownWeaponVariantOptionData(weaponVariant));
            _presetPreviewDropdown.value = options.Count - 1;
        }

        public void OnShowPlayerToggled(bool value)
        {
            FirstPersonMover firstPersonMover = PersonalizationEditorManager.Instance.GetBot();
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
            FirstPersonMover firstPersonMover = PersonalizationEditorManager.Instance.GetBot();
            if (!firstPersonMover)
                return;

            if (value)
                firstPersonMover.ShowTemporarilyHiddenWeaponModels();
            else
                firstPersonMover.TemporarilyHideWeaponModels();
        }

        public void OnAnimationToggled(bool value)
        {
            FirstPersonMover firstPersonMover = PersonalizationEditorManager.Instance.GetBot();
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

        public void OnOriginalModelToggled(bool value)
        {
            PersonalizationEditorManager.Instance.originalModelsEnabled = value;
        }

        public void OnPresetPreviewChanged(int value)
        {
            PersonalizationEditorManager.Instance.previewPresetKey = (_presetPreviewDropdown.options[value] as DropdownWeaponVariantOptionData).Value;
            GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.PRESET_PREVIEW_CHANGED_EVENT);
        }

        public void OnFavoriteColorPreviewDropdownChanged(int value)
        {
            if (!PersonalizationEditorManager.Instance.IsInScreenshotMode()) PersonalizationEditorManager.Instance.SerializeRotAndRespawnBot();
        }
    }
}
