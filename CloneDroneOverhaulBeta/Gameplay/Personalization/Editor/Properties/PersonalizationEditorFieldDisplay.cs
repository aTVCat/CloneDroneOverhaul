using System;
using System.Collections;
using System.Reflection;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorFieldDisplay : PersonalizationEditorUIElement
    {
        [ObjectReference("Label")]
        private readonly Text m_Label;

        protected object InitialFieldValue;

        public PersonalizationEditorPropertyCategoryDisplay Category
        {
            get;
            set;
        }

        public PersonalizationEditorListEntryDisplay ListFieldDisplay
        {
            get;
            set;
        }

        private FieldInfo m_FieldReference;
        public FieldInfo FieldReference
        {
            get => m_FieldReference;
            set
            {
                if (m_FieldReference == null)
                    InitialFieldValue = value.GetValue(TargetObject);

                m_FieldReference = value;
            }
        }

        public Type FieldType => FieldReference.FieldType;

        public object FieldValue
        {
            get => FieldReference.GetValue(TargetObject);
            set => FieldReference.SetValue(TargetObject, value);
        }

        public object TargetObject
        {
            get;
            set;
        }

        public bool IsCollectionObject
        {
            get;
            set;
        }

        public bool HasDifferentControl
        {
            get;
            set;
        }

        protected override bool AssignVariablesAutomatically() => false;

        public virtual void Initialize(FieldInfo fieldToEdit, object targetObject)
        {
            TargetObject = targetObject;
            FieldReference = fieldToEdit;

            OverhaulUIController.AssignValues(this);
            m_Label.text = StringUtils.AddSpacesToCamelCasedString(fieldToEdit.Name);
        }

        public virtual void InitializeAsCollectionObject(PersonalizationEditorListEntryDisplay listFieldDisplay, string displayName, object targetObject)
        {
            IsCollectionObject = true;

            OverhaulUIController.AssignValues(this);
            m_Label.text = displayName;
            ListFieldDisplay = listFieldDisplay;
            TargetObject = targetObject;
        }

        public void OnFieldValueChanged()
        {
            if (IsCollectionObject)
            {
                (ListFieldDisplay.List as IList)[ListFieldDisplay.Index] = FieldValue;
            }
            EditorUI.SavePanel.NeedsToSave = true;
        }

        public void SetLabelText(string text)
        {
            m_Label.text = text;
        }
    }
}
