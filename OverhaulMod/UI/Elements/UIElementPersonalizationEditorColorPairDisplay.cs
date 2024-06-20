using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationEditorColorPairDisplay : OverhaulUIBehaviour
    {
        [UIElement("ColorAGraphic")]
        private readonly Graphic m_colorAGraphic;

        [UIElement("ColorAGraphicAlphaText")]
        private readonly Text m_colorAAlphaText;

        [UIElement("ColorBGraphic")]
        private readonly Graphic m_colorBGraphic;

        [UIElement("ColorBGraphicAlphaText")]
        private readonly Text m_colorBAlphaText;

        [UIElementAction(nameof(OnColorBButtonClicked))]
        [UIElement("ColorBGraphic")]
        private readonly Button m_colorBButton;

        [UIElementAction(nameof(OnResetButtonClicked))]
        [UIElement("ResetButton")]
        private readonly Button m_resetButton;

        [UIElementAction(nameof(OnApplyFavColorToggleChanged))]
        [UIElement("ApplyFavColorToggle")]
        private readonly Toggle m_applyFavoriteColorToggle;

        [UIElementAction(nameof(OnFavoriteColorSaturationSliderChanged))]
        [UIElement("FavColorSaturationSlider")]
        private readonly Slider m_favoriteColorSaturationSlider;

        [UIElementAction(nameof(OnFavoriteColorBrightnessSliderChanged))]
        [UIElement("FavColorBrightnessSlider")]
        private readonly Slider m_favoriteColorBrightnessSlider;

        [UIElementAction(nameof(OnFavoriteColorGlowPercentSliderChanged))]
        [UIElement("FavColorGlowPercentSlider")]
        private readonly Slider m_favoriteColorGlowPercentSlider;

        private bool m_disableCallbacks;

        private bool m_disableColorPairUpdates;

        private ColorPairFloat m_colorPair = new ColorPairFloat();
        public ColorPairFloat colorPair
        {
            get
            {
                return m_colorPair;
            }
            set
            {
                m_disableColorPairUpdates = true;
                m_disableCallbacks = true;
                colorA = value.ColorA;
                colorB = value.ColorB;
                m_disableCallbacks = false;
                m_disableColorPairUpdates = false;

                m_colorPair = value;

                if (!m_disableCallbacks)
                    onValueChanged.Invoke(returnNewPair ? new ColorPairFloat(value.ColorA, value.ColorB) : value);
            }
        }

        private Dictionary<string, FavoriteColorSettings> m_favoriteColorSettings;
        public Dictionary<string, FavoriteColorSettings> favoriteColorSettings
        {
            get
            {
                return m_favoriteColorSettings;
            }
            set
            {
                m_favoriteColorSettings = value;

                if (value == null)
                    return;

                if (value.TryGetValue(ColorUtility.ToHtmlStringRGBA(colorA), out FavoriteColorSettings favoriteColorSettings))
                {
                    m_disableCallbacks = true;
                    m_applyFavoriteColorToggle.isOn = true;
                    m_favoriteColorSaturationSlider.value = Mathf.Clamp(favoriteColorSettings.SaturationMultiplier * 100f, 0f, 100f);
                    m_favoriteColorBrightnessSlider.value = Mathf.Clamp(favoriteColorSettings.BrightnessMultiplier * 100f, 0f, 100f);
                    m_favoriteColorGlowPercentSlider.value = Mathf.Clamp(favoriteColorSettings.GlowPercent * 100f, 0f, 100f);
                    m_favoriteColorSaturationSlider.interactable = true;
                    m_favoriteColorBrightnessSlider.interactable = true;
                    m_favoriteColorGlowPercentSlider.interactable = true;
                    m_disableCallbacks = false;
                }
                else
                {
                    m_applyFavoriteColorToggle.isOn = false;
                    m_favoriteColorSaturationSlider.interactable = false;
                    m_favoriteColorBrightnessSlider.interactable = false;
                    m_favoriteColorGlowPercentSlider.interactable = false;
                }
            }
        }

        private Color m_colorA;
        public Color colorA
        {
            get
            {
                return m_colorA;
            }
            set
            {
                Color graphicColor = new Color(value.r, value.g, value.b, 1f);
                m_colorAGraphic.color = graphicColor;

                m_colorAAlphaText.text = $"{Mathf.Round((1f - value.a) * 100f)}%";
                m_colorA = value;

                if (!m_disableColorPairUpdates)
                    m_colorPair.ColorA = value;

                if (!m_disableCallbacks)
                    onValueChanged.Invoke(returnNewPair ? new ColorPairFloat(value, colorB) : m_colorPair);
            }
        }

        private Color m_colorB;
        public Color colorB
        {
            get
            {
                return m_colorB;
            }
            set
            {
                Color graphicColor = new Color(value.r, value.g, value.b, 1f);
                m_colorBGraphic.color = graphicColor;

                m_colorBAlphaText.text = $"{Mathf.Round((1f - value.a) * 100f)}%";
                m_colorB = value;

                if (!m_disableColorPairUpdates)
                    m_colorPair.ColorB = value;

                if (!m_disableCallbacks)
                    onValueChanged.Invoke(returnNewPair ? new ColorPairFloat(colorA, value) : m_colorPair);
            }
        }

        public bool returnNewPair
        {
            get;
            set;
        }

        public Transform colorPickerTransform
        {
            get;
            set;
        }

        public ColorPairChangedEvent onValueChanged { get; set; } = new ColorPairChangedEvent();

        public UnityEvent onFavoriteColorSettingsChanged { get; set; } = new UnityEvent();

        public void OnColorBButtonClicked()
        {
            ModUIUtils.ColorPicker(colorB, true, onColorBChanged, colorPickerTransform);
        }

        public void OnResetButtonClicked()
        {
            colorB = colorA;
        }

        public void OnApplyFavColorToggleChanged(bool value)
        {
            if (m_disableCallbacks || m_favoriteColorSettings == null)
                return;

            string hex = ColorUtility.ToHtmlStringRGBA(colorA);
            if (value)
            {
                if (!m_favoriteColorSettings.ContainsKey(hex))
                    m_favoriteColorSettings.Add(hex, new FavoriteColorSettings(m_favoriteColorSaturationSlider.value / 100f, m_favoriteColorBrightnessSlider.value / 100f, m_favoriteColorGlowPercentSlider.value / 100f));
            }
            else
            {
                _ = m_favoriteColorSettings.Remove(hex);
            }
            onFavoriteColorSettingsChanged.Invoke();

            m_favoriteColorSaturationSlider.interactable = value;
            m_favoriteColorBrightnessSlider.interactable = value;
            m_favoriteColorGlowPercentSlider.interactable = value;
        }

        public void OnFavoriteColorSaturationSliderChanged(float value)
        {
            if (m_disableCallbacks || m_favoriteColorSettings == null)
                return;

            string hex = ColorUtility.ToHtmlStringRGBA(colorA);
            if (m_favoriteColorSettings.ContainsKey(hex))
                m_favoriteColorSettings[hex].SaturationMultiplier = value / 100f;

            onFavoriteColorSettingsChanged.Invoke();
        }

        public void OnFavoriteColorBrightnessSliderChanged(float value)
        {
            if (m_disableCallbacks || m_favoriteColorSettings == null)
                return;

            string hex = ColorUtility.ToHtmlStringRGBA(colorA);
            if (m_favoriteColorSettings.ContainsKey(hex))
                m_favoriteColorSettings[hex].BrightnessMultiplier = value / 100f;

            onFavoriteColorSettingsChanged.Invoke();
        }

        public void OnFavoriteColorGlowPercentSliderChanged(float value)
        {
            if (m_disableCallbacks || m_favoriteColorSettings == null)
                return;

            string hex = ColorUtility.ToHtmlStringRGBA(colorA);
            if (m_favoriteColorSettings.ContainsKey(hex))
                m_favoriteColorSettings[hex].GlowPercent = value / 100f;

            onFavoriteColorSettingsChanged.Invoke();
        }

        private void onColorBChanged(Color color)
        {
            colorB = color;
        }

        public class ColorPairChangedEvent : UnityEvent<ColorPairFloat>
        {

        }
    }
}
