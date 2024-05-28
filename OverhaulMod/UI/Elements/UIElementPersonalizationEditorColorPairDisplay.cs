using OverhaulMod.Engine;
using OverhaulMod.Utils;
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

        //[UIElementAction(nameof(OnResetButtonClicked))]
        [UIElement("PlayerColorButton")]
        private readonly Button m_playerColorButton;

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

        public void OnColorBButtonClicked()
        {
            ModUIUtils.ColorPicker(colorB, true, onColorBChanged, colorPickerTransform);
        }

        public void OnResetButtonClicked()
        {
            colorB = colorA;
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
