using OverhaulMod.Content.Personalization;
using System.Collections.Generic;
using UnityEngine;
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

        [UIElementAction(nameof(OnHideBowStringsToggled))]
        [UIElement("HideBowStringsToggle")]
        private readonly Toggle m_hideBowStrings;

        [UIElementAction(nameof(OnEditedOverrideParentDropdown))]
        [UIElement("OverrideParentDropdown")]
        private readonly Dropdown m_overrideParentDropdown;

        [UIElementAction(nameof(OnEditedBowStringsWidth))]
        [UIElement("BowStringsWidthSlider")]
        private readonly Slider m_bowStringsWidth;

        [UIElement("ExclusiveForField", typeof(UIElementPersonalizationExclusiveForField))]
        private readonly UIElementPersonalizationExclusiveForField m_exclusiveForField;

        [UIElement("AuthorField", typeof(UIElementPersonalizationAuthorsField))]
        private readonly UIElementPersonalizationAuthorsField m_authorField;

        [UIElement("HierarchyGroup", typeof(UIElementPersonalizationEditorHierarchyPanel))]
        private readonly UIElementPersonalizationEditorHierarchyPanel m_hierarchyPanel;

        [UIElement("ImportedFilesGroup", typeof(UIElementPersonalizationEditorFileImportPanel))]
        private readonly UIElementPersonalizationEditorFileImportPanel m_filesPanel;

        [UIElement("SpecialInfoGroup", false)]
        private readonly GameObject m_specialInfoPanel;

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
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.ShieldSkins))
                weaponList.Add(new DropdownWeaponTypeOptionData(WeaponType.Shield));
            m_weaponDropdown.RefreshShownValue();

            List<Dropdown.OptionData> overrideParentList = m_overrideParentDropdown.options;
            overrideParentList.Clear();
            overrideParentList.Add(new DropdownStringOptionData()
            {
                text = "Default",
                StringValue = null,
            });
            overrideParentList.Add(new DropdownStringOptionData()
            {
                text = "Left hand",
                StringValue = "HandL",
            });
            overrideParentList.Add(new DropdownStringOptionData()
            {
                text = "Right hand",
                StringValue = "HandR",
            });
            m_overrideParentDropdown.RefreshShownValue();

            List<Dropdown.OptionData> bodyPartList = m_bodyPartDropdown.options;
            bodyPartList.Clear();
            foreach (string bp in PersonalizationManager.SupportedBodyParts)
            {
                bodyPartList.Add(new Dropdown.OptionData(bp));
            }
            m_bodyPartDropdown.RefreshShownValue();

            m_typeDropdown.interactable = ModFeatures.IsEnabled(ModFeatures.FeatureType.AccessoriesAndPets);

            m_specialInfoPanel.SetActive(PersonalizationEditorManager.Instance.canEditItemSpecialInfo);
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
            m_exclusiveForField.referenceList = personalizationItemInfo.ExclusiveFor_V2;
            m_typeDropdown.value = (int)personalizationItemInfo.Category - 1;
            m_verifyButton.interactable = !personalizationItemInfo.IsVerified;
            m_itemIdField.text = personalizationItemInfo.ItemID;
            m_hierarchyPanel.itemInfo = personalizationItemInfo;
            m_filesPanel.itemInfo = personalizationItemInfo;
            m_hideBowStrings.isOn = personalizationItemInfo.HideBowStrings;
            m_hideBowStrings.interactable = personalizationItemInfo.Category == PersonalizationCategory.WeaponSkins && personalizationItemInfo.Weapon == WeaponType.Bow;
            m_overrideParentDropdown.interactable = personalizationItemInfo.Category == PersonalizationCategory.WeaponSkins && personalizationItemInfo.Weapon == WeaponType.Bow;
            m_bowStringsWidth.value = personalizationItemInfo.BowStringsWidth;
            m_bowStringsWidth.interactable = personalizationItemInfo.Category == PersonalizationCategory.WeaponSkins && personalizationItemInfo.Weapon == WeaponType.Bow;

            for (int i = 0; i < m_weaponDropdown.options.Count; i++)
            {
                if ((m_weaponDropdown.options[i] as DropdownWeaponTypeOptionData).Weapon == personalizationItemInfo.Weapon)
                {
                    m_weaponDropdown.value = i;
                    break;
                }
            }

            for (int i = 0; i < m_overrideParentDropdown.options.Count; i++)
            {
                if ((m_overrideParentDropdown.options[i] as DropdownStringOptionData).StringValue == personalizationItemInfo.OverrideParent)
                {
                    m_overrideParentDropdown.value = i;
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
            if (personalizationItemInfo.IsVerified && PersonalizationEditorManager.Instance.canVerifyItems)
                personalizationItemInfo.IsSentForVerification = false;

            personalizationItemInfo.ItemID = m_itemIdField.text;
            personalizationItemInfo.BodyPartName = m_bodyPartDropdown.options[m_bodyPartDropdown.value].text;
        }

        public void OnEditedWeaponTypeDropdown(int value)
        {
            if (m_disallowCallbacks)
                return;

            UIPersonalizationEditor.instance.PropertiesPanel.EditObject(null);

            PersonalizationItemInfo personalizationItemInfo = itemInfo;
            if (personalizationItemInfo == null)
                return;

            WeaponType weaponType = (m_weaponDropdown.options[value] as DropdownWeaponTypeOptionData).Weapon;
            personalizationItemInfo.Weapon = weaponType;

            FirstPersonMover firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
            firstPersonMover.SetEquippedWeaponType(weaponType, false);

            PersonalizationEditorManager manager = PersonalizationEditorManager.Instance;
            manager.SerializeRoot();
            manager.SpawnRootObject();
            UIPersonalizationEditor.instance.PropertiesPanel.EditObjectAgain();

            m_hideBowStrings.interactable = personalizationItemInfo.Category == PersonalizationCategory.WeaponSkins && weaponType == WeaponType.Bow;
            m_overrideParentDropdown.interactable = personalizationItemInfo.Category == PersonalizationCategory.WeaponSkins && personalizationItemInfo.Weapon == WeaponType.Bow;
            m_bowStringsWidth.interactable = personalizationItemInfo.Category == PersonalizationCategory.WeaponSkins && personalizationItemInfo.Weapon == WeaponType.Bow;

            UIElementPersonalizationEditorUtilitiesPanel utils = UIPersonalizationEditor.instance.Utilities;
            utils.SetConditionOptions(PersonalizationEditorManager.Instance.GetConditionOptionsDependingOnEditingWeapon());
        }

        public void OnEditedBodyPartDropdown(int value)
        {
            if (m_disallowCallbacks)
                return;

            UIPersonalizationEditor.instance.PropertiesPanel.EditObject(null);

            PersonalizationItemInfo personalizationItemInfo = itemInfo;
            if (personalizationItemInfo == null)
                return;

            personalizationItemInfo.BodyPartName = m_bodyPartDropdown.options[value].text;

            PersonalizationEditorManager manager = PersonalizationEditorManager.Instance;
            manager.SerializeRoot();
            manager.SpawnRootObject();
            UIPersonalizationEditor.instance.PropertiesPanel.EditObjectAgain();
        }

        public void OnHideBowStringsToggled(bool value)
        {
            if (m_disallowCallbacks)
                return;

            PersonalizationItemInfo personalizationItemInfo = itemInfo;
            if (personalizationItemInfo == null)
                return;

            personalizationItemInfo.HideBowStrings = value;
        }

        public void OnEditedOverrideParentDropdown(int value)
        {
            if (m_disallowCallbacks)
                return;

            PersonalizationItemInfo personalizationItemInfo = itemInfo;
            if (personalizationItemInfo == null)
                return;

            personalizationItemInfo.OverrideParent = (m_overrideParentDropdown.options[value] as DropdownStringOptionData).StringValue;

            PersonalizationEditorManager manager = PersonalizationEditorManager.Instance;
            manager.SerializeRoot();
            manager.SpawnRootObject();
            UIPersonalizationEditor.instance.PropertiesPanel.EditObjectAgain();
        }

        public void OnEditedBowStringsWidth(float value)
        {
            if (m_disallowCallbacks)
                return;

            PersonalizationItemInfo personalizationItemInfo = itemInfo;
            if (personalizationItemInfo == null)
                return;

            personalizationItemInfo.BowStringsWidth = value;

            PersonalizationController controller = PersonalizationEditorManager.Instance.currentPersonalizationController;
            if (controller)
                controller.SetBowStringsWidth(value);
        }

        public void OnVerifyButtonClicked()
        {
            m_verifyButton.interactable = false;
        }
    }
}
