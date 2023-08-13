using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorStringSpecViewFieldDisplay : PersonalizationEditorFieldDisplay
    {
        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorUserIDDisplay) })]
        [ObjectReference("PlayerIDPrefab")]
        private PersonalizationEditorUserIDDisplay m_PlayerIDPrefab;

        [ObjectDefaultVisibility(false)]
        [ObjectReference("AddPlayerIDPrefab")]
        private Button m_AddPlayerIDPrefab;

        [ObjectReference("Content")]
        private Transform m_Container;

        public EStringFieldDisplayType DisplayType
        {
            get;
            set;
        }

        public override void Initialize(FieldInfo fieldToEdit, object targetObject)
        {
            base.Initialize(fieldToEdit, targetObject);
        }

        public void InitializeField(EStringFieldDisplayType type)
        {
            if (type == EStringFieldDisplayType.Default)
                throw new Exception("PersonalizationEditorStringSpecViewFieldDisplay : Default, " + EditingField.Name);

            DisplayType = type;
            Populate();
        }

        public void Populate()
        {
            List<string> list = FieldValue as List<string>;

            TransformUtils.DestroyAllChildren(m_Container);
            if (!list.IsNullOrEmpty())
            {
                int index = 0;
                foreach (string id in list)
                {
                    string normalText = PersonalizationEditor.GetOnlyID(id, out byte type);

                    PersonalizationEditorUserIDDisplay component = Instantiate(m_PlayerIDPrefab, m_Container);
                    component.gameObject.SetActive(true);
                    component.Initialize(normalText, type, index);
                    component.FieldDisplay = this;
                    index++;
                }
            }

            Button addId = Instantiate(m_AddPlayerIDPrefab, m_Container);
            addId.gameObject.SetActive(true);
            addId.AddOnClickListener(AddNewPlayerInfo);
        }

        public void AddNewPlayerInfo()
        {
            EditorUI.PlayerInfoConfigMenu.Show(null, -1, true, delegate (string newValue)
            {
                List<string> list = FieldValue as List<string>;
                if(list == null)
                {
                    list = new List<string>();
                    EditingField.SetValue(TargetObject, list);
                    EditorUI.SavePanel.NeedsToSave = true;
                }
                list.Add(newValue);
                Populate();
            });
        }
    }
}
