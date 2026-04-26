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
        public Button _button;

        [UIElement("Graphic")]
        public Graphic _graphic;

        private Color _color;
        public Color color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;

                Color graphicColor = value;
                graphicColor.a = useAlpha ? value.a : 1f;
                _graphic.color = graphicColor;

                onValueChanged.Invoke(value);
            }
        }

        public bool useAlpha { get; set; }

        public ColorChangedEvent onValueChanged { get; set; } = new ColorChangedEvent();

        public Transform ColorPickerParent
        {
            get;
            set;
        }

        protected override void OnInitialized()
        {
            ColorPickerParent = ModCache.UIRoot.transform;
        }

        public void OnButtonClicked()
        {
            ModUIUtils.ColorPicker(color, useAlpha, delegate (Color outColor)
            {
                color = outColor;
            }, ColorPickerParent);
        }

        [Serializable]
        public class ColorChangedEvent : UnityEvent<Color>
        {
        }
    }
}
