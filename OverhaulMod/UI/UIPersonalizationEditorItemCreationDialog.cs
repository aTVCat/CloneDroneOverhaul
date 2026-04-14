using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditorItemCreationDialog : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnWeaponSkinButtonClicked))]
        [UIElement("WeaponSkinButton")]
        private readonly Button _weaponSkinButton;

        [UIElementAction(nameof(OnAccessoryButtonClicked))]
        [UIElement("AccessoryButton")]
        private readonly Button _accessoryButton;

        [UIElementAction(nameof(OnPetButtonClicked))]
        [UIElement("PetButton")]
        private readonly Button _petButton;

        [UIElementAction(nameof(OnDoneButtonClicked))]
        [UIElement("DoneButton")]
        private readonly Button _doneButton;

        [UIElementAction(nameof(OnItemNameChanged))]
        [UIElement("ItemNameField")]
        private readonly InputField _itemNameField;

        [UIElement("TemplateDropdown")]
        private readonly Dropdown _templateDropdown;

        [UIElement("StatusText")]
        private readonly Text _statusText;

        public bool UsePersistentFolder;

        public Action ItemCreatedCallback;

        private float _timeLeftToRefreshStatus;

        private string _generatedId;

        private string _folderName;

        private PersonalizationCategory _category;

        protected override void OnInitialized()
        {
            System.Collections.Generic.List<Dropdown.OptionData> options = _templateDropdown.options;
            options.Clear();
            options.Add(new Dropdown.OptionData("None"));

            PersonalizationItemInfo[] templates = PersonalizationEditorTemplateManager.Instance.GetTemplates();
            if (templates != null)
                foreach (PersonalizationItemInfo template in templates)
                {
                    if (!template.Corrupted)
                        options.Add(new DropdownPersonalizationItemInfo(template));
                }

            _templateDropdown.options = options;
            _templateDropdown.value = 0;

            _accessoryButton.gameObject.SetActive(ModFeatures.IsEnabled(ModFeatures.FeatureType.Accessories));
            _petButton.gameObject.SetActive(ModFeatures.IsEnabled(ModFeatures.FeatureType.Pets));
        }

        public override void Show()
        {
            base.Show();

            _generatedId = Guid.NewGuid().ToString().Remove(8);

            _templateDropdown.value = 0;
            _itemNameField.text = string.Empty;

            _category = PersonalizationCategory.None;
            refreshCategoryElements();

            ScheduleRefreshingStatus();
        }

        public override void Update()
        {
            if (_timeLeftToRefreshStatus == -1f) return;

            _timeLeftToRefreshStatus = Mathf.Max(0f, _timeLeftToRefreshStatus - Time.unscaledDeltaTime);
            if (_timeLeftToRefreshStatus == 0f)
            {
                _timeLeftToRefreshStatus = -1f;

                RefreshStatus();
            }
        }

        private void refreshCategoryElements()
        {
            _weaponSkinButton.interactable = _category != PersonalizationCategory.WeaponSkins;
            _accessoryButton.interactable = _category != PersonalizationCategory.Accessories;
            _petButton.interactable = _category != PersonalizationCategory.Pets;

            _templateDropdown.gameObject.SetActive(_category == PersonalizationCategory.WeaponSkins);
        }

        public void ScheduleRefreshingStatus()
        {
            SetStatusText("Checking...", Color.gray);
            _doneButton.interactable = false;

            _timeLeftToRefreshStatus = 1f;
        }

        public void RefreshStatus()
        {
            if(_category == PersonalizationCategory.None)
            {
                SetStatusText("You have not chosen the category.", Color.red);
                _doneButton.interactable = false;
                return;
            }

            if (_itemNameField.text.IsNullOrEmpty())
            {
                SetStatusText("The name is empty.", Color.red);
                _doneButton.interactable = false;
                return;
            }

            if (_itemNameField.text.IsNullOrWhiteSpace())
            {
                SetStatusText("The name is whitespace.", Color.red);
                _doneButton.interactable = false;
                return;
            }

            if (_itemNameField.text.EndsWith(" "))
            {
                SetStatusText("The name ends with whitespace.", Color.red);
                _doneButton.interactable = false;
                return;
            }

            if (_folderName.IsNullOrEmpty())
            {
                SetStatusText("Folder name is empty.", Color.red);
                _doneButton.interactable = false;
                return;
            }

            if (_folderName.IsNullOrWhiteSpace())
            {
                SetStatusText("Folder name is a whitespace.", Color.red);
                _doneButton.interactable = false;
                return;
            }

            if (_folderName.Contains(" "))
            {
                SetStatusText("Folder name contains whitespaces.", Color.red);
                _doneButton.interactable = false;
                return;
            }

            if (ModFileUtils.HasUnsupportedCharacters(_folderName, out char character))
            {
                SetStatusText($"The name contains invalid character: {character}", Color.red);
                _doneButton.interactable = false;
                return;
            }

            foreach (char c in Path.GetInvalidFileNameChars())
                if (_folderName.Contains(c))
                {
                    SetStatusText($"The name contains invalid character: {c}", Color.red);
                    _doneButton.interactable = false;
                    return;
                }

            string path = Path.Combine(UsePersistentFolder ? ModCore.CustomizationPersistentFolder : ModCore.CustomizationFolder, _folderName);
            if (Directory.Exists(path))
            {
                SetStatusText("A folder with the same name already exists.", Color.red);
                _doneButton.interactable = false;
                return;
            }

            SetStatusText("You can create the item.", Color.green);
            _doneButton.interactable = true;
        }

        public void SetStatusText(string text, Color color)
        {
            _statusText.text = text;
            _statusText.color = color;
        }

        public void OnWeaponSkinButtonClicked()
        {
            _category = PersonalizationCategory.WeaponSkins;
            refreshCategoryElements();
            ScheduleRefreshingStatus();
        }

        public void OnAccessoryButtonClicked()
        {
            _category = PersonalizationCategory.Accessories;
            refreshCategoryElements();
            ScheduleRefreshingStatus();
        }

        public void OnPetButtonClicked()
        {
            _category = PersonalizationCategory.Pets;
            refreshCategoryElements();
            ScheduleRefreshingStatus();
        }

        public void OnDoneButtonClicked()
        {
            Hide();

            PersonalizationItemInfo template = null;
            if (_templateDropdown.options[_templateDropdown.value] is DropdownPersonalizationItemInfo dropdownPersonalizationItemInfo)
                template = dropdownPersonalizationItemInfo.ItemInfo;

            PersonalizationItemCreationArgs creationArgs = new PersonalizationItemCreationArgs()
            {
                UsePersistentFolder = UsePersistentFolder,
                DirectoryName = _folderName,
                ItemName = _itemNameField.text,
                UniqueID = _generatedId,
                ItemCategory = _category,
                Template = template,
            };
            PersonalizationItemCreationResult creationResult = PersonalizationEditorDataManager.Instance.CreateItem(creationArgs);
            if (creationResult.HasFailed())
            {
                ModUIUtils.MessagePopupOK("Item creation error", "A folder with the name has been already created.\nTry giving your folder an alternate name.", true);
            }
            else
            {
                UIPersonalizationEditor.instance.ShowEverything();
                PersonalizationEditorManager.Instance.EditItem(creationResult.NewItem);
                Hide();

                if (ItemCreatedCallback != null)
                {
                    ItemCreatedCallback();
                    ItemCreatedCallback = null;
                }
            }
        }

        public void OnItemNameChanged(string value)
        {
            ScheduleRefreshingStatus();
            string itemName = value;

            bool isDone = false;
            while (!isDone)
            {
                int index = itemName.IndexOf(' ');
                if (index == -1)
                    isDone = true;
                else
                {
                    itemName = itemName.Remove(index, 1);
                    if (index < itemName.Length)
                    {
                        string upperChar = itemName[index].ToString().ToUpper();
                        itemName = itemName.Remove(index, 1);
                        itemName = itemName.Insert(index, upperChar);
                    }
                }
            }
            _folderName = $"{_generatedId}_{itemName}";
        }
    }
}
