using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementContentColorPropertyDisplay : UIElementContentCustomPropertyDisplay
    {
        [UIElement("RedChannelField")]
        private readonly InputField m_rChannel;
        [UIElement("GreenChannelField")]
        private readonly InputField m_gChannel;
        [UIElement("BlueChannelField")]
        private readonly InputField m_bChannel;
        [UIElement("AlphaChannelField")]
        private readonly InputField m_aChannel;

        [UIElement("HexValueField")]
        private readonly InputField m_hexValueField;
        [UIElementAction(nameof(OnSetHexValueButtonClicked))]
        [UIElement("SetHexColorButton")]
        private readonly Button m_setHexColorButton;

        protected override void OnInitialized()
        {
            object value = null;
            if (contentReference != null)
                value = fieldReference?.GetValue(contentReference);

            if (value == null)
                value = Color.white;

            Color color = (Color)value;
            m_rChannel.text = color.r.ToString().Replace(',', '.');
            m_gChannel.text = color.g.ToString().Replace(',', '.');
            m_bChannel.text = color.b.ToString().Replace(',', '.');
            m_aChannel.text = color.a.ToString().Replace(',', '.');
            m_hexValueField.text = "#" + ColorUtility.ToHtmlStringRGBA(color);
        }

        public override object GetValue()
        {
            return new Color(ModParseUtils.TryParseToFloat(m_rChannel.text, 0f), ModParseUtils.TryParseToFloat(m_gChannel.text, 0f), ModParseUtils.TryParseToFloat(m_bChannel.text, 0f), ModParseUtils.TryParseToFloat(m_aChannel.text, 1f));
        }

        public void OnSetHexValueButtonClicked()
        {
            if (fieldReference == null || contentReference == null)
            {
                ModUIUtils.MessagePopupOK("Field/Content reference is NULL!", "that's weird");
                return;
            }

            fieldReference.SetValue(contentReference, ModParseUtils.TryParseToColor(m_hexValueField.text, Color.white));
            OnInitialized();
        }
    }
}
