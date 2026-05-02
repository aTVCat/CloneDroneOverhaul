using OverhaulMod.Content;
using OverhaulMod.Utils;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementAddonEditorDownloadDisplay : OverhaulUIBehaviour
    {
        [UIElement("AddonReferenceExistIcon", false)]
        private readonly GameObject _addonReferenceExistIconObject;

        [UIElement("AddonIDField")]
        private readonly InputField _addonIdField;

        [UIElementAction(nameof(OnRefreshAddonReferenceButtonClicked))]
        [UIElement("RefreshAddonReferenceButton")]
        private readonly Button _refreshAddonReferenceButton;

        [UIElement("PackageURLField")]
        private readonly InputField _packageUrlField;

        [UIElement("PackageCompressedFilePathField")]
        private readonly InputField _packageCompressedFilePathField;

        [UIElement("PackageSizeField")]
        private readonly InputField _packageSizeField;

        [UIElementAction(nameof(OnCalculatePackageSizeButtonClicked))]
        [UIElement("CalculatePackageSizeButton")]
        private readonly Button _calculatePackageSizeButton;

        [UIElementAction(nameof(OnAddImageButtonClicked))]
        [UIElement("AddImageButton")]
        private readonly Button _addImageButton;

        [UIElement("ImageEntry", false)]
        private readonly ModdedObject _imageEntryPrefab;

        [UIElement("Content")]
        private readonly Transform _imageEntriesContainer;

        private AddonDownloadInfo _addonDownloadInfo;

        private AddonDownloadListInfo _addonDownloadListInfo;

        private UIAddonsDownloadEditor _addonDownloadEditor;

        public void Initialize(AddonDownloadInfo addonDownloadInfo, AddonDownloadListInfo addonDownloadListInfo)
        {
            base.InitializeAsElement();
            _addonDownloadInfo = addonDownloadInfo;
            _addonDownloadListInfo = addonDownloadListInfo;
            _addonIdField.text = addonDownloadInfo.UniqueID;
            _packageUrlField.text = addonDownloadInfo.PackageFileURL;
            _packageSizeField.text = addonDownloadInfo.PackageFileSize.ToString();

            refreshImageEntries();
        }

        public void UpdateAddonDownloadInfo()
        {
            _addonDownloadInfo.UniqueID = _addonIdField.text;
            _addonDownloadInfo.PackageFileURL = _packageUrlField.text;
            _addonDownloadInfo.PackageFileSize = int.Parse(_packageSizeField.text);
        }

        private void refreshAddonReferencePresence()
        {
            _addonReferenceExistIconObject.SetActive(_addonDownloadInfo.HasAddonInfo());
        }

        private void refreshImageEntries()
        {
            if (_imageEntriesContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_imageEntriesContainer);

            System.Collections.Generic.List<string> list = _addonDownloadInfo.Images;
            if (list.IsNullOrEmpty())
                return;

            for (int i = 0; i < list.Count; i++)
            {
                int index = i;

                ModdedObject moddedObject = Instantiate(_imageEntryPrefab, _imageEntriesContainer);
                moddedObject.gameObject.SetActive(true);

                InputField inputField = moddedObject.GetObject<InputField>(0);
                inputField.text = list[i];
                inputField.onEndEdit.AddListener(delegate (string text)
                {
                    list[index] = text;
                });

                Button button = moddedObject.GetObject<Button>(1);
                button.onClick.AddListener(delegate
                {
                    list.RemoveAt(index);
                    refreshImageEntries();
                });
            }
        }

        public void OnRefreshAddonReferenceButtonClicked()
        {
            _addonDownloadInfo.Addon = AddonManager.Instance.GetAddonInfo(_addonIdField.text);
            refreshAddonReferencePresence();
        }

        public void OnCalculatePackageSizeButtonClicked()
        {
            string path = _packageCompressedFilePathField.text;
            if (!File.Exists(path))
            {
                _packageSizeField.text = "0";
                return;
            }

            FileInfo fileInfo = new FileInfo(path);
            _packageSizeField.text = fileInfo.Length.ToString();
        }

        public void OnDeleteButtonClicked()
        {
            ModUIUtils.MessagePopup(true, $"Delete \"{(_addonDownloadInfo.Addon != null ? _addonDownloadInfo.GetDisplayName() : _addonDownloadInfo.UniqueID)}\"?", "so what", 125f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
            {
                _addonDownloadEditor.RemoveEntry(this);
                _addonDownloadListInfo.Addons.Remove(_addonDownloadInfo);
                Destroy(base.gameObject);
            });
        }

        public void OnAddImageButtonClicked()
        {
            if (_addonDownloadInfo.Images == null)
                _addonDownloadInfo.Images = new System.Collections.Generic.List<string>();

            if (_addonDownloadInfo.Images.Contains(string.Empty))
                return;

            _addonDownloadInfo.Images.Add(string.Empty);
            refreshImageEntries();
        }
    }
}
