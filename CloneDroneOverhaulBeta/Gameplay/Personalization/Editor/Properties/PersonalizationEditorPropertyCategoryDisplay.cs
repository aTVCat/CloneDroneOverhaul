using System;
using System.Collections.Generic;
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
        public readonly PersonalizationEditorStringFieldDisplay ShortStringFieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorStringFieldDisplay) })]
        [ObjectReference("StringProperty_Big")]
        public readonly PersonalizationEditorStringFieldDisplay LongStringFieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorIntAltFieldDisplay) })]
        [ObjectReference("IntAltProperty")]
        public readonly PersonalizationEditorIntAltFieldDisplay IntAltFieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorIntFieldDisplay) })]
        [ObjectReference("IntProperty")]
        public readonly PersonalizationEditorIntFieldDisplay IntFieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorBoolFieldDisplay) })]
        [ObjectReference("BoolProperty")]
        public readonly PersonalizationEditorBoolFieldDisplay BoolFieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorFloatFieldDisplay) })]
        [ObjectReference("FloatProperty")]
        public readonly PersonalizationEditorFloatFieldDisplay FloatFieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorStringSpecViewFieldDisplay) })]
        [ObjectReference("StringProperty_SpecialView")]
        public readonly PersonalizationEditorStringSpecViewFieldDisplay StringListFieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorAssetInfoFieldDisplay) })]
        [ObjectReference("AssetProperty")]
        public readonly PersonalizationEditorAssetInfoFieldDisplay AssetInfoFieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorVoxAssetInfoFieldDisplay) })]
        [ObjectReference("VoxAssetProperty")]
        public readonly PersonalizationEditorVoxAssetInfoFieldDisplay VoxAssetInfoFieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorListFieldDisplay) })]
        [ObjectReference("ListProperty")]
        public readonly PersonalizationEditorListFieldDisplay ListOfTuple1FieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorTupleFieldDisplay) })]
        [ObjectReference("TupleProperty")]
        public readonly PersonalizationEditorTupleFieldDisplay TupleFieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorVector3FieldDisplay) })]
        [ObjectReference("Vector3Property")]
        public readonly PersonalizationEditorVector3FieldDisplay Vector3FieldDisplay;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorGuidFieldDisplay) })]
        [ObjectReference("GuidProperty")]
        public readonly PersonalizationEditorGuidFieldDisplay GuidFieldDisplay;

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
