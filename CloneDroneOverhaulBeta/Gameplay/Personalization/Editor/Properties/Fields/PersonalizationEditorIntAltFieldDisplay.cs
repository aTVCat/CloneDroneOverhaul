using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorIntAltFieldDisplay : PersonalizationEditorFieldDisplay
    {
        [ActionReference(nameof(onChangedValue))]
        [ObjectReference("Dropdown2")]
        private Dropdown m_Dropdown;

        public override void Initialize(FieldInfo fieldToEdit, object targetObject)
        {
            base.Initialize(fieldToEdit, targetObject);
            m_Dropdown.value = (int)FieldValue;
        }

        public void AddOptions(string[] options)
        {
            List<Dropdown.OptionData> list = new List<Dropdown.OptionData>(options.Length);
            foreach(string option in options)
            {
                list.Add(new Dropdown.OptionData(option));
            }
            m_Dropdown.options = list;
        }

        private void onChangedValue(int newValue)
        {
            FieldValue = newValue;
            OnFieldValueChanged();
        }
    }
}
