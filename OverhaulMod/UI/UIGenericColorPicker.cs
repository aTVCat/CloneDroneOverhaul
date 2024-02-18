using OverhaulMod.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIGenericColorPicker : OverhaulUIBehaviour
    {
        [UIElement("Panel", typeof(DraggablePanel))]
        private readonly GameObject m_panelObject;

        [UIElementAction(nameof(OnCloseButtonClicked))]
        [UIElement("CloseButton")]
        private readonly Button m_closeButton;

        [UIElement("RGBASettings")]
        private readonly ModdedObject m_rgbSettingsTab;

        [UIElement("HSVSettings")]
        private readonly ModdedObject m_hsvSettingsTab;

        [UIElementAction(nameof(OnHexCodeFieldChanged))]
        [UIElement("HexInputField")]
        private readonly InputField m_hexCodeField;

        [UIElement("RGBSettingsHolder")]
        private readonly GameObject m_rgbSettingsHolderObject;

        [UIElement("HSVSettingsHolder")]
        private readonly GameObject m_hsvSettingsHolderObject;

        [UIElementAction(nameof(OnRGBColorRChannelSliderChanged))]
        [UIElement("RGB_RChannelSlider")]
        private readonly Slider m_rgbColorRChannelSlider;
        [UIElementAction(nameof(OnRGBColorGChannelSliderChanged))]
        [UIElement("RGB_GChannelSlider")]
        private readonly Slider m_rgbColorGChannelSlider;
        [UIElementAction(nameof(OnRGBColorBChannelSliderChanged))]
        [UIElement("RGB_BChannelSlider")]
        private readonly Slider m_rgbColorBChannelSlider;
        [UIElementAction(nameof(OnRGBColorAChannelSliderChanged))]
        [UIElement("RGB_AChannelSlider")]
        private readonly Slider m_rgbColorAChannelSlider;

        [UIElementAction(nameof(OnHSVColorHChannelSliderChanged))]
        [UIElement("HSV_HChannelSlider")]
        private readonly Slider m_hsvColorHChannelSlider;
        [UIElementAction(nameof(OnHSVColorSChannelSliderChanged))]
        [UIElement("HSV_SChannelSlider")]
        private readonly Slider m_hsvColorSChannelSlider;
        [UIElementAction(nameof(OnHSVColorVChannelSliderChanged))]
        [UIElement("HSV_VChannelSlider")]
        private readonly Slider m_hsvColorVChannelSlider;
        [UIElementAction(nameof(OnHSVColorAChannelSliderChanged))]
        [UIElement("HSV_AChannelSlider")]
        private readonly Slider m_hsvColorAChannelSlider;

        [TabManager(typeof(UIElementTab), null, null, null, nameof(OnTabSelected))]
        private readonly TabManager m_tabs;

        private bool m_disallowHexCodeFieldCallbacks, m_disallowSliderFieldCallbacks;

        private Color m_outputColor;
        public Color outputColor
        {
            get
            {
                return m_outputColor;
            }
            set
            {
                m_disallowSliderFieldCallbacks = true;
                m_rgbColorRChannelSlider.value = value.r;
                m_rgbColorGChannelSlider.value = value.g;
                m_rgbColorBChannelSlider.value = value.b;
                m_rgbColorAChannelSlider.value = showAlphaChannel ? value.a : 1f;

                Color.RGBToHSV(value, out float h, out float s, out float v);
                m_hsvColorHChannelSlider.value = h;
                m_hsvColorSChannelSlider.value = s;
                m_hsvColorVChannelSlider.value = v;
                m_hsvColorAChannelSlider.value = showAlphaChannel ? value.a : 1f;
                m_disallowSliderFieldCallbacks = false;

                m_outputColor = value;
            }
        }

        private float m_outputAlpha;
        public float outputAlpha
        {
            get
            {
                return m_outputAlpha;
            }
            set
            {
                m_disallowSliderFieldCallbacks = true;
                m_rgbColorAChannelSlider.value = showAlphaChannel ? value : 1f;
                m_hsvColorAChannelSlider.value = showAlphaChannel ? value : 1f;
                m_disallowSliderFieldCallbacks = false;
                m_outputAlpha = value;
            }
        }

        private bool m_showAlphaChannel;
        public bool showAlphaChannel
        {
            get
            {
                return m_showAlphaChannel;
            }
            set
            {
                m_disallowSliderFieldCallbacks = true;
                m_hsvColorAChannelSlider.gameObject.SetActive(value);
                m_hsvColorAChannelSlider.value = 1f;
                m_rgbColorAChannelSlider.gameObject.SetActive(value);
                m_rgbColorAChannelSlider.value = 1f;
                m_disallowSliderFieldCallbacks = false;
                m_showAlphaChannel = value;
            }
        }

        public Action<Color> callback { get; set; }

        public override bool enableCursor => true;

        protected override void OnInitialized()
        {
            m_tabs.AddTab(m_rgbSettingsTab.gameObject, "rgb");
            m_tabs.AddTab(m_hsvSettingsTab.gameObject, "hsv");
            m_tabs.SelectTab("rgb");
        }

        public override void Hide()
        {
            base.Hide();
            callback = null;
        }

        public void OnTabSelected(UIElementTab elementTab)
        {
            bool rgb = elementTab.tabId == "rgb";

            UIElementTab oldTab = m_tabs.prevSelectedTab;
            UIElementTab newTab = m_tabs.selectedTab;
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

            m_rgbSettingsHolderObject.SetActive(rgb);
            m_hsvSettingsHolderObject.SetActive(!rgb);
        }

        public void Populate(Color currentColor, bool useAlphaChannel, Action<Color> onColorChanged)
        {
            callback = onColorChanged;
            showAlphaChannel = useAlphaChannel;
            outputColor = currentColor;
            outputAlpha = useAlphaChannel ? currentColor.a : 1f;
        }

        public void InvokeCallback()
        {
            callback?.Invoke(outputColor);
        }

        public void RefreshHexCodeField()
        {
            m_disallowHexCodeFieldCallbacks = true;
            m_hexCodeField.text = "#" + (showAlphaChannel ? ColorUtility.ToHtmlStringRGBA(outputColor) : ColorUtility.ToHtmlStringRGB(outputColor));
            m_disallowHexCodeFieldCallbacks = false;
        }

        public void OnCloseButtonClicked()
        {
            Hide();
        }

        public void OnHexCodeFieldChanged(string value)
        {
            if (m_disallowHexCodeFieldCallbacks)
                return;

            outputColor = ModParseUtils.TryParseToColor(value, Color.white);
            InvokeCallback();
        }

        public void OnRGBColorRChannelSliderChanged(float value)
        {
            if (m_disallowSliderFieldCallbacks)
                return;

            Color color = outputColor;
            color.r = value;
            outputColor = color;
            InvokeCallback();
            RefreshHexCodeField();
        }

        public void OnRGBColorGChannelSliderChanged(float value)
        {
            if (m_disallowSliderFieldCallbacks)
                return;

            Color color = outputColor;
            color.g = value;
            outputColor = color;
            InvokeCallback();
            RefreshHexCodeField();
        }

        public void OnRGBColorBChannelSliderChanged(float value)
        {
            if (m_disallowSliderFieldCallbacks)
                return;

            Color color = outputColor;
            color.b = value;
            outputColor = color;
            InvokeCallback();
            RefreshHexCodeField();
        }

        public void OnRGBColorAChannelSliderChanged(float value)
        {
            if (m_disallowSliderFieldCallbacks)
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
            if (m_disallowSliderFieldCallbacks)
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
            if (m_disallowSliderFieldCallbacks)
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
            if (m_disallowSliderFieldCallbacks)
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
            if (m_disallowSliderFieldCallbacks)
                return;

            Color color = outputColor;
            color.a = value;
            outputColor = color;
            outputAlpha = value;
            InvokeCallback();
            RefreshHexCodeField();
        }
    }
}
