using OverhaulMod.Engine;
using OverhaulMod.Utils;
using OverhaulMod.Visuals;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPhotoModeUIRework : OverhaulUIBehaviour
    {
        [UIElement("ExpandButton", typeof(UIElementExpandButton))]
        private readonly UIElementExpandButton _expandButton;

        [UIElement("LightingPanel")]
        private readonly RectTransform _lightingPanel;

        [UIElementAction(nameof(OnSaveRLightInfoButtonClicked))]
        [UIElement("SaveRLightInfoButton")]
        private readonly Button _saveRLightInfoButton;

        [UIElementAction(nameof(OnRestoreDefaultsButtonClicked))]
        [UIElement("RestoreDefaultsButton")]
        private readonly Button _restoreDefaultsButton;

        [UIElementCallback(true)]
        [UIElementAction(nameof(OnTimeScaleChanged))]
        [UIElement("TimeScaleSlider")]
        private readonly Slider _timeScaleSlider;


        [UIElementAction(nameof(OnShowPlayerToggled))]
        [UIElement("ShowPlayerToggle")]
        private readonly Toggle _showPlayerToggle;

        [UIElementAction(nameof(OnShowEnemiesToggled))]
        [UIElement("ShowEnemiesToggle")]
        private readonly Toggle _showEnemiesToggle;

        [UIElementAction(nameof(OnShowGarbageToggled))]
        [UIElement("ShowGarbageToggle")]
        private readonly Toggle _showGarbageToggle;

        [UIElementAction(nameof(OnHUDToggled))]
        [UIElement("ShowHUDToggle")]
        private readonly Toggle _showHUDToggle;


        [UIElementAction(nameof(OnCinematicBordersToggled))]
        [UIElement("CinematicBordersToggle")]
        private readonly Toggle _cinematicBordersToggle;

        [UIElementAction(nameof(OnCinematicBordersHeightChanged))]
        [UIElement("CinematicBordersHeightSlider")]
        private readonly Slider _cinematicBordersHeightSlider;

        [UIElementAction(nameof(OnSSAOToggled))]
        [UIElement("SSAOToggle")]
        private readonly Toggle _ambientOcclusionToggle;

        [UIElementAction(nameof(OnVignetteToggled))]
        [UIElement("VignetteToggle")]
        private readonly Toggle _vignetteToggle;

        [UIElementAction(nameof(OnVignetteIntensityChanged))]
        [UIElement("VignetteIntensitySlider")]
        private readonly Slider _vignetteIntensitySlider;

        [UIElementAction(nameof(OnDitheringToggled))]
        [UIElement("DitheringToggle")]
        private readonly Toggle _ditheringToggle;

        [UIElementAction(nameof(OnGlobalIlluminationToggled))]
        [UIElement("GlobalIlluminationToggle")]
        private readonly Toggle _globalIlluminationToggle;

        [UIElementAction(nameof(OnReflectionProbeToggled))]
        [UIElement("ReflectionProbeToggle")]
        private readonly Toggle _reflectionProbeToggle;

        [UIElementAction(nameof(OnCAToggled))]
        [UIElement("CAToggle")]
        private readonly Toggle _caToggle;

        [UIElementAction(nameof(OnSunShaftsToggled))]
        [UIElement("SunShaftsToggle")]
        private readonly Toggle _sunShaftsToggle;

        [UIElementAction(nameof(OnDoFToggled))]
        [UIElement("DoFToggle")]
        private readonly Toggle _dofToggle;


        [UIElementAction(nameof(OnFogToggled))]
        [UIElement("FogToggle")]
        private readonly Toggle _fogToggle;

        [ColorPicker(false)]
        [UIElementAction(nameof(OnFogColored))]
        [UIElement("FogColor")]
        private readonly UIElementColorPickerButton _fogColor;

        [UIElementAction(nameof(OnFogStartChanged))]
        [UIElement("FogStartSlider")]
        private readonly Slider _fogStartSlider;

        [UIElementAction(nameof(OnFogEndChanged))]
        [UIElement("FogEndSlider")]
        private readonly Slider _fogEndSlider;


        [UIElementAction(nameof(OnDirectionalLightToggled))]
        [UIElement("DirectionalLightToggle")]
        private readonly Toggle _directionalLightToggle;

        [ColorPicker(false)]
        [UIElementAction(nameof(OnDirectionalLightColored))]
        [UIElement("DirectionalLightColor")]
        private readonly UIElementColorPickerButton _directionalLightColor;

        [UIElementAction(nameof(OnDirectionalLightXChanged))]
        [UIElement("DirectionalLightX")]
        private readonly Slider _directionalLightXSlider;

        [UIElementAction(nameof(OnDirectionalLightYChanged))]
        [UIElement("DirectionalLightY")]
        private readonly Slider _directionalLightYSlider;

        [UIElementAction(nameof(OnDirectionalLightIntensityChanged))]
        [UIElement("DirectionalLightIntensity")]
        private readonly Slider _directionalLightIntensitySlider;

        [UIElementAction(nameof(OnDirectionalLightShadowsChanged))]
        [UIElement("DirectionalLightShadows")]
        private readonly Slider _directionalLightShadowsSlider;


        [UIElementAction(nameof(OnSkyBoxIndexChanged))]
        [UIElement("SkyboxSlider")]
        private readonly Slider _skyBoxSlider;

        [UIElementAction(nameof(OnUseRealisticSkyBoxesToggled))]
        [UIElement("RealisticSkyboxToggle")]
        private readonly Toggle _realisticSkyBoxToggle;

        [UIElementAction(nameof(OnRealisticSkyBoxDropdownChanged))]
        [UIElement("RealisticSkyboxDropdown")]
        private readonly Dropdown _realisticSkyBoxDropdown;

        [UIElement("RealisticSkyboxDropdownField")]
        private readonly GameObject _realisticSkyBoxDropdownFieldObject;

        [ColorPicker(false)]
        [UIElementAction(nameof(OnRealisticSkyboxTintChanged))]
        [UIElement("RealisticSkyboxColor")]
        private readonly UIElementColorPickerButton _realisticSkyboxColor;

        [UIElementAction(nameof(OnRealisticSkyboxRotationChanged))]
        [UIElement("RealisticSkyboxRotationSlider")]
        private readonly Slider _realisticSkyboxRotationColor;


        [UIElementAction(nameof(OnAutoResetLightingSettingsToggleChanged))]
        [UIElement("AutoResetLightingToggle")]
        private readonly Toggle _autoResetLightingSettingsToggle;

        private LightingInfo _lightingInfo;

        private bool _disallowCallbacks;

        private List<GarbageTarget> _garbageTargets;

        public override bool CloseOnEscapeButtonPress => false;

        public override bool EnableCursor => true;

        protected override void OnInitialized()
        {
            RectTransform lightingPanel = _lightingPanel;
            lightingPanel.sizeDelta = new Vector2(225f, 400f);

            UIElementExpandButton expandButton = _expandButton;
            expandButton.rectTransform = lightingPanel;
            expandButton.collapsedSize = lightingPanel.sizeDelta;
            expandButton.expandedSize = new Vector2(375f, 400f);

            _directionalLightColor.colorPickerParent = base.transform;
            _fogColor.colorPickerParent = base.transform;

            _saveRLightInfoButton.gameObject.SetActive(ModUserInfo.isDeveloper);
        }

        public override void Show()
        {
            base.Show();
            ModActionUtils.DoInFrame(setFieldsValues);

            _autoResetLightingSettingsToggle.isOn = ModSettingsManager.GetBoolValue(ModSettingsConstants.AUTO_RESET_LIGHTING_SETTINGS);
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void ResetEnvironmentFields()
        {
            _showEnemiesToggle.isOn = true;
            _showPlayerToggle.isOn = true;
            _showHUDToggle.isOn = true;
            _showGarbageToggle.isOn = true;
        }

        private void setFieldsValues()
        {
            LightingInfo lightingInfo = AdvancedPhotoModeManager.Instance.GetEditedLightingInfo();
            if (lightingInfo == null)
            {
                _lightingInfo = default;
                return;
            }
            _lightingInfo = lightingInfo;

            _disallowCallbacks = true;

            UICinematicEffects cinematicEffects = UICinematicEffects.instance;
            if (!cinematicEffects)
            {
                _cinematicBordersHeightSlider.value = 100f;
                _cinematicBordersToggle.isOn = false;
            }
            else
            {
                _cinematicBordersHeightSlider.value = cinematicEffects.bordersHeight;
                _cinematicBordersToggle.isOn = cinematicEffects.borders;
            }

            _timeScaleSlider.value = Mathf.Round(Mathf.Clamp01(PhotoManager.Instance.OverridePausedTimeScale) * 10f);

            _vignetteToggle.isOn = AdvancedPhotoModeManager.Settings.EnableVignette;
            _ditheringToggle.isOn = AdvancedPhotoModeManager.Settings.EnableDithering;
            _ambientOcclusionToggle.isOn = AdvancedPhotoModeManager.Settings.EnableSSAO;
            _vignetteIntensitySlider.value = AdvancedPhotoModeManager.Settings.VignetteIntensity;
            _globalIlluminationToggle.isOn = AdvancedPhotoModeManager.Settings.EnableGlobalIllumination;
            _reflectionProbeToggle.isOn = AdvancedPhotoModeManager.Settings.EnableReflectionProbe;
            _caToggle.isOn = AdvancedPhotoModeManager.Settings.EnableCA;
            _sunShaftsToggle.isOn = AdvancedPhotoModeManager.Settings.EnableSunShafts;
            _dofToggle.isOn = AdvancedPhotoModeManager.Settings.EnableDoF;

            _fogToggle.isOn = lightingInfo.FogEnabled;
            _fogColor.color = lightingInfo.FogColor;
            _fogStartSlider.value = lightingInfo.FogStartDistance;
            _fogEndSlider.value = lightingInfo.FogEndDistance;

            _directionalLightToggle.isOn = lightingInfo.EnableDirectionalLight;
            _directionalLightColor.color = lightingInfo.DirectionalColor;
            _directionalLightXSlider.value = lightingInfo.DirectionalRotationX;
            _directionalLightYSlider.value = lightingInfo.DirectionalRotationY;
            _directionalLightIntensitySlider.value = lightingInfo.DirectionalIntensity;
            _directionalLightShadowsSlider.value = lightingInfo.DirectionalShadowStrength;

            _skyBoxSlider.value = lightingInfo.SkyboxIndex;

            bool hasAdditionalSkybox = !lightingInfo.AdditonalSkybox.IsNullOrEmpty();
            _realisticSkyBoxToggle.isOn = hasAdditionalSkybox;
            _realisticSkyBoxDropdownFieldObject.SetActive(hasAdditionalSkybox);
            _realisticSkyboxColor.gameObject.SetActive(hasAdditionalSkybox);
            _realisticSkyboxColor.color = lightingInfo.AdditionalSkyboxTint;
            _realisticSkyboxRotationColor.gameObject.SetActive(hasAdditionalSkybox);
            _realisticSkyboxRotationColor.value = Mathf.RoundToInt(lightingInfo.AdditionalSkyboxRotation) % 360;

            List<Dropdown.OptionData> additonalSkyboxOptions = AdditionalSkyboxesManager.Instance.GetSkyboxOptions();
            _realisticSkyBoxDropdown.options = additonalSkyboxOptions;
            for (int i = 0; i < additonalSkyboxOptions.Count; i++)
            {
                if ((additonalSkyboxOptions[i] as DropdownStringOptionData).StringValue == lightingInfo.AdditonalSkybox)
                {
                    _realisticSkyBoxDropdown.value = i;
                    break;
                }
            }

            _showHUDToggle.isOn = !CutSceneManager.Instance.IsInCutscene() && !SettingsManager.Instance.ShouldHideGameUI();

            _disallowCallbacks = false;
        }

        private void toggleRobot(FirstPersonMover firstPersonMover, bool value)
        {
            if (!firstPersonMover)
                return;

            if (value)
                firstPersonMover.ShowTemporarilyHiddenWeaponModels();
            else
                firstPersonMover.TemporarilyHideWeaponModels();

            CharacterModel characterModel = firstPersonMover.GetCharacterModel();
            if (characterModel)
            {
                if (value)
                    characterModel.ShowAllHiddenBodyPartsAndArmor();
                else
                    characterModel.HideAllBodyPartsandArmor();
            }
        }

        public void OnSaveRLightInfoButtonClicked()
        {
            RealisticLightingManager.Instance.SaveCurrentLighting();
        }

        public void OnRestoreDefaultsButtonClicked()
        {
            AdvancedPhotoModeManager.Instance.RestoreDefaults();
            UICinematicEffects cinematicEffects = UICinematicEffects.instance;
            if (cinematicEffects)
            {
                cinematicEffects.borders = false;
                cinematicEffects.bordersHeight = 100f;
            }
            setFieldsValues();
        }

        public void OnTimeScaleChanged(float value)
        {
            if (_disallowCallbacks)
                return;

            PhotoManager.Instance.OverridePausedTimeScale = Mathf.Clamp(value / 10f, 0.1f, 1f);
        }

        public void OnShowPlayerToggled(bool value)
        {
            if (_disallowCallbacks)
                return;

            toggleRobot(CharacterTracker.Instance.GetPlayerRobot(), value);
        }

        public void OnShowEnemiesToggled(bool value)
        {
            if (_disallowCallbacks)
                return;

            foreach (Character character in CharacterTracker.Instance.GetAllLivingCharacters())
            {
                if (!character || character.IsMainPlayer() || !(character is FirstPersonMover firstPersonMover))
                    continue;

                toggleRobot(firstPersonMover, value);
            }
        }

        public void OnShowGarbageToggled(bool value)
        {
            if (_disallowCallbacks)
                return;

            List<GarbageTarget> list = value ? _garbageTargets : GarbageManager.Instance.GetAllGarbageReadyForCollection();
            if (list.IsNullOrEmpty())
                return;

            if (!value)
            {
                _garbageTargets = list;

                foreach (GarbageTarget t in list)
                    if (t)
                        t.gameObject.SetActive(false);
            }
            else
            {
                foreach (GarbageTarget t in list)
                    if (t)
                        t.gameObject.SetActive(true);
            }
        }

        public void OnHUDToggled(bool value)
        {
            if (_disallowCallbacks)
                return;

            ModCache.gameUIRoot.SetPlayerHUDVisible(value && !CutSceneManager.Instance.IsInCutscene());
        }

        public void OnCinematicBordersToggled(bool value)
        {
            _cinematicBordersHeightSlider.gameObject.SetActive(value);
            if (_disallowCallbacks)
                return;

            UICinematicEffects cinematicEffects = UICinematicEffects.instance;
            if (cinematicEffects)
                cinematicEffects.borders = value;
        }

        public void OnCinematicBordersHeightChanged(float value)
        {
            if (_disallowCallbacks)
                return;

            UICinematicEffects cinematicEffects = UICinematicEffects.instance;
            if (cinematicEffects)
                cinematicEffects.bordersHeight = value;
        }

        public void OnSSAOToggled(bool value)
        {
            if (_disallowCallbacks)
                return;

            AdvancedPhotoModeManager.Settings.EnableSSAO = value;
            PostEffectsManager.Instance.RefreshCameraPostEffects();
        }

        public void OnVignetteToggled(bool value)
        {
            _vignetteIntensitySlider.gameObject.SetActive(value);
            if (_disallowCallbacks)
                return;

            AdvancedPhotoModeManager.Settings.EnableVignette = value;
            PostEffectsManager.Instance.RefreshCameraPostEffects();
        }

        public void OnVignetteIntensityChanged(float value)
        {
            if (_disallowCallbacks)
                return;

            AdvancedPhotoModeManager.Settings.VignetteIntensity = value;
            PostEffectsManager.Instance.RefreshCameraPostEffects();
        }

        public void OnDitheringToggled(bool value)
        {
            if (_disallowCallbacks)
                return;

            AdvancedPhotoModeManager.Settings.EnableDithering = value;
            PostEffectsManager.Instance.RefreshCameraPostEffects();
        }

        public void OnGlobalIlluminationToggled(bool value)
        {
            if (_disallowCallbacks)
                return;

            AdvancedPhotoModeManager.Settings.EnableGlobalIllumination = value;
            PostEffectsManager.Instance.RefreshCameraPostEffects();
        }

        public void OnReflectionProbeToggled(bool value)
        {
            if (_disallowCallbacks)
                return;

            AdvancedPhotoModeManager.Settings.EnableReflectionProbe = value;
            PostEffectsManager.Instance.RefreshCameraPostEffects();
        }

        public void OnCAToggled(bool value)
        {
            if (_disallowCallbacks)
                return;

            AdvancedPhotoModeManager.Settings.EnableCA = value;
            PostEffectsManager.Instance.RefreshCameraPostEffects();
        }

        public void OnSunShaftsToggled(bool value)
        {
            if (_disallowCallbacks)
                return;

            AdvancedPhotoModeManager.Settings.EnableSunShafts = value;
            PostEffectsManager.Instance.RefreshCameraPostEffects();
        }

        public void OnDoFToggled(bool value)
        {
            if (_disallowCallbacks)
                return;

            AdvancedPhotoModeManager.Settings.EnableDoF = value;
            PostEffectsManager.Instance.RefreshCameraPostEffects();
        }

        public void OnFogToggled(bool value)
        {
            if (_disallowCallbacks)
                return;

            _lightingInfo.FogEnabled = value;
            AdvancedPhotoModeManager.Instance.SetEditedLighting();
        }

        public void OnFogColored(Color value)
        {
            if (_disallowCallbacks)
                return;

            _lightingInfo.FogColor = value;
            AdvancedPhotoModeManager.Instance.SetEditedLighting();
        }

        public void OnFogStartChanged(float value)
        {
            if (_disallowCallbacks)
                return;

            _lightingInfo.FogStartDistance = value;
            AdvancedPhotoModeManager.Instance.SetEditedLighting();
        }

        public void OnFogEndChanged(float value)
        {
            if (_disallowCallbacks)
                return;

            _lightingInfo.FogEndDistance = value;
            AdvancedPhotoModeManager.Instance.SetEditedLighting();
        }

        public void OnDirectionalLightToggled(bool value)
        {
            if (_disallowCallbacks)
                return;

            _lightingInfo.EnableDirectionalLight = value;
            AdvancedPhotoModeManager.Instance.SetEditedLighting();
        }

        public void OnDirectionalLightColored(Color value)
        {
            if (_disallowCallbacks)
                return;

            _lightingInfo.DirectionalColor = value;
            AdvancedPhotoModeManager.Instance.SetEditedLighting();
        }

        public void OnDirectionalLightXChanged(float value)
        {
            if (_disallowCallbacks)
                return;

            _lightingInfo.DirectionalRotationX = value;
            AdvancedPhotoModeManager.Instance.SetEditedLighting();
        }

        public void OnDirectionalLightYChanged(float value)
        {
            if (_disallowCallbacks)
                return;

            _lightingInfo.DirectionalRotationY = value;
            AdvancedPhotoModeManager.Instance.SetEditedLighting();
        }

        public void OnSkyBoxIndexChanged(float value)
        {
            if (_disallowCallbacks)
                return;

            _lightingInfo.SkyboxIndex = Mathf.RoundToInt(value);
            AdvancedPhotoModeManager.Instance.SetEditedLighting();
        }

        public void OnUseRealisticSkyBoxesToggled(bool value)
        {
            if (_disallowCallbacks)
                return;

            string skyboxName = (_realisticSkyBoxDropdown.options[_realisticSkyBoxDropdown.value] as DropdownStringOptionData).StringValue;

            _lightingInfo.AdditonalSkybox = value ? skyboxName : string.Empty;
            AdvancedPhotoModeManager.Instance.SetEditedLighting();

            _realisticSkyBoxDropdownFieldObject.SetActive(value);
            _realisticSkyboxColor.gameObject.SetActive(value);
            _realisticSkyboxRotationColor.gameObject.SetActive(value);
        }

        public void OnRealisticSkyBoxDropdownChanged(int value)
        {
            if (_disallowCallbacks)
                return;

            string skyboxName = (_realisticSkyBoxDropdown.options[value] as DropdownStringOptionData).StringValue;

            _lightingInfo.AdditonalSkybox = _realisticSkyBoxToggle.isOn ? skyboxName : string.Empty;
            AdvancedPhotoModeManager.Instance.SetEditedLighting();
        }

        public void OnRealisticSkyboxTintChanged(Color value)
        {
            if (_disallowCallbacks)
                return;

            _lightingInfo.AdditionalSkyboxTint = value;
            AdvancedPhotoModeManager.Instance.SetEditedLighting();
        }


        public void OnRealisticSkyboxRotationChanged(float value)
        {
            if (_disallowCallbacks)
                return;

            _lightingInfo.AdditionalSkyboxRotation = value;
            AdvancedPhotoModeManager.Instance.SetEditedLighting();
        }

        public void OnDirectionalLightIntensityChanged(float value)
        {
            if (_disallowCallbacks)
                return;

            _lightingInfo.DirectionalIntensity = value;
            AdvancedPhotoModeManager.Instance.SetEditedLighting();
        }

        public void OnDirectionalLightShadowsChanged(float value)
        {
            if (_disallowCallbacks)
                return;

            _lightingInfo.DirectionalShadowStrength = value;
            AdvancedPhotoModeManager.Instance.SetEditedLighting();
        }

        public void OnAutoResetLightingSettingsToggleChanged(bool value)
        {
            ModSettingsManager.SetBoolValue(ModSettingsConstants.AUTO_RESET_LIGHTING_SETTINGS, value, true);
        }
    }
}
