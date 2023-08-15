using System.Reflection;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorBoolFieldDisplay : PersonalizationEditorFieldDisplay
    {
        [ActionReference(nameof(onChangedValue))]
        [ObjectReference("Toggle")]
        private readonly Toggle m_Toggle;

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
