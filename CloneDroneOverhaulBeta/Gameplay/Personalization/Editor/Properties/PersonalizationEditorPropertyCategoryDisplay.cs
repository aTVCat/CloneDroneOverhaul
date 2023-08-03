using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorPropertyCategoryDisplay : OverhaulBehaviour
    {
        [ObjectReference("Header")]
        private Text m_Header;

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
                OverhaulUIVer2.AssignVariables(this);
                m_HasInitialized = true;
            }

            m_Header.text = categoryName;
        }
    }
}
