using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorBoolFieldDisplay : PersonalizationEditorFieldDisplay
    {
        [ActionReference(nameof(onChangedValue))]
        [ObjectReference("Toggle")]
        private Toggle m_Toggle;

        public override void Initialize(FieldInfo fieldToEdit, object targetObject)
        {
            base.Initialize(fieldToEdit, targetObject);
            m_Toggle.isOn = (bool)FieldValue;
        }

        private void onChangedValue(bool newValue)
        {
            FieldValue = newValue;
            OnFieldValueChanged();
        }
    }
}
