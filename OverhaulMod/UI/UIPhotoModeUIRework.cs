using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPhotoModeUIRework : OverhaulUIBehaviour
    {
        [UIElement("ExpandButton", typeof(UIElementExpandButton))]
        private readonly UIElementExpandButton m_expandButton;

        [UIElement("LightningPanel")]
        private readonly RectTransform m_lightningPanel;

        [UIElementAction(nameof(OnSaveRLightInfoButtonClicked))]
        [UIElement("SaveRLightInfoButton")]
        private readonly Button m_saveRLightInfoButton;

        [UIElementAction(nameof(OnRestoreDefaultsButtonClicked))]
        [UIElement("RestoreDefaultsButton")]
        private readonly Button m_restoreDefaultsButton;


        [UIElementAction(nameof(OnShowPlayerToggled))]
        [UIElement("ShowPlayerToggle")]
        private readonly Toggle m_showPlayerToggle;

        [UIElementAction(nameof(OnShowEnemiesToggled))]
        [UIElement("ShowEnemiesToggle")]
        private readonly Toggle m_showEnemiesToggle;

        [UIElementAction(nameof(OnShowGarbageToggled))]
        [UIElement("ShowGarbageToggle")]
        private readonly Toggle m_showGarbageToggle;

        [UIElementAction(nameof(OnHUDToggled))]
        [UIElement("ShowHUDToggle")]
        private readonly Toggle m_showHUDToggle;


        [UIElementAction(nameof(OnCinematicBordersToggled))]
        [UIElement("CinematicBordersToggle")]
        private readonly Toggle m_cinematicBordersToggle;

        [UIElementAction(nameof(OnCinematicBordersHeightChanged))]
        [UIElement("CinematicBordersHeightSlider")]
        private readonly Slider m_cinematicBordersHeightSlider;


        [UIElementAction(nameof(OnFogToggled))]
        [UIElement("FogToggle")]
        private readonly Toggle m_fogToggle;

        [ColorPicker(false)]
        [UIElementAction(nameof(OnFogColored))]
        [UIElement("FogColor")]
        private readonly UIElementColorPickerButton m_fogColor;

        [UIElementAction(nameof(OnFogStartChanged))]
        [UIElement("FogStartSlider")]
        private readonly Slider m_fogStartSlider;

        [UIElementAction(nameof(OnFogEndChanged))]
        [UIElement("FogEndSlider")]
        private readonly Slider m_fogEndSlider;


        [UIElementAction(nameof(OnDirectionalLightToggled))]
        [UIElement("DirectionalLightToggle")]
        private readonly Toggle m_directionalLightToggle;

        [ColorPicker(false)]
        [UIElementAction(nameof(OnDirectionalLightColored))]
        [UIElement("DirectionalLightColor")]
        private readonly UIElementColorPickerButton m_directionalLightColor;

        [UIElementAction(nameof(OnDirectionalLightXChanged))]
        [UIElement("DirectionalLightX")]
        private readonly Slider m_directionalLightXSlider;

        [UIElementAction(nameof(OnDirectionalLightYChanged))]
        [UIElement("DirectionalLightY")]
        private readonly Slider m_directionalLightYSlider;

        [UIElementAction(nameof(OnDirectionalLightIntensityChanged))]
        [UIElement("DirectionalLightIntensity")]
        private readonly Slider m_directionalLightIntensitySlider;

        [UIElementAction(nameof(OnDirectionalLightShadowsChanged))]
        [UIElement("DirectionalLightShadows")]
        private readonly Slider m_directionalLightShadowsSlider;


        [UIElementAction(nameof(OnSkyBoxIndexChanged))]
        [UIElement("SkyboxSlider")]
        private readonly Slider m_skyBoxSlider;

        [UIElementAction(nameof(OnUseRealisticSkyBoxesToggled))]
        [UIElement("RealisticSkyboxToggle")]
        private readonly Toggle m_realisticSkyBoxToggle;

        [UIElementAction(nameof(OnRealisticSkyBoxIndexChanged))]
        [UIElement("RealisticSkyboxSlider")]
        private readonly Slider m_realisticSkyBoxSlider;

        private LightningInfo m_lightningInfo;

        private bool m_disallowCallbacks;

        private List<GarbageTarget> m_garbageTargets;

        protected override void OnInitialized()
        {
            RectTransform lightningPanel = m_lightningPanel;
            lightningPanel.sizeDelta = new Vector2(225f, 300f);

            UIElementExpandButton expandButton = m_expandButton;
            expandButton.rectTransform = lightningPanel;
            expandButton.collapsedSize = lightningPanel.sizeDelta;
            expandButton.expandedSize = new Vector2(375f, 300f);

            m_saveRLightInfoButton.gameObject.SetActive(ModBuildInfo.debug);
        }

        public override void Show()
        {
            base.Show();
            ModActionUtils.DoInFrame(setFieldsValues);
        }

        public override void Hide()
        {
            base.Hide();
            m_showEnemiesToggle.isOn = true;
            m_showPlayerToggle.isOn = true;
            m_showHUDToggle.isOn = true;
            m_showGarbageToggle.isOn = true;
        }

        private void setFieldsValues()
        {
            LightningInfo lightningInfo = AdvancedPhotoModeManager.Instance.GetEditedLightningInfo();
            if (lightningInfo == null)
            {
                m_lightningInfo = default;
                return;
            }
            m_lightningInfo = lightningInfo;

            m_disallowCallbacks = true;

            UICinematicEffects cinematicEffects = UICinematicEffects.instance;
            if (!cinematicEffects)
            {
                m_cinematicBordersHeightSlider.value = 100f;
                m_cinematicBordersToggle.isOn = false;
            }
            else
            {
                m_cinematicBordersHeightSlider.value = cinematicEffects.bordersHeight;
                m_cinematicBordersToggle.isOn = cinematicEffects.borders;
            }

            m_fogToggle.isOn = lightningInfo.FogEnabled;
            m_fogColor.color = lightningInfo.FogColor;
            m_fogStartSlider.value = lightningInfo.FogStartDistance;
            m_fogEndSlider.value = lightningInfo.FogEndDistance;

            m_directionalLightToggle.isOn = lightningInfo.EnableDirectionalLight;
            m_directionalLightColor.color = lightningInfo.DirectionalColor;
            m_directionalLightXSlider.value = lightningInfo.DirectionalRotationX;
            m_directionalLightYSlider.value = lightningInfo.DirectionalRotationY;
            m_directionalLightIntensitySlider.value = lightningInfo.DirectionalIntensity;
            m_directionalLightShadowsSlider.value = lightningInfo.DirectionalShadowStrength;

            m_skyBoxSlider.value = lightningInfo.SkyboxIndex;

            RealisticLightningInfo realisticLightningInfo = RealisticLightningManager.Instance.GetCurrentRealisticLightningInfo();
            if(realisticLightningInfo == null)
            {
                m_realisticSkyBoxToggle.isOn = false;
                m_realisticSkyBoxSlider.value = -1;
            }
            else
            {
                m_realisticSkyBoxToggle.isOn = realisticLightningInfo.SkyboxIndex != -1;
                m_realisticSkyBoxSlider.value = realisticLightningInfo.SkyboxIndex;
            }

            m_disallowCallbacks = false;
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
            RealisticLightningManager.Instance.SaveCurrentLightningInfo(Mathf.RoundToInt(m_realisticSkyBoxSlider.value));
        }

        public void OnRestoreDefaultsButtonClicked()
        {
            AdvancedPhotoModeManager.Instance.RestoreDefaults();
            setFieldsValues();
        }

        public void OnShowPlayerToggled(bool value)
        {
            toggleRobot(CharacterTracker.Instance.GetPlayerRobot(), value);
        }

        public void OnShowEnemiesToggled(bool value)
        {
            foreach(var character in CharacterTracker.Instance.GetAllLivingCharacters())
            {
                if (!character || character.IsMainPlayer() || !(character is FirstPersonMover firstPersonMover))
                    continue;

                toggleRobot(firstPersonMover, value);
            }
        }

        public void OnShowGarbageToggled(bool value)
        {
            var list = value ? m_garbageTargets : GarbageManager.Instance.GetAllGarbageReadyForCollection();
            if (list.IsNullOrEmpty())
                return;

            if (!value)
            {
                m_garbageTargets = list;

                foreach (var t in list)
                    if (t)
                        t.gameObject.SetActive(false);
            }
            else
            {
                foreach (var t in list)
                    if (t)
                        t.gameObject.SetActive(true);
            }
        }

        public void OnHUDToggled(bool value)
        {
            GameUIRoot.Instance.SetPlayerHUDVisible(value && !CutSceneManager.Instance.IsInCutscene());
        }

        public void OnCinematicBordersToggled(bool value)
        {
            if (m_disallowCallbacks)
                return;

            UICinematicEffects cinematicEffects = UICinematicEffects.instance;
            if (cinematicEffects)
                cinematicEffects.borders = value;
        }

        public void OnCinematicBordersHeightChanged(float value)
        {
            if (m_disallowCallbacks)
                return;

            UICinematicEffects cinematicEffects = UICinematicEffects.instance;
            if (cinematicEffects)
                cinematicEffects.bordersHeight = value;
        }

        public void OnFogToggled(bool value)
        {
            if (m_disallowCallbacks)
                return;

            m_lightningInfo.FogEnabled = value;
            AdvancedPhotoModeManager.Instance.RefreshLightningWithEditedInfo();
        }

        public void OnFogColored(Color value)
        {
            if (m_disallowCallbacks)
                return;

            m_lightningInfo.FogColor = value;
            AdvancedPhotoModeManager.Instance.RefreshLightningWithEditedInfo();
        }

        public void OnFogStartChanged(float value)
        {
            if (m_disallowCallbacks)
                return;

            m_lightningInfo.FogStartDistance = value;
            AdvancedPhotoModeManager.Instance.RefreshLightningWithEditedInfo();
        }

        public void OnFogEndChanged(float value)
        {
            if (m_disallowCallbacks)
                return;

            m_lightningInfo.FogEndDistance = value;
            AdvancedPhotoModeManager.Instance.RefreshLightningWithEditedInfo();
        }

        public void OnDirectionalLightToggled(bool value)
        {
            if (m_disallowCallbacks)
                return;

            m_lightningInfo.EnableDirectionalLight = value;
            AdvancedPhotoModeManager.Instance.RefreshLightningWithEditedInfo();
        }

        public void OnDirectionalLightColored(Color value)
        {
            if (m_disallowCallbacks)
                return;

            m_lightningInfo.DirectionalColor = value;
            AdvancedPhotoModeManager.Instance.RefreshLightningWithEditedInfo();
        }

        public void OnDirectionalLightXChanged(float value)
        {
            if (m_disallowCallbacks)
                return;

            m_lightningInfo.DirectionalRotationX = value;
            AdvancedPhotoModeManager.Instance.RefreshLightningWithEditedInfo();
        }

        public void OnDirectionalLightYChanged(float value)
        {
            if (m_disallowCallbacks)
                return;

            m_lightningInfo.DirectionalRotationY = value;
            AdvancedPhotoModeManager.Instance.RefreshLightningWithEditedInfo();
        }

        public void OnSkyBoxIndexChanged(float value)
        {
            if (m_disallowCallbacks)
                return;

            m_lightningInfo.SkyboxIndex = Mathf.RoundToInt(value);
            AdvancedPhotoModeManager.Instance.RefreshLightningWithEditedInfo();
        }

        public void OnUseRealisticSkyBoxesToggled(bool value)
        {
            if (m_disallowCallbacks)
                return;

            m_lightningInfo.RealisticSkyboxIndex = value ? Mathf.RoundToInt(m_realisticSkyBoxSlider.value) : -1;
            AdvancedPhotoModeManager.Instance.RefreshLightningWithEditedInfo();
        }

        public void OnRealisticSkyBoxIndexChanged(float value)
        {
            if (m_disallowCallbacks)
                return;

            m_lightningInfo.RealisticSkyboxIndex = m_realisticSkyBoxToggle.isOn ? Mathf.RoundToInt(value) : -1;
            AdvancedPhotoModeManager.Instance.RefreshLightningWithEditedInfo();
        }

        public void OnDirectionalLightIntensityChanged(float value)
        {
            if (m_disallowCallbacks)
                return;

            m_lightningInfo.DirectionalIntensity = value;
            AdvancedPhotoModeManager.Instance.RefreshLightningWithEditedInfo();
        }

        public void OnDirectionalLightShadowsChanged(float value)
        {
            if (m_disallowCallbacks)
                return;

            m_lightningInfo.DirectionalShadowStrength = value;
            AdvancedPhotoModeManager.Instance.RefreshLightningWithEditedInfo();
        }
    }
}
