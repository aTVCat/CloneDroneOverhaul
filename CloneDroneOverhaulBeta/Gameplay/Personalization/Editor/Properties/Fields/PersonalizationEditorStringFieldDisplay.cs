using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorStringFieldDisplay : PersonalizationEditorFieldDisplay
    {
        [ActionReference(nameof(onChangedValue))]
        [ObjectReference("InputField")]
        private InputField m_InputField;

        public override void Initialize(FieldInfo fieldToEdit)
        {
            base.Initialize(fieldToEdit);
            m_InputField.text = FieldValue as string;
        }

        private void onChangedValue(string newValue)
        {
            FieldValue = newValue;
            OnFieldValueChanged();
        }
    }
}
