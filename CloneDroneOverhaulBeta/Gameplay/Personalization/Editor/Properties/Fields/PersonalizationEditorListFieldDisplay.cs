using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorListFieldDisplay : PersonalizationEditorFieldDisplay
    {
        [ObjectReference("Content")]
        private readonly Transform m_Container;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new Type[] { typeof(PersonalizationEditorListEntryDisplay) })]
        [ObjectReference("ListEntry")]
        private readonly PersonalizationEditorListEntryDisplay m_EntryPrefab;

        [ObjectDefaultVisibility(false)]
        [ObjectReference("AddValue")]
        private readonly Button m_AddValuePrefab;

        public bool IsPositionNodesList
        {
            get;
            set;
        }

        public override void Initialize(FieldInfo fieldToEdit, object targetObject)
        {
            base.Initialize(fieldToEdit, targetObject);
        }

        public override void Start()
        {
            Populate();
        }

        public void Populate()
        {
            TransformUtils.DestroyAllChildren(m_Container);
            if (!(FieldValue is ICollection list))
            {
                list = (ICollection)Activator.CreateInstance(FieldType);
                FieldValue = list;
            }

            int index = 0;
            foreach (object value in list)
            {
                PersonalizationEditorListEntryDisplay entry = Instantiate(m_EntryPrefab, m_Container);
                entry.gameObject.SetActive(true);
                entry.Initialize(this, list, index);
                index++;
            }

            Button addValue = Instantiate(m_AddValuePrefab, m_Container);
            addValue.gameObject.SetActive(true);
            addValue.AddOnClickListener(AddValue);
        }

        public void AddValue()
        {
            if (IsPositionNodesList)
            {
                (FieldValue as List<Tuple<string, Vector3>>).Add(new Tuple<string, Vector3>(string.Empty, Vector3.zero));
            }
            Populate();
        }
    }
}
