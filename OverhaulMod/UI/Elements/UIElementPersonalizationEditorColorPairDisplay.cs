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
        private readonly Graphic _colorAGraphic;

        [UIElement("ColorAGraphicAlphaText")]
        private readonly Text _colorAAlphaText;

        [UIElement("ColorBGraphic")]
        private readonly Graphic _colorBGraphic;

        [UIElement("ColorBGraphicAlphaText")]
        private readonly Text _colorBAlphaText;

        [UIElementAction(nameof(OnColorBButtonClicked))]
        [UIElement("ColorBGraphic")]
        private readonly Button _colorBButton;

        [UIElementAction(nameof(OnResetButtonClicked))]
        [UIElement("ResetButton")]
        private readonly Button _resetButton;

        [UIElementAction(nameof(OnApplyFavColorToggleChanged))]
        [UIElement("ApplyFavColorToggle")]
        private readonly Toggle _applyFavoriteColorToggle;

        [UIElementAction(nameof(OnFavoriteColorSaturationSliderChanged))]
        [UIElement("FavColorSaturationSlider")]
        private readonly Slider _favoriteColorSaturationSlider;

        [UIElementAction(nameof(OnFavoriteColorBrightnessSliderChanged))]
        [UIElement("FavColorBrightnessSlider")]
        private readonly Slider _favoriteColorBrightnessSlider;

        [UIElementAction(nameof(OnFavoriteColorGlowPercentSliderChanged))]
        [UIElement("FavColorGlowPercentSlider")]
        private readonly Slider _favoriteColorGlowPercentSlider;

        private bool _disableCallbacks;

        private bool _disableColorPairUpdates;

        private ColorPairFloat _colorPair = new ColorPairFloat();
        public ColorPairFloat colorPair
        {
            get
            {
                return _colorPair;
            }
            set
            {
                _disableColorPairUpdates = true;
                _disableCallbacks = true;
                colorA = value.ColorA;
                colorB = value.ColorB;
                _disableCallbacks = false;
                _disableColorPairUpdates = false;

                _colorPair = value;

                if (!_disableCallbacks)
                    OnValueChanged.Invoke(returnNewPair ? new ColorPairFloat(value.ColorA, value.ColorB) : value);
            }
        }

        private Dictionary<string, FavoriteColorSettings> _favoriteColorSettings;
        public Dictionary<string, FavoriteColorSettings> favoriteColorSettings
        {
            get
            {
                return _favoriteColorSettings;
            }
            set
            {
                _favoriteColorSettings = value;

                if (value == null)
                    return;

                if (value.TryGetValue(ColorUtility.ToHtmlStringRGBA(colorA), out FavoriteColorSettings favoriteColorSettings))
                {
                    _disableCallbacks = true;
                    _applyFavoriteColorToggle.isOn = true;
                    _favoriteColorSaturationSlider.value = Mathf.Clamp(favoriteColorSettings.SaturationMultiplier * 100f, 0f, 100f);
                    _favoriteColorBrightnessSlider.value = Mathf.Clamp(favoriteColorSettings.BrightnessMultiplier * 100f, 0f, 100f);
                    _favoriteColorGlowPercentSlider.value = Mathf.Clamp(favoriteColorSettings.GlowPercent * 100f, 0f, 100f);
                    _favoriteColorSaturationSlider.interactable = true;
                    _favoriteColorBrightnessSlider.interactable = true;
                    _favoriteColorGlowPercentSlider.interactable = true;
                    _disableCallbacks = false;
                }
                else
                {
                    _applyFavoriteColorToggle.isOn = false;
                    _favoriteColorSaturationSlider.interactable = false;
                    _favoriteColorBrightnessSlider.interactable = false;
                    _favoriteColorGlowPercentSlider.interactable = false;
                }
            }
        }

        private Color _colorA;
        public Color colorA
        {
            get
            {
                return _colorA;
            }
            set
            {
                Color graphicColor = new Color(value.r, value.g, value.b, 1f);
                _colorAGraphic.color = graphicColor;

                _colorAAlphaText.text = $"{Mathf.Round((1f - value.a) * 100f)}%";
                _colorA = value;

                if (!_disableColorPairUpdates)
                    _colorPair.ColorA = value;

                if (!_disableCallbacks)
                    OnValueChanged.Invoke(returnNewPair ? new ColorPairFloat(value, colorB) : _colorPair);
            }
        }

        private Color _colorB;
        public Color colorB
        {
            get
            {
                return _colorB;
            }
            set
            {
                Color graphicColor = new Color(value.r, value.g, value.b, 1f);
                _colorBGraphic.color = graphicColor;

                _colorBAlphaText.text = $"{Mathf.Round((1f - value.a) * 100f)}%";
                _colorB = value;

                if (!_disableColorPairUpdates)
                    _colorPair.ColorB = value;

                if (!_disableCallbacks)
                    OnValueChanged.Invoke(returnNewPair ? new ColorPairFloat(colorA, value) : _colorPair);
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

        public ColorPairChangedEvent OnValueChanged { get; set; } = new ColorPairChangedEvent();

        public UnityEvent OnFavoriteColorSettingsChanged { get; set; } = new UnityEvent();

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
            if (_disableCallbacks || _favoriteColorSettings == null)
                return;

            string hex = ColorUtility.ToHtmlStringRGBA(colorA);
            if (value)
            {
                if (!_favoriteColorSettings.ContainsKey(hex))
                    _favoriteColorSettings.Add(hex, new FavoriteColorSettings(_favoriteColorSaturationSlider.value / 100f, _favoriteColorBrightnessSlider.value / 100f, _favoriteColorGlowPercentSlider.value / 100f));
            }
            else
            {
                _ = _favoriteColorSettings.Remove(hex);
            }
            OnFavoriteColorSettingsChanged.Invoke();

            _favoriteColorSaturationSlider.interactable = value;
            _favoriteColorBrightnessSlider.interactable = value;
            _favoriteColorGlowPercentSlider.interactable = value;
        }

        public void OnFavoriteColorSaturationSliderChanged(float value)
        {
            if (_disableCallbacks || _favoriteColorSettings == null)
                return;

            string hex = ColorUtility.ToHtmlStringRGBA(colorA);
            if (_favoriteColorSettings.ContainsKey(hex))
                _favoriteColorSettings[hex].SaturationMultiplier = value / 100f;

            ModUIUtils.Tooltip($"{value}%");

            OnFavoriteColorSettingsChanged.Invoke();
        }

        public void OnFavoriteColorBrightnessSliderChanged(float value)
        {
            if (_disableCallbacks || _favoriteColorSettings == null)
                return;

            string hex = ColorUtility.ToHtmlStringRGBA(colorA);
            if (_favoriteColorSettings.ContainsKey(hex))
                _favoriteColorSettings[hex].BrightnessMultiplier = value / 100f;

            ModUIUtils.Tooltip($"{value}%");

            OnFavoriteColorSettingsChanged.Invoke();
        }

        public void OnFavoriteColorGlowPercentSliderChanged(float value)
        {
            if (_disableCallbacks || _favoriteColorSettings == null)
                return;

            string hex = ColorUtility.ToHtmlStringRGBA(colorA);
            if (_favoriteColorSettings.ContainsKey(hex))
                _favoriteColorSettings[hex].GlowPercent = value / 100f;

            ModUIUtils.Tooltip($"{value}%");

            OnFavoriteColorSettingsChanged.Invoke();
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
