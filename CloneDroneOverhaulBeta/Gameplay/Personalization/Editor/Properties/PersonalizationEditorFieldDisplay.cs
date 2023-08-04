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
                    InitialFieldValue = value.GetValue(PersonalizationEditor.EditingItem);

                m_EditingField = value;
            }
        }

        public object FieldValue
        {
            get => EditingField.GetValue(PersonalizationEditor.EditingItem);
            set => EditingField.SetValue(PersonalizationEditor.EditingItem, value);
        }

        public Type FieldType => EditingField.FieldType;

        protected override bool AssignVariablesAutomatically() => false;

        public virtual void Initialize(FieldInfo fieldToEdit)
        {
            EditingField = fieldToEdit;

            OverhaulUIVer2.AssignVariables(this);
            m_Label.text = StringUtils.AddSpacesToCamelCasedString(fieldToEdit.Name);
        }

        public void OnFieldValueChanged()
        {
            EditorUI.SavePanel.NeedsSaving = true;
        }
    }
}
