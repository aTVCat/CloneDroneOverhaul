using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditorItemImportHelper : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnAddItemClicked))]
        [UIElement("AddItemButton")]
        private readonly Button _addItemButton;

        [UIElementAction(nameof(OnStartVerifyingButtonClicked))]
        [UIElement("StartVerifyingButton")]
        private readonly Button _startVerifyingButton;

        [UIElement("ItemDisplayPrefab", false)]
        private readonly ModdedObject _itemDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform _content;

        [UIElement("Shading", true)]
        private readonly GameObject _shading;

        [UIElement("ImportPanel", true)]
        private readonly GameObject _importPanel;

        [UIElement("TaskPanel", false)]
        private readonly GameObject _taskPanel;

        [UIElement("ProgressFill")]
        private readonly Image _progressFill;

        [UIElement("ProgressText")]
        private readonly Text _progressText;

        [UIElementAction(nameof(OnDeclineButtonClicked))]
        [UIElement("DeclineButton")]
        private readonly Button _declineButton;

        [UIElementAction(nameof(OnVerifyButtonClicked))]
        [UIElement("VerifyButton")]
        private readonly Button _verifyButton;

        [UIElement("DeleteCheckedItemFilesToggle")]
        private readonly Toggle _deleteCheckedFiles;

        private List<string> _files;

        private int _currentItemIndex;

        private bool _isExecutingTasks;

        public override bool closeOnEscapeButtonPress => !_isExecutingTasks;

        protected override void OnInitialized()
        {
            _files = new List<string>();
        }

        public override void Show()
        {
            base.Show();

            _files.Clear();
            populate();

            ShowImportPanel();

            UIDownloadPersonalizationAssetsMenu menu = ModUIConstants.ShowDownloadPersonalizationAssetsMenu(base.transform);
            menu.OnRefreshButtonClicked();
        }

        public void ShowImportPanel()
        {
            _shading.SetActive(true);
            _importPanel.SetActive(true);
            _taskPanel.SetActive(false);
            _isExecutingTasks = false;
        }

        public void ShowTaskPanel()
        {
            _shading.SetActive(false);
            _importPanel.SetActive(false);
            _taskPanel.SetActive(true);
            _isExecutingTasks = true;
        }

        private void refreshTaskProgressBar()
        {
            _progressFill.fillAmount = (_currentItemIndex + 1) / (float)_files.Count;
            _progressText.text = $"{_currentItemIndex + 1}/{_files.Count}";
        }

        private void continueOrEndVerifyingItems()
        {
            if (_deleteCheckedFiles.isOn)
            {
                string path = _files[_currentItemIndex];
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            _currentItemIndex++;
            if (_currentItemIndex >= _files.Count)
            {
                Hide();
                ModUIUtils.MessagePopup(true, "All items verfied!", "Would you like to export all items?", 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "Ok", "Yes", "No", null, exportAllItems);
                return;
            }

            refreshTaskProgressBar();
            importAndEditCurrentItem();
        }

        private void importAndEditCurrentItem()
        {
            string path = _files[_currentItemIndex];
            string folderName = Path.GetFileName(path).Replace("PersonalizationIte_", string.Empty).Remove(8);

            PersonalizationEditorManager.Instance.ImportItem(_files[_currentItemIndex], folderName, out string error, true);
            if (!string.IsNullOrEmpty(error))
            {
                ModUIUtils.MessagePopupOK("Import error", error, true);
                return;
            }
        }

        private void exportAllItems()
        {
            Hide();
            ModUIConstants.ShowPersonalizationEditorExportAllMenu(UIPersonalizationEditor.instance.transform);
        }

        private void onSelectedFiles(List<string> files)
        {
            if (files == null || files.Count == 0) return;

            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];
                if (!_files.Contains(file))
                {
                    _files.Add(file);
                }
            }

            populate();
        }

        private void populate()
        {
            if (_content.childCount != 0)
                TransformUtils.DestroyAllChildren(_content);

            List<string> list = _files;
            if (list == null || list.Count == 0)
            {
                _startVerifyingButton.interactable = false;
                return;
            }

            for (int i = 0; i < list.Count; i++)
            {
                int index = i;

                string file = list[i];
                ModdedObject display = Instantiate(_itemDisplayPrefab, _content);
                display.gameObject.SetActive(true);
                display.GetObject<Text>(2).text = $"{i + 1}.";
                display.GetObject<Text>(0).text = Path.GetFileName(file);
                display.GetObject<Button>(1).onClick.AddListener(delegate
                {
                    _files.RemoveAt(index);
                    populate();
                });
            }
            _startVerifyingButton.interactable = true;
        }

        public void OnAddItemClicked()
        {
            ModUIUtils.FileExplorer(base.transform, true, onSelectedFiles, null, "*.zip");
        }

        public void OnStartVerifyingButtonClicked()
        {
            ShowTaskPanel();

            _currentItemIndex = 0;
            refreshTaskProgressBar();
            importAndEditCurrentItem();
        }

        public void OnDeclineButtonClicked()
        {
            PersonalizationItemInfo info = PersonalizationEditorManager.Instance.currentEditingItemInfo;
            info.IsVerified = false;
            info.IsSentForVerification = false;
            info.ReuploadedTheItem = false;

            PersonalizationEditorManager.Instance.SaveItem(out string error, true);
            if (!string.IsNullOrEmpty(error))
            {
                UIPersonalizationEditor.instance.ShowSaveErrorMessage(error);
                return;
            }

            continueOrEndVerifyingItems();
        }

        public void OnVerifyButtonClicked()
        {
            PersonalizationItemInfo info = PersonalizationEditorManager.Instance.currentEditingItemInfo;
            info.IsVerified = true;
            info.IsSentForVerification = false;
            info.ReuploadedTheItem = false;

            PersonalizationEditorManager.Instance.SaveItem(out string error, true);
            if (!string.IsNullOrEmpty(error))
            {
                UIPersonalizationEditor.instance.ShowSaveErrorMessage(error);
                return;
            }

            continueOrEndVerifyingItems();
        }
    }
}
