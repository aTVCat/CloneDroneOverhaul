using OverhaulMod.Content.Personalization;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationEditorInspector : OverhaulUIBehaviour
    {
        [UIElement("NameField")]
        private readonly InputField m_nameField;

        [UIElement("DescriptionField")]
        private readonly InputField m_descriptionField;

        [UIElement("EditorIdField")]
        private readonly InputField m_editorIdField;

        [UIElement("ItemIdField")]
        private readonly InputField m_itemIdField;

        [UIElement("TypeDropdown")]
        private readonly Dropdown m_typeDropdown;

        [UIElementAction(nameof(OnVerifyButtonClicked))]
        [UIElement("VerifyButton")]
        private readonly Button m_verifyButton;

        [UIElement("AuthorField", typeof(UIElementPersonalizationAuthorsField))]
        private readonly UIElementPersonalizationAuthorsField m_authorField;

        [UIElement("HierarchyGroup", typeof(UIElementPersonalizationEditorHierarchyPanel))]
        private readonly UIElementPersonalizationEditorHierarchyPanel m_hierarchyField;

        [UIElement("ImportedFilesGroup", typeof(UIElementPersonalizationEditorFileImportPanel))]
        private readonly UIElementPersonalizationEditorFileImportPanel m_filesField;

        public PersonalizationItemInfo itemInfo
        {
            get;
            private set;
        }

        public void Populate(PersonalizationItemInfo personalizationItemInfo)
        {
            itemInfo = personalizationItemInfo;
            if (personalizationItemInfo == null)
                return;

            personalizationItemInfo.FixValues();
            m_nameField.text = personalizationItemInfo.Name;
            m_descriptionField.text = personalizationItemInfo.Description;
            m_editorIdField.text = personalizationItemInfo.EditorID;
            m_authorField.referenceList = personalizationItemInfo.Authors;
            m_typeDropdown.value = (int)personalizationItemInfo.Category - 1;
            m_verifyButton.interactable = !personalizationItemInfo.IsVerified;
            m_itemIdField.text = personalizationItemInfo.ItemID;
        }

        public void ApplyValues()
        {
            PersonalizationItemInfo personalizationItemInfo = itemInfo;
            if (personalizationItemInfo == null)
                return;

            personalizationItemInfo.FixValues();
            personalizationItemInfo.Name = m_nameField.text;
            personalizationItemInfo.Description = m_descriptionField.text;
            personalizationItemInfo.EditorID = m_editorIdField.text;
            personalizationItemInfo.Category = (PersonalizationCategory)(m_typeDropdown.value + 1);
            personalizationItemInfo.IsVerified = !m_verifyButton.interactable;
            personalizationItemInfo.ItemID = m_itemIdField.text;
        }

        public void OnVerifyButtonClicked()
        {
            m_verifyButton.interactable = false;
        }
    }
}
