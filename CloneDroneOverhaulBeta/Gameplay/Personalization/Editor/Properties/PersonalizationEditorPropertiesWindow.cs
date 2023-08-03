using OverhaulAPI.SharedMonoBehaviours;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorPropertiesWindow : PersonalizationEditorElement
    {
        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new System.Type[] { typeof(PersonalizationEditorPropertyCategoryDisplay) })]
        [ObjectReference("CategoryPrefab")]
        private readonly PersonalizationEditorPropertyCategoryDisplay m_PropertyCategoryPrefab;

        [ObjectReference("Content")]
        private readonly Transform m_Container;

        private bool m_HasInitialized;

        public void Populate(PersonalizationCategory category)
        {
            if (!m_HasInitialized)
            {
                OverhaulUIVer2.AssignVariables(this);
                _ = base.gameObject.AddComponent<OverhaulDraggablePanel>();
                m_HasInitialized = true;
            }

            if (PersonalizationEditor.EditingItem == null)
                return;

            List<PersonalizationEditorPropertyAttribute> properties;
            switch (category)
            {
                case PersonalizationCategory.WeaponSkins:
                    properties = PersonalizationEditor.WeaponSkinItemFields;
                    break;
                case PersonalizationCategory.Outfits:
                    properties = PersonalizationEditor.AccessoryItemFields;
                    break;
                case PersonalizationCategory.Pets:
                    properties = PersonalizationEditor.PetItemFields;
                    break;
                default:
                    throw new System.NotImplementedException(category + " is not supported!");
            }

            if (properties.IsNullOrEmpty())
                throw new System.NullReferenceException("The attributes list of " + category + " is null/empty");


        }
    }
}
