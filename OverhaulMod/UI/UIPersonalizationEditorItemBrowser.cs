using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditorItemBrowser : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnReloadButtonClicked))]
        [UIElement("ReloadButton")]
        private readonly Button _reloadButton;

        [UIElementAction(nameof(OnFolderButtonClicked))]
        [UIElement("FolderButton")]
        private readonly Button _folderButton;

        [UIElementAction(nameof(OnCreateNewButtonClicked))]
        [UIElement("CreateNewButton")]
        private readonly Button _createNewButton;

        [UIElementAction(nameof(OnImportButtonClicked))]
        [UIElement("ImportButton")]
        private readonly Button _importButton;

        [UIElementAction(nameof(OnViewAllItemsToggleChanged))]
        [UIElement("ViewAllItemsToggle")]
        private readonly Toggle _viewAllItemsToggle;

        [UIElement("UsePersistentDirectoryToggle")]
        private readonly Toggle _usePersistentDirectoryToggle;

        [UIElement("ItemDisplayPrefab", false)]
        private readonly ModdedObject _itemDisplayPrefab;

        [UIElement("LoadErrorDisplayPrefab", false)]
        private readonly ModdedObject _itemLoadErrorDisplayPrefab;

        [UIElement("TextPrefab", false)]
        private readonly Text _textPrefab;

        [UIElement("HeaderPrefab", false)]
        private readonly ModdedObject _headerPrefab;

        [UIElement("Content")]
        private readonly Transform _container;

        [UIElementAction(nameof(OnSearchBoxChanged))]
        [UIElement("SearchBox")]
        private readonly InputField _searchBox;

        private Dictionary<string, GameObject> _cachedInstantiatedDisplays;

        protected override void OnInitialized()
        {
            bool canVerifyItems = PersonalizationEditorManager.Instance.canVerifyItems;

            _cachedInstantiatedDisplays = new Dictionary<string, GameObject>();
            _viewAllItemsToggle.gameObject.SetActive(PersonalizationEditorManager.Instance.canEditNonOwnItems);
            _usePersistentDirectoryToggle.gameObject.SetActive(canVerifyItems);
            _usePersistentDirectoryToggle.isOn = true;
            _importButton.interactable = canVerifyItems;
        }

        public override void Show()
        {
            base.Show();
            Populate();

            _searchBox.ActivateInputField();
        }

        public void Populate()
        {
            _cachedInstantiatedDisplays.Clear();
            if (_container.childCount != 0)
                TransformUtils.DestroyAllChildren(_container);

            PersonalizationItemList itemList = PersonalizationManager.Instance.itemList;
            if (itemList == null)
                return;

            if (!itemList.Items.IsNullOrEmpty())
            {
                bool getAll = _viewAllItemsToggle.isOn && PersonalizationEditorManager.Instance.canEditNonOwnItems;
                List<PersonalizationItemInfo> nonPersistent = new List<PersonalizationItemInfo>();
                List<PersonalizationItemInfo> persistent = new List<PersonalizationItemInfo>();
                foreach (PersonalizationItemInfo item in itemList.Items)
                    if (item.CanBeEdited() || getAll)
                    {
                        if (item.IsPersistentAsset)
                        {
                            persistent.Add(item);
                            continue;
                        }
                        nonPersistent.Add(item);
                    }

                if (!nonPersistent.IsNullOrEmpty())
                {
                    instantiateHeader("Verified items");
                    populate(nonPersistent);
                }
                if (!persistent.IsNullOrEmpty())
                {
                    instantiateHeader("Local items");
                    populate(persistent);
                }

                Text textComponent = Instantiate(_textPrefab, _container);
                textComponent.gameObject.SetActive(true);
                textComponent.text = $"{nonPersistent.Count + persistent.Count} items in total";
            }

            if (!itemList.ItemLoadErrors.IsNullOrEmpty())
                foreach (KeyValuePair<string, System.Exception> keyValue in itemList.ItemLoadErrors)
                {
                    ModdedObject moddedObject = Instantiate(_itemLoadErrorDisplayPrefab, _container);
                    moddedObject.gameObject.SetActive(true);
                    moddedObject.GetObject<Text>(0).text = $"Item load error: {keyValue.Key}";
                    moddedObject.GetObject<Text>(1).text = keyValue.Value.ToString();
                }
        }

        private void populate(List<PersonalizationItemInfo> list)
        {
            if (list != null)
            {
                foreach (PersonalizationItemInfo item in list)
                {
                    ModdedObject moddedObject = Instantiate(_itemDisplayPrefab, _container);
                    moddedObject.gameObject.SetActive(true);
                    moddedObject.GetObject<Text>(0).text = item.Name;
                    moddedObject.GetObject<Text>(1).text = PersonalizationItemInfo.GetCategoryString(item.Category);
                    moddedObject.GetObject<Text>(2).text = item.GetSpecialInfoString();
                    moddedObject.GetObject<GameObject>(3).SetActive(item.IsVerified);
                    moddedObject.GetObject<Text>(4).text = item.GetAuthorsString(false);
                    moddedObject.GetObject<GameObject>(5).SetActive(item.IsExclusive());

                    Button button = moddedObject.GetComponent<Button>();
                    button.onClick.AddListener(delegate
                    {
                        UIPersonalizationEditor.instance.ShowEverything();
                        PersonalizationEditorManager.Instance.EditItem(item, item.FolderPath);
                        Hide();
                    });

                    string text = item.Name.ToLower();
                    while (_cachedInstantiatedDisplays.ContainsKey(text))
                        text += "_1";

                    _cachedInstantiatedDisplays.Add(text, moddedObject.gameObject);
                }

                Text textComponent = Instantiate(_textPrefab, _container);
                textComponent.gameObject.SetActive(true);
                textComponent.text = $"{list.Count} items";
            }
        }

        private void instantiateHeader(string text)
        {
            ModdedObject moddedObject = Instantiate(_headerPrefab, _container);
            moddedObject.gameObject.SetActive(true);
            moddedObject.GetObject<Text>(0).text = text;
        }

        public void OnViewAllItemsToggleChanged(bool value)
        {
            Populate();
        }

        public void OnReloadButtonClicked()
        {
            PersonalizationManager.Instance.itemList.Load();
            Populate();
        }

        public void OnFolderButtonClicked()
        {
            _ = ModFileUtils.OpenFileExplorer(_usePersistentDirectoryToggle.isOn ? ModCore.customizationPersistentFolder : ModCore.customizationFolder);
        }

        public void OnCreateNewButtonClicked()
        {
            UIPersonalizationEditorItemCreationDialog panel = ModUIConstants.ShowPersonalizationEditorItemCreationDialog(base.transform);
            panel.TargetDirectory = _usePersistentDirectoryToggle.isOn ? ModCore.customizationPersistentFolder : ModCore.customizationFolder;
            panel.UsePersistentFolder = _usePersistentDirectoryToggle.isOn;
            panel.ItemCreatedCallback = Hide;

            /*
            ModUIUtils.InputFieldWindow("Create new item", "Enter folder name", 150f, delegate (string str)
            {
                string rootDirectory = _usePersistentDirectoryToggle.isOn ? ModCore.customizationPersistentFolder : ModCore.customizationFolder;
                string directoryName = str.Replace(" ", string.Empty);
                string directoryPath = $"{Path.Combine(rootDirectory, directoryName)}/";
                if (Directory.Exists(directoryPath))
                {
                    ModUIUtils.MessagePopupOK("Directory with the same name already exists", "Try naming the folder differently", true);
                    return;
                }

                if (PersonalizationEditorManager.Instance.CreateItem(str, _usePersistentDirectoryToggle.isOn, out PersonalizationItemInfo personalizationItem))
                {
                    UIPersonalizationEditor.instance.ShowEverything();
                    PersonalizationEditorManager.Instance.EditItem(personalizationItem, personalizationItem.FolderPath);
                    Hide();
                }
                else
                {
                    ModUIUtils.MessagePopupOK("Item creation error", "A folder with the name has been already created.\nTry giving your folder an alternate name.", true);
                }
            });*/
        }

        public void OnSearchBoxChanged(string text)
        {
            string lowerText = text.ToLower();
            bool forceSetEnabled = text.IsNullOrEmpty();

            foreach (KeyValuePair<string, GameObject> keyValue in _cachedInstantiatedDisplays)
            {
                if (forceSetEnabled)
                {
                    keyValue.Value.SetActive(true);
                }
                else
                {
                    keyValue.Value.SetActive(keyValue.Key.Contains(lowerText));
                }
            }
        }

        public void OnImportButtonClicked()
        {
            ModUIUtils.FileExplorer(base.transform, true, delegate (string path)
            {
                string folderName = Path.GetFileNameWithoutExtension(path);
                PersonalizationEditorManager.Instance.ImportItem(path, folderName, out string error, true);
                if (!string.IsNullOrEmpty(error))
                {
                    ModUIUtils.MessagePopupOK("Import error", error, true);
                    return;
                }

                Hide();
            }, null, "*.zip");
        }
    }
}
