using OverhaulMod.Content.Personalization;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationEditorHierarchyPanel : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(OnCreateButtonClicked))]
        [UIElement("CreateButton")]
        private readonly Button m_createButton;

        [UIElement("ObjectDisplayPrefab", false)]
        private readonly ModdedObject m_showWeaponToggle;

        [UIElement("Content")]
        private readonly Transform m_enableAnimationToggle;

        private PersonalizationEditorObjectBehaviour m_objectBehaviour;
        public PersonalizationEditorObjectBehaviour objectBehaviour
        {
            get
            {
                return m_objectBehaviour;
            }
            set
            {
                m_objectBehaviour = value;
                Populate();
            }
        }

        public void Populate()
        {

        }

        public void OnCreateButtonClicked()
        {

        }
    }
}
