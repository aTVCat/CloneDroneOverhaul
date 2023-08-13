using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorPropertyCategoryDisplay : PersonalizationEditorUIElement
    {
        [ObjectReference("Header")]
        private readonly Text m_Header;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorStringFieldDisplay) })]
        [ObjectReference("StringProperty_Small")]
        private readonly PersonalizationEditorStringFieldDisplay m_ShortStringFieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorStringFieldDisplay) })]
        [ObjectReference("StringProperty_Big")]
        private readonly PersonalizationEditorStringFieldDisplay m_LongStringFieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorIntAltFieldDisplay) })]
        [ObjectReference("IntAltProperty")]
        private readonly PersonalizationEditorIntAltFieldDisplay m_IntAltFieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorIntFieldDisplay) })]
        [ObjectReference("IntProperty")]
        private readonly PersonalizationEditorIntFieldDisplay m_IntFieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorBoolFieldDisplay) })]
        [ObjectReference("BoolProperty")]
        private readonly PersonalizationEditorBoolFieldDisplay m_BoolFieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorFloatFieldDisplay) })]
        [ObjectReference("FloatProperty")]
        private readonly PersonalizationEditorFloatFieldDisplay m_FloatFieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorStringSpecViewFieldDisplay) })]
        [ObjectReference("StringProperty_SpecialView")]
        private readonly PersonalizationEditorStringSpecViewFieldDisplay m_StringListFieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorAssetInfoFieldDisplay) })]
        [ObjectReference("AssetProperty")]
        private readonly PersonalizationEditorAssetInfoFieldDisplay m_AssetInfoFieldDisplay;

        [ObjectReference("Container")]
        private readonly ContentSizeFitter m_Container;

        private bool m_HasInitialized;

        public bool IsInstantiated
        {
            get;
            set;
        }

        public void Populate(string categoryName, List<PersonalizationEditorPropertyAttribute> properties)
        {
            if (!IsInstantiated)
                return;

            if (!m_HasInitialized)
            {
                OverhaulUIVer2.AssignValues(this);
                m_HasInitialized = true;
            }

            m_Header.text = categoryName;
            foreach (PersonalizationEditorPropertyAttribute property in properties)
            {
                object targetObject = PersonalizationEditor.EditingItem;
                if (!string.IsNullOrEmpty(property.SubClassFieldName))
                {
                    targetObject = PersonalizationEditor.EditingItem.GetTargetObjectOfSubClass(property.SubClassFieldName);
                }

                /*
                if (property.AssignValueIfNull)
                {
                    object fieldValue = property.FieldReference.GetValue(targetObject);
                    if(fieldValue == null)
                    {
                        property.FieldReference.SetValue(System.Activator.CreateInstance(property.FieldReference.FieldType), targetObject);
                    }
                }*/

                PersonalizationEditorFieldDisplay fieldDisplay = null;
                if (property.FieldReference.FieldType == typeof(string))
                {
                    fieldDisplay = Instantiate(property.UseAltDisplay ? m_LongStringFieldDisplay : m_ShortStringFieldDisplay, m_Container.transform);
                }
                else if (property.FieldReference.FieldType == typeof(int))
                {
                    if (property.UseAltDisplay)
                    {
                        fieldDisplay = Instantiate(m_IntAltFieldDisplay, m_Container.transform);

                        PersonalizationEditorIntAltFieldDisplay intAltDisplay = fieldDisplay as PersonalizationEditorIntAltFieldDisplay;
                        intAltDisplay.AddOptions(property.AdditionalParameters as string[]);
                    }
                    else
                    {
                        fieldDisplay = Instantiate(m_IntFieldDisplay, m_Container.transform);
                    }
                }
                else if (property.FieldReference.FieldType == typeof(float))
                {
                    fieldDisplay = Instantiate(m_FloatFieldDisplay, m_Container.transform);
                }
                else if (property.FieldReference.FieldType == typeof(bool))
                {
                    fieldDisplay = Instantiate(m_BoolFieldDisplay, m_Container.transform);
                }
                else if (property.FieldReference.FieldType == typeof(List<string>))
                {
                    fieldDisplay = Instantiate(m_StringListFieldDisplay, m_Container.transform);
                }
                else if (property.FieldReference.FieldType == typeof(OverhaulAssetInfo))
                {
                    fieldDisplay = Instantiate(m_AssetInfoFieldDisplay, m_Container.transform);
                }

                if (fieldDisplay)
                {
                    fieldDisplay.gameObject.SetActive(true);
                    fieldDisplay.Initialize(property.FieldReference, targetObject);

                    if (fieldDisplay is PersonalizationEditorStringSpecViewFieldDisplay)
                        (fieldDisplay as PersonalizationEditorStringSpecViewFieldDisplay).InitializeField(EStringFieldDisplayType.UserIDs);
                }
            }
        }

        private void Update()
        {
            RectTransform rectTransform = base.transform as RectTransform;
            Vector2 vector = rectTransform.sizeDelta;
            vector.y = (m_Container.transform as RectTransform).sizeDelta.y + 30f;
            rectTransform.sizeDelta = vector;
        }
    }
}
