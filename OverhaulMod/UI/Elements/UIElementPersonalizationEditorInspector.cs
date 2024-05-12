using OverhaulMod.Content.Personalization;
using System.Collections.Generic;
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

        [UIElementAction(nameof(OnEditedWeaponTypeDropdown))]
        [UIElement("WeaponDropdown")]
        private readonly Dropdown m_weaponDropdown;

        [UIElementAction(nameof(OnEditedBodyPartDropdown))]
        [UIElement("BodyPartDropdown")]
        private readonly Dropdown m_bodyPartDropdown;

        [UIElementAction(nameof(OnVerifyButtonClicked))]
        [UIElement("VerifyButton")]
        private readonly Button m_verifyButton;

        [UIElement("AuthorField", typeof(UIElementPersonalizationAuthorsField))]
        private readonly UIElementPersonalizationAuthorsField m_authorField;

        [UIElement("HierarchyGroup", typeof(UIElementPersonalizationEditorHierarchyPanel))]
        private readonly UIElementPersonalizationEditorHierarchyPanel m_hierarchyField;

        [UIElement("ImportedFilesGroup", typeof(UIElementPersonalizationEditorFileImportPanel))]
        private readonly UIElementPersonalizationEditorFileImportPanel m_filesField;

        private bool m_disallowCallbacks;

        public PersonalizationItemInfo itemInfo
        {
            get;
            private set;
        }

        protected override void OnInitialized()
        {
            List<Dropdown.OptionData> weaponList = m_weaponDropdown.options;
            weaponList.Clear();
            weaponList.Add(new DropdownWeaponTypeOptionData(WeaponType.Sword));
            weaponList.Add(new DropdownWeaponTypeOptionData(WeaponType.Bow));
            weaponList.Add(new DropdownWeaponTypeOptionData(WeaponType.Hammer));
            weaponList.Add(new DropdownWeaponTypeOptionData(WeaponType.Spear));
            weaponList.Add(new DropdownWeaponTypeOptionData(WeaponType.Shield));
            m_weaponDropdown.RefreshShownValue();

            List<Dropdown.OptionData> bodyPartList = m_bodyPartDropdown.options;
            bodyPartList.Clear();
            foreach (string bp in PersonalizationManager.SupportedBodyParts)
            {
                bodyPartList.Add(new Dropdown.OptionData(bp));
            }
            m_bodyPartDropdown.RefreshShownValue();
        }

        public void Populate(PersonalizationItemInfo personalizationItemInfo)
        {
            itemInfo = personalizationItemInfo;
            if (personalizationItemInfo == null)
                return;

            m_disallowCallbacks = true;
            personalizationItemInfo.FixValues();
            m_nameField.text = personalizationItemInfo.Name;
            m_descriptionField.text = personalizationItemInfo.Description;
            m_editorIdField.text = personalizationItemInfo.EditorID;
            m_authorField.referenceList = personalizationItemInfo.Authors;
            m_typeDropdown.value = (int)personalizationItemInfo.Category - 1;
            m_verifyButton.interactable = !personalizationItemInfo.IsVerified;
            m_itemIdField.text = personalizationItemInfo.ItemID;
            m_hierarchyField.itemInfo = personalizationItemInfo;
            m_filesField.itemInfo = personalizationItemInfo;

            for (int i = 0; i < m_weaponDropdown.options.Count; i++)
            {
                if ((m_weaponDropdown.options[i] as DropdownWeaponTypeOptionData).Weapon == personalizationItemInfo.Weapon)
                {
                    m_weaponDropdown.value = i;
                    break;
                }
            }

            for (int i = 0; i < m_bodyPartDropdown.options.Count; i++)
            {
                string text = m_bodyPartDropdown.options[i].text;
                if (text == personalizationItemInfo.BodyPartName)
                {
                    m_bodyPartDropdown.value = i;
                    break;
                }
            }

            FirstPersonMover firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
            firstPersonMover.SetEquippedWeaponType(personalizationItemInfo.Weapon, false);

            m_disallowCallbacks = false;
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
            personalizationItemInfo.BodyPartName = m_bodyPartDropdown.options[m_bodyPartDropdown.value].text;
        }

        public void OnEditedWeaponTypeDropdown(int value)
        {
            if (m_disallowCallbacks)
                return;

            PersonalizationItemInfo personalizationItemInfo = itemInfo;
            if (personalizationItemInfo == null)
                return;

            WeaponType weaponType = (m_weaponDropdown.options[value] as DropdownWeaponTypeOptionData).Weapon;
            personalizationItemInfo.Weapon = weaponType;

            FirstPersonMover firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
            firstPersonMover.SetEquippedWeaponType(weaponType, false);

            PersonalizationEditorManager.Instance.SpawnRootObject();
        }

        public void OnEditedBodyPartDropdown(int value)
        {
            if (m_disallowCallbacks)
                return;

            PersonalizationItemInfo personalizationItemInfo = itemInfo;
            if (personalizationItemInfo == null)
                return;

            personalizationItemInfo.BodyPartName = m_bodyPartDropdown.options[value].text;
            PersonalizationEditorManager.Instance.SpawnRootObject();
        }

        public void OnVerifyButtonClicked()
        {
            m_verifyButton.interactable = false;
        }
    }
}
