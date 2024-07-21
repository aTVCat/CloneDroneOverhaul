using OverhaulMod.Utils;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementColorPickerButton : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(OnButtonClicked))]
        [UIElement("Button")]
        public Button m_button;

        [UIElement("Graphic")]
        public Graphic m_graphic;

        private Color m_color;
        public Color color
        {
            get
            {
                return m_color;
            }
            set
            {
                m_color = value;

                Color graphicColor = value;
                graphicColor.a = useAlpha ? value.a : 1f;
                m_graphic.color = graphicColor;

                onValueChanged.Invoke(value);
            }
        }

        public bool useAlpha { get; set; }

        public ColorChangedEvent onValueChanged { get; set; } = new ColorChangedEvent();

        public Transform colorPickerParent
        {
            get;
            set;
        }

        protected override void OnInitialized()
        {
            colorPickerParent = ModCache.gameUIRoot.transform;
        }

        public void OnButtonClicked()
        {
            ModUIUtils.ColorPicker(color, useAlpha, delegate (Color outColor)
            {
                color = outColor;
            }, colorPickerParent);
        }

        [Serializable]
        public class ColorChangedEvent : UnityEvent<Color>
        {
        }
    }
}
