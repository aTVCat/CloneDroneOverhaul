using OverhaulMod.Engine;
using OverhaulMod.Utils;
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

        protected override void OnInitialized()
        {
            RectTransform lightningPanel = m_lightningPanel;
            lightningPanel.sizeDelta = new Vector2(225f, 300f);

            UIElementExpandButton expandButton = m_expandButton;
            expandButton.rectTransform = lightningPanel;
            expandButton.collapsedSize = lightningPanel.sizeDelta;
            expandButton.expandedSize = new Vector2(375f, 300f);
        }

        public override void Show()
        {
            base.Show();
            ModActionUtils.DoNextFrame(setFieldsValues);
        }

        private void setFieldsValues()
        {
            AdvancedPhotoModeManager.Instance.SetDefaultValues();
            m_fogToggle.isOn = AdvancedPhotoModeManager.Instance.GetPropertyModdedValue<bool>(typeof(RenderSettings), nameof(RenderSettings.fog));
            m_fogColor.color = AdvancedPhotoModeManager.Instance.GetPropertyModdedValue<Color>(typeof(RenderSettings), nameof(RenderSettings.fogColor));
            m_fogStartSlider.value = AdvancedPhotoModeManager.Instance.GetPropertyModdedValue<float>(typeof(RenderSettings), nameof(RenderSettings.fogStartDistance));
            m_fogEndSlider.value = AdvancedPhotoModeManager.Instance.GetPropertyModdedValue<float>(typeof(RenderSettings), nameof(RenderSettings.fogEndDistance));

            m_directionalLightToggle.isOn = AdvancedPhotoModeManager.Instance.GetPropertyModdedValue<bool>(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.directionalLight));
            m_directionalLightColor.color = AdvancedPhotoModeManager.Instance.GetPropertyModdedValue<Color>(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.directionalLightColor));
            m_directionalLightXSlider.value = AdvancedPhotoModeManager.Instance.GetPropertyModdedValue<float>(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.directionalLightX));
            m_directionalLightYSlider.value = AdvancedPhotoModeManager.Instance.GetPropertyModdedValue<float>(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.directionalLightY));
            m_directionalLightIntensitySlider.value = AdvancedPhotoModeManager.Instance.GetPropertyModdedValue<float>(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.directionalLightIntensity));
            m_directionalLightShadowsSlider.value = AdvancedPhotoModeManager.Instance.GetPropertyModdedValue<float>(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.directionalLightShadows));

            m_skyBoxSlider.value = AdvancedPhotoModeManager.Instance.GetPropertyModdedValue<int>(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.skyBoxIndex));
            m_realisticSkyBoxToggle.isOn = AdvancedPhotoModeManager.Instance.GetPropertyModdedValue<bool>(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.useRealisticSkyBoxes));
            m_realisticSkyBoxSlider.value = AdvancedPhotoModeManager.Instance.GetPropertyModdedValue<int>(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.realisticSkyBoxIndex));
        }

        public void OnSaveRLightInfoButtonClicked()
        {
            RealisticLightningManager.Instance.SaveCurrentLightningInfo(Mathf.RoundToInt(m_realisticSkyBoxSlider.value));
        }

        public void OnFogToggled(bool value)
        {
            AdvancedPhotoModeManager.Instance.SetPropertyValue(typeof(RenderSettings), nameof(RenderSettings.fog), value);
        }

        public void OnFogColored(Color value)
        {
            AdvancedPhotoModeManager.Instance.SetPropertyValue(typeof(RenderSettings), nameof(RenderSettings.fogColor), value);
        }

        public void OnFogStartChanged(float value)
        {
            AdvancedPhotoModeManager.Instance.SetPropertyValue(typeof(RenderSettings), nameof(RenderSettings.fogStartDistance), value);
        }

        public void OnFogEndChanged(float value)
        {
            AdvancedPhotoModeManager.Instance.SetPropertyValue(typeof(RenderSettings), nameof(RenderSettings.fogEndDistance), value);
        }

        public void OnDirectionalLightToggled(bool value)
        {
            AdvancedPhotoModeManager.Instance.SetPropertyValue(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.directionalLight), value);
        }

        public void OnDirectionalLightColored(Color value)
        {
            AdvancedPhotoModeManager.Instance.SetPropertyValue(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.directionalLightColor), value);
        }

        public void OnDirectionalLightXChanged(float value)
        {
            AdvancedPhotoModeManager.Instance.SetPropertyValue(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.directionalLightX), value);
        }

        public void OnDirectionalLightYChanged(float value)
        {
            AdvancedPhotoModeManager.Instance.SetPropertyValue(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.directionalLightY), value);
        }

        public void OnSkyBoxIndexChanged(float value)
        {
            AdvancedPhotoModeManager.Instance.SetPropertyValue(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.skyBoxIndex), Mathf.RoundToInt(value));
        }

        public void OnUseRealisticSkyBoxesToggled(bool value)
        {
            AdvancedPhotoModeManager.Instance.SetPropertyValue(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.useRealisticSkyBoxes), value);
        }

        public void OnRealisticSkyBoxIndexChanged(float value)
        {
            AdvancedPhotoModeManager.Instance.SetPropertyValue(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.realisticSkyBoxIndex), Mathf.RoundToInt(value));
        }

        public void OnDirectionalLightIntensityChanged(float value)
        {
            AdvancedPhotoModeManager.Instance.SetPropertyValue(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.directionalLightIntensity), value);
        }

        public void OnDirectionalLightShadowsChanged(float value)
        {
            AdvancedPhotoModeManager.Instance.SetPropertyValue(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.directionalLightShadows), value);
        }
    }
}
