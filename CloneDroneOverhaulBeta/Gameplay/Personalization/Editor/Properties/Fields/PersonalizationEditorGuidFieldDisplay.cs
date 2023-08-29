using System;
using System.Reflection;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorGuidFieldDisplay : PersonalizationEditorFieldDisplay
    {
        [UIElementReferenceAttribute("InputField")]
        private readonly InputField m_InputField;

        [UIElementActionReference(nameof(NewGuid))]
        [UIElementReferenceAttribute("NewGuid")]
        private readonly Button m_NewGuid;

        public override void Initialize(FieldInfo fieldToEdit, object targetObject)
        {
            base.Initialize(fieldToEdit, targetObject);
            m_InputField.text = ((Guid)FieldValue).ToString();
        }

        public void NewGuid()
        {
            FieldValue = Guid.NewGuid();
            m_InputField.text = ((Guid)FieldValue).ToString();
            OnFieldValueChanged();
        }
    }
}
