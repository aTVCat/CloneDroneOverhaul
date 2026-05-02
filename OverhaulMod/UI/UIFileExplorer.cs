using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIFileExplorer : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [ShowTooltipOnHighLight("Go up")]
        [UIElementAction(nameof(OnGoUpButtonClicked))]
        [UIElement("GoToParentFolderButton")]
        private readonly Button _goUpButton;

        [UIElementAction(nameof(OnDoneButtonClicked))]
        [UIElement("DoneButton")]
        private readonly Button _doneButton;

        [UIElementAction(nameof(OnCancelButtonClicked))]
        [UIElement("CancelButton")]
        private readonly Button _cancelButton;

        [UIElementAction(nameof(OnDownloadsFolderButtonClicked))]
        [UIElement("DownloadsFolderButton")]
        private readonly Button _downloadsFolderButton;

        [UIElementAction(nameof(OnSearchBoxChanged))]
        [UIElement("SearchBox")]
        private readonly InputField _searchBox;

        [UIElement("ItemDisplayPrefab", false)]
        private readonly ModdedObject _itemDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform _itemDisplayContainer;

        [UIElement("ErrorWindow", false)]
        private readonly GameObject _errorWindow;

        [UIElement("ErrorDescription")]
        private readonly Text _errorText;

        [UIElement("SearchPatternText")]
        private readonly Text _searchPatternText;

        [UIElementAction(nameof(OnEditedDriveDropdown))]
        [UIElement("DriveDropdown")]
        private readonly Dropdown _driveDropdown;

        [UIElementCallback(true)]
        [UIElementAction(nameof(OnEditedPathField))]
        [UIElement("PathField")]
        private readonly InputField _pathField;

        [UIElementAction(nameof(OnRevealPathButtonClicked))]
        [UIElement("RevealPathButton")]
        private readonly Button _revealPathButton;

        private bool _populateNextFrame;

        private List<string> _selectedEntries;

        private string _selectedEntryPath;

        private GameObject _prevSelectedIndicator;

        private Dictionary<string, GameObject> _cachedInstantiatedDisplays;

        public override bool EnableCursor => true;

        public Action<string> singleFileCallback
        {
            get;
            set;
        }

        public Action<List<string>> multipleFilesCallback
        {
            get;
            set;
        }

        public DirectoryInfo currentFolderInfo { get; private set; }

        public string currentFolder
        {
            get
            {
                return currentFolderInfo == null ? null : currentFolderInfo.FullName;
            }
            set
            {
                if (value.IsNullOrEmpty())
                {
                    currentFolderInfo = new DirectoryInfo("C:\\");
                }
                else if (value.Length == 1)
                {
                    currentFolderInfo = new DirectoryInfo($"{value.ToUpper()}:\\");
                }
                else
                {
                    currentFolderInfo = new DirectoryInfo(value);
                }

                _populateNextFrame = true;
                _pathField.text = currentFolderInfo.FullName;

                string rootName = currentFolderInfo.Root.Name;
                for (int i = 0; i < _driveDropdown.options.Count; i++)
                {
                    if (_driveDropdown.options[i].text == rootName)
                    {
                        _driveDropdown.value = i;
                        break;
                    }
                }
            }
        }

        private string _searchPattern;
        public string searchPattern
        {
            get
            {
                return _searchPattern;
            }
            set
            {
                _searchPattern = value;
                _populateNextFrame = true;

                if (value.IsNullOrEmpty())
                    _searchPatternText.text = "All";
                else
                    _searchPatternText.text = value;
            }
        }

        private bool _selectFolder;
        public bool selectFolder
        {
            get => _selectFolder;
            set => _selectFolder = value;
        }

        private bool _selectMany;
        public bool selectMany
        {
            get => _selectMany;
            set => _selectMany = value;
        }

        protected override void OnInitialized()
        {
            _selectedEntries = new List<string>();
            _cachedInstantiatedDisplays = new Dictionary<string, GameObject>();

            _doneButton.interactable = false;
            _searchBox.text = string.Empty;

            _driveDropdown.options.Clear();
            foreach (DriveInfo d in DriveInfo.GetDrives())
            {
                Sprite sprite = ModResources.Sprite(ModAssetBundles.UI, d.Name == "C:\\" ? "SysDrive-Mini-16x16" : "Drive-Mini-16x16");
                _driveDropdown.options.Add(new Dropdown.OptionData(d.Name, sprite));
            }
            _driveDropdown.RefreshShownValue();
        }

        public override void Hide()
        {
            base.Hide();

            _doneButton.interactable = false;
            _selectedEntryPath = null;
            _selectedEntries.Clear();
            singleFileCallback = null;
        }

        public override void Show()
        {
            base.Show();

            _pathField.interactable = false;
            _revealPathButton.gameObject.SetActive(true);
        }

        public override void Update()
        {
            base.Update();
            if (_populateNextFrame)
            {
                _populateNextFrame = false;
                Populate();
            }
        }

        public void Populate()
        {
            _errorWindow.SetActive(false);

            _cachedInstantiatedDisplays.Clear();
            if (_itemDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_itemDisplayContainer);

            string currentDirectory = currentFolder;
            if (currentDirectory.IsNullOrEmpty())
                return;

            string sp = searchPattern;
            if (sp.IsNullOrEmpty())
                sp = "*";

            DirectoryInfo directoryInfo = currentFolderInfo;
            _goUpButton.interactable = directoryInfo.Parent != null;

            List<FileSystemInfo> list = new List<FileSystemInfo>();
            try
            {
                list.AddRange(directoryInfo.GetDirectories());
                list.AddRange(directoryInfo.GetFiles(sp));
            }
            catch (Exception exc)
            {
                ShowError(exc);
                return;
            }

            if (list.IsNullOrEmpty())
                return;

            foreach (FileSystemInfo info in list)
            {
                spawnItem(info);
            }
            OnSearchBoxChanged(_searchBox.text);
        }

        private void spawnItem(FileSystemInfo fileSystemInfo)
        {
            if (fileSystemInfo.Attributes.HasFlag(FileAttributes.Hidden))
                return;

            bool isFolder = fileSystemInfo is DirectoryInfo;
            bool isSelected = fileSystemInfo.FullName == _selectedEntryPath;

            ModdedObject moddedObject = Instantiate(_itemDisplayPrefab, _itemDisplayContainer);
            moddedObject.gameObject.SetActive(true);
            moddedObject.GetObject<Text>(0).text = fileSystemInfo.Name;
            moddedObject.GetObject<GameObject>(1).SetActive(!isFolder);
            moddedObject.GetObject<GameObject>(2).SetActive(isFolder);
            moddedObject.GetObject<GameObject>(3).SetActive(isSelected);
            if (isSelected)
                _prevSelectedIndicator = moddedObject.GetObject<GameObject>(3);

            UIElementFileExplorerItemDisplay itemDisplay = moddedObject.gameObject.AddComponent<UIElementFileExplorerItemDisplay>();
            itemDisplay.InitializeElement();
            itemDisplay.isFolder = isFolder;
            itemDisplay.fullName = fileSystemInfo.FullName;
            itemDisplay.displayName = fileSystemInfo.Name;
            itemDisplay.clickAction = onItemClicked;
            itemDisplay.doubleClickAction = onItemDoubleClicked;

            string text = fileSystemInfo.Name.ToLower();
            while (_cachedInstantiatedDisplays.ContainsKey(text))
                text += "_1";

            _cachedInstantiatedDisplays.Add(text, moddedObject.gameObject);
        }

        public void ShowError(Exception exception)
        {
            _errorWindow.SetActive(true);
            if (exception == null)
            {
                _errorText.text = "Unknown error.";
                return;
            }

            if (exception is SecurityException || exception is UnauthorizedAccessException)
            {
                _errorText.text = "Access denied.";
            }
            else if (exception is DirectoryNotFoundException)
            {
                _errorText.text = "Directory not found.";
            }
            else
            {
                _errorText.text = $"{exception.GetType().Name}.";
            }
        }

        private void onItemClicked(UIElementFileExplorerItemDisplay itemDisplay)
        {
            if (itemDisplay.isFolder != selectFolder)
                return;

            if (!selectMany && _prevSelectedIndicator)
                _prevSelectedIndicator.SetActive(false);

            _prevSelectedIndicator = itemDisplay.ModdedObjectComponent.GetObject<GameObject>(3);
            _selectedEntryPath = itemDisplay.fullName;

            if (selectMany)
            {
                if (_selectedEntries.Contains(itemDisplay.fullName))
                {
                    _selectedEntries.Remove(itemDisplay.fullName);
                    _prevSelectedIndicator.SetActive(false);
                }
                else
                {
                    _selectedEntries.Add(itemDisplay.fullName);
                    _prevSelectedIndicator.SetActive(true);
                }
            }
            else
            {
                _prevSelectedIndicator.SetActive(true);
            }

            _doneButton.interactable = true;
        }

        private void onItemDoubleClicked(UIElementFileExplorerItemDisplay itemDisplay)
        {
            if (itemDisplay.isFolder)
            {
                currentFolder = itemDisplay.fullName;
            }
            else if (!selectFolder && !selectMany)
            {
                OnDoneButtonClicked();
            }
        }

        public void OnEditedDriveDropdown(int value)
        {
            if (_populateNextFrame)
                return;

            currentFolder = _driveDropdown.options[value].text;
        }

        public void OnEditedPathField(string path)
        {
            if (_populateNextFrame)
                return;

            currentFolder = path.Replace("'", string.Empty).Replace("\"", string.Empty);
        }

        public void OnDownloadsFolderButtonClicked()
        {
            currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
        }

        public void OnGoUpButtonClicked()
        {
            if (currentFolderInfo == null || currentFolderInfo.Parent == null)
                return;

            currentFolder = currentFolderInfo.Parent.FullName;
        }

        public void OnDoneButtonClicked()
        {
            if (selectMany)
            {
                multipleFilesCallback?.Invoke(_selectedEntries);
                multipleFilesCallback = null;
            }
            else
            {
                singleFileCallback?.Invoke(_selectedEntryPath);
                singleFileCallback = null;
            }
            Hide();
        }

        public void OnRevealPathButtonClicked()
        {
            _pathField.interactable = true;
            _revealPathButton.gameObject.SetActive(false);
        }

        public void OnCancelButtonClicked()
        {
            if (selectMany)
            {
                multipleFilesCallback?.Invoke(null);
                multipleFilesCallback = null;
            }
            else
            {
                singleFileCallback?.Invoke(null);
                singleFileCallback = null;
            }
            Hide();
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
    }
}
