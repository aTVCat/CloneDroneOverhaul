using OverhaulMod.Combat;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationEditorItemConfigPanel : OverhaulUIBehaviour
    {
        private const float GENERIC_INFO_GROUP_HEIGHT_FOR_ACCESSORY = 280f;

        [UIElement("NameField")]
        private readonly InputField _nameField;

        [UIElement("DescriptionField")]
        private readonly InputField _descriptionField;

        [UIElement("charLeftText_Description")]
        private readonly Text _descriptionFieldCharsLeftText;

        [UIElement("NewEditorIdField")]
        private readonly InputField _editorIdField;

        [UIElement("NewItemIdField")]
        private readonly InputField _itemIdField;

        [UIElement("VersionField")]
        private readonly InputField _versionField;

        [UIElement("VerifiedToggle")]
        private readonly Toggle _verifiedToggle;

        [UIElement("SentToVerificationToggle")]
        private readonly Toggle _sentToVerificationToggle;

        [UIElement("ReuploadedToggle")]
        private readonly Toggle _reuploadedToggle;

        [UIElement("ExportedFileNameText")]
        private readonly Text _exportedFileNameText;

        [UIElementAction(nameof(OnExportButtonClicked))]
        [UIElement("ExportButton")]
        private readonly Button _exportButton;

        [UIElementAction(nameof(OnSavesFolderButtonClicked))]
        [UIElement("SavesFolderButton")]
        private readonly Button _savesFolderButton;

        [UIElementAction(nameof(OnItemFolderFolderButtonClicked))]
        [UIElement("ItemFolderButton")]
        private readonly Button _itemFolderButton;

        [UIElementAction(nameof(OnRevealEditorIDButtonClicked))]
        [UIElement("RevealEditorIDButton")]
        private readonly Button _revealEditorIDButton;

        [UIElementAction(nameof(OnEditedWeaponTypeDropdown))]
        [UIElement("WeaponDropdown")]
        private readonly Dropdown _weaponDropdown;

        [UIElementAction(nameof(OnEditedBodyPartDropdown))]
        [UIElement("BodyPartDropdown")]
        private readonly Dropdown _bodyPartDropdown;

        [UIElementAction(nameof(OnVerifyButtonClicked))]
        [UIElement("VerifyButton")]
        private readonly Button _verifyButton;

        [UIElementAction(nameof(OnHideBowStringsToggled))]
        [UIElement("HideBowStringsToggle")]
        private readonly Toggle _hideBowStrings;

        [UIElementAction(nameof(OnEditedOverrideParentDropdown))]
        [UIElement("OverrideParentDropdown")]
        private readonly Dropdown _overrideParentDropdown;

        [UIElementAction(nameof(OnEditedBowStringsWidth))]
        [UIElement("BowStringsWidthSlider")]
        private readonly Slider _bowStringsWidth;

        [UIElement("ExclusiveForField", typeof(UIElementPersonalizationExclusiveForField))]
        private readonly UIElementPersonalizationExclusiveForField _exclusiveForField;

        [UIElement("AuthorField", typeof(UIElementPersonalizationAuthorsField))]
        private readonly UIElementPersonalizationAuthorsField _authorField;

        [UIElement("HierarchyGroup", typeof(UIElementPersonalizationEditorHierarchyPanel))]
        private readonly UIElementPersonalizationEditorHierarchyPanel _hierarchyPanel;

        [UIElement("ImportedFilesGroup", typeof(UIElementPersonalizationEditorFileImportPanel))]
        private readonly UIElementPersonalizationEditorFileImportPanel _filesPanel;

        [UIElement("SpecialInfoGroup")]
        private readonly GameObject _specialInfoPanel;

        [UIElement("GenericInfoGroup")]
        private readonly RectTransform _generalInfoPanel;

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
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.ShieldSkins))
                weaponList.Add(new DropdownWeaponTypeOptionData(WeaponType.Shield));
            weaponList.Add(new DropdownWeaponTypeOptionData(ModWeaponsManager.SCYTHE_TYPE));
            _weaponDropdown.RefreshShownValue();

            List<Dropdown.OptionData> overrideParentList = _overrideParentDropdown.options;
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
            _overrideParentDropdown.RefreshShownValue();

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

            RefreshGeneralInfoPanel();

            _disallowCallbacks = true;
            itemInfo.FixValues();
            _nameField.text = itemInfo.Name;
            _descriptionField.text = itemInfo.Description;
            _authorField.referenceList = itemInfo.Authors;
            _exclusiveForField.referenceList = itemInfo.ExclusiveFor_V2;
            _verifyButton.interactable = !itemInfo.IsVerified;
            _hierarchyPanel.itemInfo = itemInfo;
            _filesPanel.itemInfo = itemInfo;
            _hideBowStrings.isOn = itemInfo.HideBowStrings;
            _hideBowStrings.interactable = itemInfo.Category == PersonalizationCategory.WeaponSkins && itemInfo.Weapon == WeaponType.Bow;
            _overrideParentDropdown.interactable = itemInfo.Category == PersonalizationCategory.WeaponSkins && itemInfo.Weapon == WeaponType.Bow;
            _bowStringsWidth.value = itemInfo.BowStringsWidth;
            _bowStringsWidth.interactable = itemInfo.Category == PersonalizationCategory.WeaponSkins && itemInfo.Weapon == WeaponType.Bow;

            _editorIdField.text = itemInfo.EditorID;
            _itemIdField.text = itemInfo.ItemID;
            _versionField.text = itemInfo.Version.ToString();
            _sentToVerificationToggle.isOn = itemInfo.IsSentForVerification;
            _verifiedToggle.isOn = itemInfo.IsVerified;
            _reuploadedToggle.isOn = itemInfo.ReuploadedTheItem;
            _exportedFileNameText.text = $"Will be exported as:\n{PersonalizationEditorDataManager.Instance.GetExportedItemFileName(itemInfo)}";

            _editorIdField.interactable = false;
            _revealEditorIDButton.gameObject.SetActive(true);

            _specialInfoPanel.SetActive(itemInfo.Category == PersonalizationCategory.WeaponSkins);
            _weaponDropdown.gameObject.SetActive(itemInfo.Category == PersonalizationCategory.WeaponSkins);
            _bodyPartDropdown.gameObject.SetActive(itemInfo.Category == PersonalizationCategory.Accessories);

            for (int i = 0; i < _weaponDropdown.options.Count; i++)
            {
                if ((_weaponDropdown.options[i] as DropdownWeaponTypeOptionData).Weapon == itemInfo.Weapon)
                {
                    _weaponDropdown.value = i;
                    break;
                }
            }

            for (int i = 0; i < _overrideParentDropdown.options.Count; i++)
            {
                if ((_overrideParentDropdown.options[i] as DropdownStringOptionData).StringValue == itemInfo.OverrideParent)
                {
                    _overrideParentDropdown.value = i;
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

        public void ApplyValues(bool ignoreDevPanel = false)
        {
            PersonalizationItemInfo itemInfo = EditingItemInfo;
            if (itemInfo == null) return;

            itemInfo.FixValues();
            itemInfo.Name = _nameField.text;
            itemInfo.Description = _descriptionField.text;
            itemInfo.EditorID = _editorIdField.text;
            itemInfo.ItemID = _itemIdField.text;
            itemInfo.BodyPartName = _bodyPartDropdown.options[_bodyPartDropdown.value].text;

            if (PersonalizationEditorManager.Instance.CanVerifyItems && !ignoreDevPanel)
            {
                itemInfo.IsVerified = _verifiedToggle.isOn;
                if (itemInfo.IsVerified)
                {
                    itemInfo.IsSentForVerification = false;
                    itemInfo.ReuploadedTheItem = false;
                    _sentToVerificationToggle.isOn = false;
                    _reuploadedToggle.isOn = false;
                }

                int prevVersion = itemInfo.Version;
                if (!int.TryParse(_versionField.text, out int version))
                    version = prevVersion;

                itemInfo.Version = version;
            }
        }

        public void RefreshHierarchyPanel()
        {
            _hierarchyPanel.Populate();
        }

        public void RefreshGeneralInfoPanel()
        {
            float height;
            switch (EditingItemInfo.Category)
            {
                case PersonalizationCategory.Accessories:
                    height = GENERIC_INFO_GROUP_HEIGHT_FOR_ACCESSORY;
                    break;
                default:
                    height = GENERIC_INFO_GROUP_HEIGHT_FOR_ACCESSORY - 30f;
                    break;
            }

            RectTransform rectTransform = _generalInfoPanel;
            Vector2 size = rectTransform.sizeDelta;
            size.y = height;
            rectTransform.sizeDelta = size;

            _editOffsetsButton.gameObject.SetActive(EditingItemInfo.Category == PersonalizationCategory.Accessories);
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
            if (_disallowCallbacks)
                return;

            PersonalizationItemInfo personalizationItemInfo = EditingItemInfo;
            if (personalizationItemInfo == null)
                return;

            WeaponType weaponType = (_weaponDropdown.options[value] as DropdownWeaponTypeOptionData).Weapon;
            personalizationItemInfo.Weapon = weaponType;

            FirstPersonMover firstPersonMover = PersonalizationEditorManager.Instance.GetBot();
            if (firstPersonMover)
                firstPersonMover.SetEquippedWeaponType(weaponType, false);

            PersonalizationEditorManager manager = PersonalizationEditorManager.Instance;
            manager.SerializeRoot();
            manager.SpawnRootObject();
            UIPersonalizationEditor.instance.PropertiesPanel.EditObjectAgain();

            _hideBowStrings.interactable = personalizationItemInfo.Category == PersonalizationCategory.WeaponSkins && weaponType == WeaponType.Bow;
            _overrideParentDropdown.interactable = personalizationItemInfo.Category == PersonalizationCategory.WeaponSkins && personalizationItemInfo.Weapon == WeaponType.Bow;
            _bowStringsWidth.interactable = personalizationItemInfo.Category == PersonalizationCategory.WeaponSkins && personalizationItemInfo.Weapon == WeaponType.Bow;

            UIElementPersonalizationEditorUtilitiesPanel utils = UIPersonalizationEditor.instance.Utilities;
            utils.SetAvailablePresets(PersonalizationEditorManager.Instance.GetPresetsForEditingWeaponSkin());
            utils.EnableAnimation();
        }

        public void OnEditedBodyPartDropdown(int value)
        {
            if (_disallowCallbacks)
                return;

            PersonalizationItemInfo personalizationItemInfo = EditingItemInfo;
            if (personalizationItemInfo == null)
                return;

            personalizationItemInfo.BodyPartName = _bodyPartDropdown.options[value].text;

            PersonalizationEditorManager manager = PersonalizationEditorManager.Instance;
            manager.SerializeRoot();
            manager.SpawnRootObject();
            UIPersonalizationEditor.instance.PropertiesPanel.EditObjectAgain();
            UIPersonalizationEditor.instance.Utilities.EnableAnimation();
        }

        public void OnHideBowStringsToggled(bool value)
        {
            if (_disallowCallbacks)
                return;

            PersonalizationItemInfo personalizationItemInfo = EditingItemInfo;
            if (personalizationItemInfo == null)
                return;

            personalizationItemInfo.HideBowStrings = value;
        }

        public void OnEditedOverrideParentDropdown(int value)
        {
            if (_disallowCallbacks)
                return;

            PersonalizationItemInfo personalizationItemInfo = EditingItemInfo;
            if (personalizationItemInfo == null)
                return;

            personalizationItemInfo.OverrideParent = (_overrideParentDropdown.options[value] as DropdownStringOptionData).StringValue;

            PersonalizationEditorManager manager = PersonalizationEditorManager.Instance;
            manager.SerializeRoot();
            manager.SpawnRootObject();
            UIPersonalizationEditor.instance.PropertiesPanel.EditObjectAgain();
        }

        public void OnEditedBowStringsWidth(float value)
        {
            if (_disallowCallbacks)
                return;

            PersonalizationItemInfo personalizationItemInfo = EditingItemInfo;
            if (personalizationItemInfo == null)
                return;

            personalizationItemInfo.BowStringsWidth = value;

            PersonalizationController controller = PersonalizationEditorManager.Instance.PreviewingPersonalizationController;
            if (controller)
                controller.SetBowStringsWidth(value);
        }

        public void OnVerifyButtonClicked()
        {
            _verifyButton.interactable = false;
        }

        public void OnExportButtonClicked()
        {
            PersonalizationItemSaveResult result = PersonalizationEditorManager.Instance.SaveItem();
            if (result.HasFailed())
            {
                ModUIUtils.MessagePopupOK("Error", result.Error, true);
            }
            else
            {
                PersonalizationItemInfo itemInfo = PersonalizationEditorManager.Instance.EditingItemInfo;
                PersonalizationEditorDataManager.Instance.ExportItem(itemInfo, out _, ModCore.SavesFolder, PersonalizationEditorDataManager.Instance.GetExportedItemFileName(itemInfo));
                ModUIUtils.MessagePopupOK("Exported the item", "for real", false);
            }
        }

        public void OnSavesFolderButtonClicked()
        {
            ModFileUtils.OpenFileExplorer(ModCore.SavesFolder);
        }

        public void OnItemFolderFolderButtonClicked()
        {
            ModFileUtils.OpenFileExplorer(PersonalizationEditorManager.Instance.EditingItemInfo.FolderPath);
        }

        public void OnRevealEditorIDButtonClicked()
        {
            _editorIdField.interactable = true;
            _revealEditorIDButton.gameObject.SetActive(false);
        }
    }
}
