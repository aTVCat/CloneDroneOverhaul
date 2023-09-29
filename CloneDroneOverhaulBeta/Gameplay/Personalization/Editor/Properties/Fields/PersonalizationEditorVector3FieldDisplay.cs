using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorVector3FieldDisplay : PersonalizationEditorFieldDisplay
    {
        [UIElementActionReference(nameof(onXChanged))]
        [UIElementReferenceAttribute("InputField_X")]
        private readonly InputField m_X;

        [UIElementActionReference(nameof(onYChanged))]
        [UIElementReferenceAttribute("InputField_Y")]
        private readonly InputField m_Y;

        [UIElementActionReference(nameof(onZChanged))]
        [UIElementReferenceAttribute("InputField_Z")]
        private readonly InputField m_Z;

        private Action<Vector3> m_OnValueChangedAction;

        public override void Initialize(FieldInfo fieldToEdit, object targetObject)
        {
            if (HasDifferentControl)
            {
                UIController.AssignVariables(this);
            }
            else
            {
                base.Initialize(fieldToEdit, targetObject);
            }

            Vector3 vector = HasDifferentControl ? (Vector3)TargetObject : (Vector3)FieldValue;
            m_X.text = vector.x.ToString().Replace(',', '.');
            m_Y.text = vector.y.ToString().Replace(',', '.');
            m_Z.text = vector.z.ToString().Replace(',', '.');
        }

        private void onXChanged(string newValue)
        {
            Vector3 vector = HasDifferentControl ? (Vector3)TargetObject : (Vector3)FieldValue;
            vector.x = float.Parse(newValue.Replace('.', ','));
            if (HasDifferentControl)
                TargetObject = vector;
            else
                FieldValue = vector;
            OnFieldValueChanged();
            m_OnValueChangedAction?.Invoke(vector);
        }

        private void onYChanged(string newValue)
        {
            Vector3 vector = HasDifferentControl ? (Vector3)TargetObject : (Vector3)FieldValue;
            vector.y = float.Parse(newValue.Replace('.', ','));
            if (HasDifferentControl)
                TargetObject = vector;
            else
                FieldValue = vector;
            OnFieldValueChanged();
            m_OnValueChangedAction?.Invoke(vector);
        }

        private void onZChanged(string newValue)
        {
            Vector3 vector = HasDifferentControl ? (Vector3)TargetObject : (Vector3)FieldValue;
            vector.z = float.Parse(newValue.Replace('.', ','));
            if (HasDifferentControl)
                TargetObject = vector;
            else
                FieldValue = vector;
            OnFieldValueChanged();
            m_OnValueChangedAction?.Invoke(vector);
        }

        public void SetOnValueChangeAction(Action<Vector3> action)
        {
            m_OnValueChangedAction = action;
        }
    }
}
