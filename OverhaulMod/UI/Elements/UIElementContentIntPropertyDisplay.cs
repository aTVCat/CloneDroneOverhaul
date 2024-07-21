using OverhaulMod.Utils;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    internal class UIElementContentIntPropertyDisplay : UIElementContentCustomPropertyDisplay
    {
        [UIElement("InputField")]
        private readonly InputField m_InputField;

        protected override void OnInitialized()
        {
            m_InputField.contentType = InputField.ContentType.IntegerNumber;

            object value = null;
            if (contentReference != null)
                value = fieldReference?.GetValue(contentReference);

            if (value == null)
                value = 0;

            int number = (int)value;
            m_InputField.text = number.ToString();
        }

        public override object GetValue()
        {
            return ModParseUtils.TryParseToInt(m_InputField.text, 0);
        }
    }
}
