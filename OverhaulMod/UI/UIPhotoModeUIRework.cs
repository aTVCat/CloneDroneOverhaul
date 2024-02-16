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


        [UIElementAction(nameof(OnFogToggled))]
        [UIElement("FogToggle")]
        private readonly Toggle m_fogToggle;

        [UIElement("FogColor", typeof(UIElementColorPickerButton))]
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

        [UIElement("DirectionalLightColor", typeof(UIElementColorPickerButton))]
        private readonly UIElementColorPickerButton m_directionalLightColor;

        [UIElementAction(nameof(OnDirectionalLightXChanged))]
        [UIElement("DirectionalLightX")]
        private readonly Slider m_directionalLightXSlider;

        [UIElementAction(nameof(OnDirectionalLightYChanged))]
        [UIElement("DirectionalLightY")]
        private readonly Slider m_directionalLightYSlider;

        protected override void OnInitialized()
        {
            RectTransform lightningPanel = m_lightningPanel;
            lightningPanel.sizeDelta = new Vector2(200f, 300f);

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
            m_fogStartSlider.value = AdvancedPhotoModeManager.Instance.GetPropertyModdedValue<float>(typeof(RenderSettings), nameof(RenderSettings.fogStartDistance));
            m_fogEndSlider.value = AdvancedPhotoModeManager.Instance.GetPropertyModdedValue<float>(typeof(RenderSettings), nameof(RenderSettings.fogEndDistance));

            m_directionalLightToggle.isOn = AdvancedPhotoModeManager.Instance.GetPropertyModdedValue<bool>(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.directionalLight));
            m_directionalLightXSlider.value = AdvancedPhotoModeManager.Instance.GetPropertyModdedValue<float>(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.directionalLightX));
            m_directionalLightYSlider.value = AdvancedPhotoModeManager.Instance.GetPropertyModdedValue<float>(typeof(AdvancedPhotoModeManager), nameof(AdvancedPhotoModeManager.directionalLightY));
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
    }
}
