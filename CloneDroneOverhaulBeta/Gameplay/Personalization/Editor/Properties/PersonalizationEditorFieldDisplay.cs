using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorFieldDisplay : PersonalizationEditorUIElement
    {
        [ObjectReference("Label")]
        private Text m_Label;

        protected object InitialFieldValue;

        private FieldInfo m_EditingField;
        public FieldInfo EditingField
        {
            get => m_EditingField;
            set
            {
                if (m_EditingField == null)
                    InitialFieldValue = value.GetValue(TargetObject);

                m_EditingField = value;
            }
        }

        public object FieldValue
        {
            get => EditingField.GetValue(TargetObject);
            set => EditingField.SetValue(TargetObject, value);
        }

        public Type FieldType => EditingField.FieldType;

        public object TargetObject
        {
            get;
            set;
        }

        protected override bool AssignVariablesAutomatically() => false;

        public virtual void Initialize(FieldInfo fieldToEdit, object targetObject)
        {
            TargetObject = targetObject;
            EditingField = fieldToEdit;

            OverhaulUIVer2.AssignValues(this);
            m_Label.text = StringUtils.AddSpacesToCamelCasedString(fieldToEdit.Name);
        }

        public void OnFieldValueChanged()
        {
            EditorUI.SavePanel.NeedsToSave = true;
        }
    }
}
