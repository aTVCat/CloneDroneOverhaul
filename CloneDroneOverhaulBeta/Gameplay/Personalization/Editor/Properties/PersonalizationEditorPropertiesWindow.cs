using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorPropertiesWindow : OverhaulBehaviour
    {
        [ObjectReference("CategoryPrefab")]
        private Transform m_PropertyCategoryPrefab;

        [ObjectReference("Content")]
        private Transform m_Container;

        private bool m_HasAssignedValues;

        public void Populate(PersonalizationCategory category)
        {
            if (!m_HasAssignedValues)
            {
                OverhaulUIVer2.AssignVariables(this);
                m_HasAssignedValues = true;
            }

            if(PersonalizationEditor.EditingItem == null)
                return;

            List<PersonalizationEditorPropertyAttribute> properties = null;
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
