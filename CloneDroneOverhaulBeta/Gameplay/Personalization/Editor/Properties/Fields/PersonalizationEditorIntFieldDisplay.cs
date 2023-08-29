using System.Reflection;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorIntFieldDisplay : PersonalizationEditorFieldDisplay
    {
        [UIElementActionReference(nameof(onChangedValue))]
        [UIElementReferenceAttribute("InputField")]
        private readonly InputField m_InputField;

        public override void Initialize(FieldInfo fieldToEdit, object targetObject)
        {
            base.Initialize(fieldToEdit, targetObject);
            m_InputField.text = ((int)FieldValue).ToString();
        }

        private void onChangedValue(string newValue)
        {
            if (!int.TryParse(newValue, out int result))
            {
                string initialText = ((int)InitialFieldValue).ToString();
                FieldValue = initialText;
                m_InputField.text = initialText;
                return;
            }

            FieldValue = result;
            OnFieldValueChanged();
        }
    }
}
