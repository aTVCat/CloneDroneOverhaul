using OverhaulMod.Content.Personalization;
using OverhaulMod.UI.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [UIElement("TypeDropdown")]
        private readonly Dropdown m_typeDropdown;

        [UIElementAction(nameof(OnVerifyButtonClicked))]
        [UIElement("VerifyButton")]
        private readonly Button m_verifyButton;

        [UIElement("AuthorField", typeof(UIElementPersonalizationAuthorsField))]
        private readonly UIElementPersonalizationAuthorsField m_authorField;

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
        }

        public void OnVerifyButtonClicked()
        {
            m_verifyButton.interactable = false;
        }
    }
}
