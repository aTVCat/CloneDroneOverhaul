using OverhaulMod.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIGenericColorPicker : OverhaulUIBehaviour
    {
        private static bool s_open;

        [UIElement("Panel", typeof(DraggablePanel))]
        private readonly GameObject _panelObject;

        [UIElementAction(nameof(OnCloseButtonClicked))]
        [UIElement("CloseButton")]
        private readonly Button _closeButton;

        [UIElement("RGBASettings")]
        private readonly ModdedObject _rgbSettingsTab;

        [UIElement("HSVSettings")]
        private readonly ModdedObject _hsvSettingsTab;

        [UIElementAction(nameof(OnHexCodeFieldChanged))]
        [UIElement("HexInputField")]
        private readonly InputField _hexCodeField;

        [UIElement("RGBSettingsHolder")]
        private readonly GameObject _rgbSettingsHolderObject;

        [UIElement("HSVSettingsHolder")]
        private readonly GameObject _hsvSettingsHolderObject;

        [UIElementAction(nameof(OnRGBColorRChannelSliderChanged))]
        [UIElement("RGB_RChannelSlider")]
        private readonly Slider _rgbColorRChannelSlider;
        [UIElementAction(nameof(OnRGBColorGChannelSliderChanged))]
        [UIElement("RGB_GChannelSlider")]
        private readonly Slider _rgbColorGChannelSlider;
        [UIElementAction(nameof(OnRGBColorBChannelSliderChanged))]
        [UIElement("RGB_BChannelSlider")]
        private readonly Slider _rgbColorBChannelSlider;
        [UIElementAction(nameof(OnRGBColorAChannelSliderChanged))]
        [UIElement("RGB_AChannelSlider")]
        private readonly Slider _rgbColorAChannelSlider;

        [UIElementAction(nameof(OnHSVColorHChannelSliderChanged))]
        [UIElement("HSV_HChannelSlider")]
        private readonly Slider _hsvColorHChannelSlider;
        [UIElementAction(nameof(OnHSVColorSChannelSliderChanged))]
        [UIElement("HSV_SChannelSlider")]
        private readonly Slider _hsvColorSChannelSlider;
        [UIElementAction(nameof(OnHSVColorVChannelSliderChanged))]
        [UIElement("HSV_VChannelSlider")]
        private readonly Slider _hsvColorVChannelSlider;
        [UIElementAction(nameof(OnHSVColorAChannelSliderChanged))]
        [UIElement("HSV_AChannelSlider")]
        private readonly Slider _hsvColorAChannelSlider;

        [TabManager(typeof(UIElementTab), null, null, null, nameof(OnTabSelected))]
        private readonly TabManager _tabs;

        private bool _disallowHexCodeFieldCallbacks, _disallowSliderFieldCallbacks;

        private Color _outputColor;
        public Color outputColor
        {
            get
            {
                return _outputColor;
            }
            set
            {
                _disallowSliderFieldCallbacks = true;
                _rgbColorRChannelSlider.value = value.r;
                _rgbColorGChannelSlider.value = value.g;
                _rgbColorBChannelSlider.value = value.b;
                _rgbColorAChannelSlider.value = showAlphaChannel ? value.a : 1f;

                Color.RGBToHSV(value, out float h, out float s, out float v);
                _hsvColorHChannelSlider.value = h;
                _hsvColorSChannelSlider.value = s;
                _hsvColorVChannelSlider.value = v;
                _hsvColorAChannelSlider.value = showAlphaChannel ? value.a : 1f;
                _disallowSliderFieldCallbacks = false;

                _outputColor = value;
            }
        }

        private float _outputAlpha;
        public float outputAlpha
        {
            get
            {
                return _outputAlpha;
            }
            set
            {
                _disallowSliderFieldCallbacks = true;
                _rgbColorAChannelSlider.value = showAlphaChannel ? value : 1f;
                _hsvColorAChannelSlider.value = showAlphaChannel ? value : 1f;
                _disallowSliderFieldCallbacks = false;
                _outputAlpha = value;
            }
        }

        private bool _showAlphaChannel;
        public bool showAlphaChannel
        {
            get
            {
                return _showAlphaChannel;
            }
            set
            {
                _disallowSliderFieldCallbacks = true;
                _hsvColorAChannelSlider.gameObject.SetActive(value);
                _hsvColorAChannelSlider.value = 1f;
                _rgbColorAChannelSlider.gameObject.SetActive(value);
                _rgbColorAChannelSlider.value = 1f;
                _disallowSliderFieldCallbacks = false;
                _showAlphaChannel = value;
            }
        }

        public Action<Color> callback { get; set; }

        public override bool EnableCursor => true;

        protected override void OnInitialized()
        {
            _tabs.AddTab(_rgbSettingsTab.gameObject, "rgb");
            _tabs.AddTab(_hsvSettingsTab.gameObject, "hsv");
            _tabs.SelectTab("rgb");
        }

        public override void Show()
        {
            base.Show();
            s_open = true;
        }

        public override void Hide()
        {
            base.Hide();
            callback = null;
            s_open = false;
        }

        public void OnTabSelected(UIElementTab elementTab)
        {
            bool rgb = elementTab.tabId == "rgb";

            UIElementTab oldTab = _tabs.PreviousSelectedTab;
            UIElementTab newTab = _tabs.SelectedTab;
            if (oldTab)
            {
                RectTransform rt = oldTab.transform as RectTransform;
                Vector2 vector = rt.sizeDelta;
                vector.y = 25f;
                rt.sizeDelta = vector;
            }
            if (newTab)
            {
                RectTransform rt = newTab.transform as RectTransform;
                Vector2 vector = rt.sizeDelta;
                vector.y = 30f;
                rt.sizeDelta = vector;
            }

            _rgbSettingsHolderObject.SetActive(rgb);
            _hsvSettingsHolderObject.SetActive(!rgb);
        }

        public void Populate(Color currentColor, bool useAlphaChannel, Action<Color> onColorChanged)
        {
            callback = onColorChanged;
            showAlphaChannel = useAlphaChannel;
            outputColor = currentColor;
            outputAlpha = useAlphaChannel ? currentColor.a : 1f;
            RefreshHexCodeField();
        }

        public void InvokeCallback()
        {
            callback?.Invoke(outputColor);
        }

        public void RefreshHexCodeField()
        {
            _disallowHexCodeFieldCallbacks = true;
            _hexCodeField.text = "#" + (showAlphaChannel ? ColorUtility.ToHtmlStringRGBA(outputColor) : ColorUtility.ToHtmlStringRGB(outputColor));
            _disallowHexCodeFieldCallbacks = false;
        }

        public void OnCloseButtonClicked()
        {
            Hide();
        }

        public void OnHexCodeFieldChanged(string value)
        {
            if (_disallowHexCodeFieldCallbacks)
                return;

            outputColor = ModParseUtils.TryParseColor(value, Color.white);
            InvokeCallback();
        }

        public void OnRGBColorRChannelSliderChanged(float value)
        {
            if (_disallowSliderFieldCallbacks)
                return;

            Color color = outputColor;
            color.r = value;
            outputColor = color;
            InvokeCallback();
            RefreshHexCodeField();
        }

        public void OnRGBColorGChannelSliderChanged(float value)
        {
            if (_disallowSliderFieldCallbacks)
                return;

            Color color = outputColor;
            color.g = value;
            outputColor = color;
            InvokeCallback();
            RefreshHexCodeField();
        }

        public void OnRGBColorBChannelSliderChanged(float value)
        {
            if (_disallowSliderFieldCallbacks)
                return;

            Color color = outputColor;
            color.b = value;
            outputColor = color;
            InvokeCallback();
            RefreshHexCodeField();
        }

        public void OnRGBColorAChannelSliderChanged(float value)
        {
            if (_disallowSliderFieldCallbacks)
                return;

            Color color = outputColor;
            color.a = value;
            outputColor = color;
            outputAlpha = value;
            InvokeCallback();
            RefreshHexCodeField();
        }

        public void OnHSVColorHChannelSliderChanged(float value)
        {
            if (_disallowSliderFieldCallbacks)
                return;
            Color.RGBToHSV(outputColor, out _, out float s, out float v);
            float h = value;
            if (v == 0f)
                v = 0.01f;

            Color color = Color.HSVToRGB(h, s, v);
            color.a = outputAlpha;
            outputColor = color;
            InvokeCallback();
            RefreshHexCodeField();
        }

        public void OnHSVColorSChannelSliderChanged(float value)
        {
            if (_disallowSliderFieldCallbacks)
                return;
            Color.RGBToHSV(outputColor, out float h, out _, out float v);
            float s = value;
            if (v == 0f)
                v = 0.01f;

            Color color = Color.HSVToRGB(h, s, v);
            color.a = outputAlpha;
            outputColor = color;
            InvokeCallback();
            RefreshHexCodeField();
        }

        public void OnHSVColorVChannelSliderChanged(float value)
        {
            if (_disallowSliderFieldCallbacks)
                return;
            Color.RGBToHSV(outputColor, out float h, out float s, out _);
            float v = value;
            Color color = Color.HSVToRGB(h, s, v);
            color.a = outputAlpha;
            outputColor = color;
            InvokeCallback();
            RefreshHexCodeField();
        }

        public void OnHSVColorAChannelSliderChanged(float value)
        {
            if (_disallowSliderFieldCallbacks)
                return;

            Color color = outputColor;
            color.a = value;
            outputColor = color;
            outputAlpha = value;
            InvokeCallback();
            RefreshHexCodeField();
        }

        public static bool IsOpen()
        {
            return s_open;
        }
    }
}
