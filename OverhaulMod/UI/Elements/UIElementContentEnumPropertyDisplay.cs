using System.Collections.Generic;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    internal class UIElementContentEnumPropertyDisplay : UIElementContentCustomPropertyDisplay
    {
        [UIElement(0)]
        private readonly Dropdown m_dropdown;

        protected override void OnInitialized()
        {
            List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
            foreach (string name in fieldReference.FieldType.GetEnumNames())
            {
                list.Add(new Dropdown.OptionData() { text = name });
            }
            m_dropdown.options = list;

            object value = null;
            if (contentReference != null)
                value = fieldReference?.GetValue(contentReference);

            if (value == null)
                value = 0;

            int number = (int)value;
            m_dropdown.value = number;
        }

        public override object GetValue()
        {
            return m_dropdown.value;
        }
    }
}
