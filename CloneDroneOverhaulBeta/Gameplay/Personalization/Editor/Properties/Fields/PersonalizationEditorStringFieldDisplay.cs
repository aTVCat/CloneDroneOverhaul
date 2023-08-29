using System.Reflection;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorStringFieldDisplay : PersonalizationEditorFieldDisplay
    {
        [UIElementActionReference(nameof(onChangedValue))]
        [UIElementReferenceAttribute("InputField")]
        private readonly InputField m_InputField;

        public override void Initialize(FieldInfo fieldToEdit, object targetObject)
        {
            if (HasDifferentControl)
            {
                UIController.AssignValues(this);
                m_InputField.text = TargetObject as string;
            }
            else
            {
                base.Initialize(fieldToEdit, targetObject);
                m_InputField.text = FieldValue as string;
            }
        }

        private void onChangedValue(string newValue)
        {
            if (!HasDifferentControl)
            {
                FieldValue = newValue;
            }
            OnFieldValueChanged();
        }

        public void SetOnValueChangeAction(UnityAction<string> action)
        {
            m_InputField.onEndEdit = new InputField.SubmitEvent();
            m_InputField.onEndEdit.AddListener(action);
        }
    }
}
