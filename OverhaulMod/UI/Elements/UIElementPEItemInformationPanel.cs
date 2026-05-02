using OverhaulMod.Content.Personalization;
using OverhaulMod.Gameplay;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPEItemInformationPanel : OverhaulUIBehaviour
    {
        [UIElement("NameField")]
        private readonly InputField _nameField;

        [UIElement("AuthorField", typeof(UIElementPEAuthorsField))]
        private readonly UIElementPEAuthorsField _authorField;

        [UIElement("DescriptionField")]
        private readonly InputField _descriptionField;

        [UIElement("charLeftText_Description")]
        private readonly Text _descriptionFieldCharsLeftText;

        [UIElementAction(nameof(OnEditedWeaponTypeDropdown))]
        [UIElement("WeaponDropdown")]
        private readonly Dropdown _weaponDropdown;

        [UIElementAction(nameof(OnEditedBodyPartDropdown))]
        [UIElement("BodyPartDropdown")]
        private readonly Dropdown _bodyPartDropdown;

        [UIElementAction(nameof(OnEditOffsetsButtonClicked))]
        [UIElement("EditOffsetsButton")]
        private readonly Button _editOffsetsButton;

        private bool _disallowCallbacks;

        public PersonalizationItemInfo EditingItemInfo
        {
            get => PersonalizationEditorManager.Instance.EditingItemInfo;
        }

        protected override void OnInitialized()
        {
            _descriptionField.onValueChanged.AddListener(OnDescriptionFieldChanged);

            List<Dropdown.OptionData> weaponList = _weaponDropdown.options;
            weaponList.Clear();
            weaponList.Add(new DropdownWeaponTypeOptionData(WeaponType.Sword));
            weaponList.Add(new DropdownWeaponTypeOptionData(WeaponType.Bow));
            weaponList.Add(new DropdownWeaponTypeOptionData(WeaponType.Hammer));
            weaponList.Add(new DropdownWeaponTypeOptionData(WeaponType.Spear));
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.ShieldSkins)) weaponList.Add(new DropdownWeaponTypeOptionData(WeaponType.Shield));
            weaponList.Add(new DropdownWeaponTypeOptionData(ModWeaponsManager.SCYTHE_TYPE));
            _weaponDropdown.RefreshShownValue();

            List<Dropdown.OptionData> bodyPartList = _bodyPartDropdown.options;
            bodyPartList.Clear();
            foreach (string bp in PersonalizationManager.SupportedBodyParts)
            {
                bodyPartList.Add(new Dropdown.OptionData(bp));
            }
            _bodyPartDropdown.RefreshShownValue();
        }

        public void Populate()
        {
            PersonalizationItemInfo itemInfo = EditingItemInfo;
            if (itemInfo == null) return;

            _disallowCallbacks = true;
            _nameField.text = itemInfo.Name;
            _descriptionField.text = itemInfo.Description;
            _authorField.referenceList = itemInfo.Authors;

            _weaponDropdown.gameObject.SetActive(itemInfo.Category == PersonalizationCategory.WeaponSkins);
            _bodyPartDropdown.gameObject.SetActive(itemInfo.Category == PersonalizationCategory.Accessories);
            _editOffsetsButton.gameObject.SetActive(itemInfo.Category == PersonalizationCategory.Accessories);

            for (int i = 0; i < _weaponDropdown.options.Count; i++)
            {
                if ((_weaponDropdown.options[i] as DropdownWeaponTypeOptionData).Weapon == itemInfo.Weapon)
                {
                    _weaponDropdown.value = i;
                    break;
                }
            }

            for (int i = 0; i < _bodyPartDropdown.options.Count; i++)
            {
                string text = _bodyPartDropdown.options[i].text;
                if (text == itemInfo.BodyPartName)
                {
                    _bodyPartDropdown.value = i;
                    break;
                }
            }

            FirstPersonMover firstPersonMover = PersonalizationEditorManager.Instance.GetBot();
            firstPersonMover.SetEquippedWeaponType(itemInfo.Weapon, false);

            _disallowCallbacks = false;
        }

        public void ApplyValues()
        {
            PersonalizationItemInfo itemInfo = EditingItemInfo;
            if (itemInfo == null) return;

            itemInfo.FixValues();
            itemInfo.Name = _nameField.text;
            itemInfo.Description = _descriptionField.text;
            itemInfo.BodyPartName = _bodyPartDropdown.options[_bodyPartDropdown.value].text;
        }

        private string getCharLeftTextForField(InputField inputField)
        {
            return $"{inputField.characterLimit - inputField.text.Length} {LocalizationManager.Instance.GetTranslatedString("charsleft")}";
        }

        public void OnDescriptionFieldChanged(string text)
        {
            _descriptionFieldCharsLeftText.text = getCharLeftTextForField(_descriptionField);
        }

        public void OnEditedWeaponTypeDropdown(int value)
        {
            if (_disallowCallbacks) return;

            PersonalizationItemInfo itemInfo = EditingItemInfo;
            if (itemInfo == null) return;

            WeaponType weaponType = (_weaponDropdown.options[value] as DropdownWeaponTypeOptionData).Weapon;
            itemInfo.Weapon = weaponType;

            FirstPersonMover firstPersonMover = PersonalizationEditorManager.Instance.GetBot();
            if (firstPersonMover)
                firstPersonMover.SetEquippedWeaponType(weaponType, false);

            PersonalizationEditorManager manager = PersonalizationEditorManager.Instance;
            manager.SerializeRoot();
            manager.SpawnRootObject();
            UIPE.Instance.Inspector.EditObjectAgain();

            UIElementPEUtilitiesPane utils = UIPE.Instance.Utilities;
            utils.OnItemSelected();
        }

        public void OnEditedBodyPartDropdown(int value)
        {
            if (_disallowCallbacks) return;

            PersonalizationItemInfo itemInfo = EditingItemInfo;
            if (itemInfo == null) return;

            itemInfo.BodyPartName = _bodyPartDropdown.options[value].text;

            PersonalizationEditorManager manager = PersonalizationEditorManager.Instance;
            manager.SerializeRoot();
            manager.SpawnRootObject();
            UIPE.Instance.Inspector.EditObjectAgain();

            UIElementPEUtilitiesPane utils = UIPE.Instance.Utilities;
            utils.OnItemSelected();
        }

        public void OnEditOffsetsButtonClicked()
        {
            UIPE.Instance.ShowItemOffsets();
        }
    }
}