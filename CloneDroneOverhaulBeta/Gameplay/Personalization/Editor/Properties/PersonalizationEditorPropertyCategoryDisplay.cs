using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorPropertyCategoryDisplay : PersonalizationEditorUIElement
    {
        [UIElementReferenceAttribute("Header")]
        private readonly Text m_Header;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new System.Type[] { typeof(PersonalizationEditorStringFieldDisplay) })]
        [UIElementReferenceAttribute("StringProperty_Small")]
        public readonly PersonalizationEditorStringFieldDisplay ShortStringFieldDisplay;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new System.Type[] { typeof(PersonalizationEditorStringFieldDisplay) })]
        [UIElementReferenceAttribute("StringProperty_Big")]
        public readonly PersonalizationEditorStringFieldDisplay LongStringFieldDisplay;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new System.Type[] { typeof(PersonalizationEditorIntAltFieldDisplay) })]
        [UIElementReferenceAttribute("IntAltProperty")]
        public readonly PersonalizationEditorIntAltFieldDisplay IntAltFieldDisplay;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new System.Type[] { typeof(PersonalizationEditorIntFieldDisplay) })]
        [UIElementReferenceAttribute("IntProperty")]
        public readonly PersonalizationEditorIntFieldDisplay IntFieldDisplay;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new System.Type[] { typeof(PersonalizationEditorBoolFieldDisplay) })]
        [UIElementReferenceAttribute("BoolProperty")]
        public readonly PersonalizationEditorBoolFieldDisplay BoolFieldDisplay;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new System.Type[] { typeof(PersonalizationEditorFloatFieldDisplay) })]
        [UIElementReferenceAttribute("FloatProperty")]
        public readonly PersonalizationEditorFloatFieldDisplay FloatFieldDisplay;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new System.Type[] { typeof(PersonalizationEditorStringSpecViewFieldDisplay) })]
        [UIElementReferenceAttribute("StringProperty_SpecialView")]
        public readonly PersonalizationEditorStringSpecViewFieldDisplay StringListFieldDisplay;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new System.Type[] { typeof(PersonalizationEditorAssetInfoFieldDisplay) })]
        [UIElementReferenceAttribute("AssetProperty")]
        public readonly PersonalizationEditorAssetInfoFieldDisplay AssetInfoFieldDisplay;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new System.Type[] { typeof(PersonalizationEditorVoxAssetInfoFieldDisplay) })]
        [UIElementReferenceAttribute("VoxAssetProperty")]
        public readonly PersonalizationEditorVoxAssetInfoFieldDisplay VoxAssetInfoFieldDisplay;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new System.Type[] { typeof(PersonalizationEditorListFieldDisplay) })]
        [UIElementReferenceAttribute("ListProperty")]
        public readonly PersonalizationEditorListFieldDisplay ListOfTuple1FieldDisplay;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new System.Type[] { typeof(PersonalizationEditorTupleFieldDisplay) })]
        [UIElementReferenceAttribute("TupleProperty")]
        public readonly PersonalizationEditorTupleFieldDisplay TupleFieldDisplay;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new System.Type[] { typeof(PersonalizationEditorVector3FieldDisplay) })]
        [UIElementReferenceAttribute("Vector3Property")]
        public readonly PersonalizationEditorVector3FieldDisplay Vector3FieldDisplay;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new System.Type[] { typeof(PersonalizationEditorGuidFieldDisplay) })]
        [UIElementReferenceAttribute("GuidProperty")]
        public readonly PersonalizationEditorGuidFieldDisplay GuidFieldDisplay;

        [UIElementReferenceAttribute("Container")]
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
                UIController.AssignVariables(this);
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

                PersonalizationEditorFieldDisplay fieldDisplay = null;
                if (property.FieldReference.FieldType == typeof(string))
                {
                    fieldDisplay = Instantiate(property.UseAltDisplay ? LongStringFieldDisplay : ShortStringFieldDisplay, m_Container.transform);
                }
                else if (property.FieldReference.FieldType == typeof(int))
                {
                    if (property.UseAltDisplay)
                    {
                        fieldDisplay = Instantiate(IntAltFieldDisplay, m_Container.transform);

                        PersonalizationEditorIntAltFieldDisplay intAltDisplay = fieldDisplay as PersonalizationEditorIntAltFieldDisplay;
                        intAltDisplay.AddOptions(property.AdditionalParameters as string[]);
                    }
                    else
                    {
                        fieldDisplay = Instantiate(IntFieldDisplay, m_Container.transform);
                    }
                }
                else if (property.FieldReference.FieldType == typeof(float))
                {
                    fieldDisplay = Instantiate(FloatFieldDisplay, m_Container.transform);
                }
                else if (property.FieldReference.FieldType == typeof(bool))
                {
                    fieldDisplay = Instantiate(BoolFieldDisplay, m_Container.transform);
                }
                else if (property.FieldReference.FieldType == typeof(List<string>))
                {
                    fieldDisplay = Instantiate(StringListFieldDisplay, m_Container.transform);
                }
                else if (property.FieldReference.FieldType == typeof(OverhaulAssetInfo))
                {
                    fieldDisplay = Instantiate(AssetInfoFieldDisplay, m_Container.transform);
                }
                else if (property.FieldReference.FieldType == typeof(OverhaulVoxAssetInfo))
                {
                    fieldDisplay = Instantiate(VoxAssetInfoFieldDisplay, m_Container.transform);
                }
                else if (property.FieldReference.FieldType == typeof(List<Tuple<string, Vector3>>))
                {
                    fieldDisplay = Instantiate(ListOfTuple1FieldDisplay, m_Container.transform);
                }
                else if (property.FieldReference.FieldType == typeof(Vector3))
                {
                    fieldDisplay = Instantiate(Vector3FieldDisplay, m_Container.transform);
                }
                else if (property.FieldReference.FieldType == typeof(Guid))
                {
                    fieldDisplay = Instantiate(GuidFieldDisplay, m_Container.transform);
                }

                if (fieldDisplay)
                {
                    fieldDisplay.gameObject.SetActive(true);
                    fieldDisplay.Category = this;
                    fieldDisplay.Initialize(property.FieldReference, targetObject);

                    if (fieldDisplay is PersonalizationEditorStringSpecViewFieldDisplay)
                        (fieldDisplay as PersonalizationEditorStringSpecViewFieldDisplay).InitializeField(EStringFieldDisplayType.UserIDs);

                    if (fieldDisplay is PersonalizationEditorListFieldDisplay)
                    {
                        PersonalizationEditorListFieldDisplay listFieldDisplay = fieldDisplay as PersonalizationEditorListFieldDisplay;
                        listFieldDisplay.IsPositionNodesList = property.FieldReference.FieldType == typeof(List<Tuple<string, Vector3>>);
                    }
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
