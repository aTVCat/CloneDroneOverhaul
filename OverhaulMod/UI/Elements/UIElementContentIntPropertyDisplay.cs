using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace OverhaulMod.UI
{
    internal class UIElementContentIntPropertyDisplay : UIElementContentCustomPropertyDisplay
    {
        [UIElement("InputField")]
        private InputField m_InputField;

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
