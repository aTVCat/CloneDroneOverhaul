using OverhaulMod.Content;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAddonsDownloadEditor : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElement("AddonDisplay", false)]
        private readonly ModdedObject _addonDisplay;

        [UIElement("Content")]
        private readonly Transform _container;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button _saveButton;

        [UIElementAction(nameof(OnNewAddonButtonClicked))]
        [UIElement("NewAddonButton")]
        private readonly Button _newAddonButton;

        [UIElementAction(nameof(OnGetInfoFileClicked))]
        [UIElement("GetInfoFileButton")]
        private readonly Button _getInfoFileButton;

        [UIElementAction(nameof(OnSavesFolderButtonClicked))]
        [UIElement("SavesFolderButton")]
        private readonly Button _savesFolderButton;

        [UIElement("NeedsSaveIcon", false)]
        private readonly GameObject _needsSaveIcon;

        private List<UIElementAddonEditorDownloadDisplay> _instantiatedEntries;

        protected override void OnInitialized()
        {
            _instantiatedEntries = new List<UIElementAddonEditorDownloadDisplay>();
            _saveButton.interactable = false;
            _newAddonButton.interactable = false;
        }

        private void onGotDownloadList(AddonManager.GetDownloadListResult result)
        {
            if (result.HasFailed())
            {
                _getInfoFileButton.interactable = true;
                ModUIUtils.MessagePopupOK("Error", result.Error, false);
                return;
            }

            _saveButton.interactable = true;
            _newAddonButton.interactable = true;

            _instantiatedEntries.Clear();
            if (_container.childCount != 0)
                TransformUtils.DestroyAllChildren(_container);

            foreach (AddonDownloadInfo download in result.List.Addons)
            {
                ModdedObject moddedObject = Instantiate(_addonDisplay, _container);
                moddedObject.gameObject.SetActive(true);
                UIElementAddonEditorDownloadDisplay addonEditorDownloadDisplay = moddedObject.gameObject.AddComponent<UIElementAddonEditorDownloadDisplay>();
                addonEditorDownloadDisplay.Initialize(download, result.List);
                _instantiatedEntries.Add(addonEditorDownloadDisplay);
            }
        }

        public void RemoveEntry(UIElementAddonEditorDownloadDisplay downloadDisplay)
        {
            _instantiatedEntries.Remove(downloadDisplay);
        }

        public void OnSaveButtonClicked()
        {
            foreach (UIElementAddonEditorDownloadDisplay entry in _instantiatedEntries)
            {
                entry.UpdateAddonDownloadInfo();
            }
            AddonManager.Instance.SaveDownloadListToDisk();
        }

        public void OnNewAddonButtonClicked()
        {
            _newAddonButton.interactable = false;
            DelegateScheduler.Instance.Schedule(delegate
            {
                if (_newAddonButton)
                    _newAddonButton.interactable = true;
            }, 1f);

            AddonDownloadInfo addonDownloadInfo = new AddonDownloadInfo();
            AddonDownloadListInfo downloads = AddonManager.Instance.GetCachedDownloadList();
            downloads.Addons.Add(addonDownloadInfo);
            ModdedObject moddedObject = Instantiate(_addonDisplay, _container);
            moddedObject.gameObject.SetActive(true);
            UIElementAddonEditorDownloadDisplay addonEditorDownloadDisplay = moddedObject.gameObject.AddComponent<UIElementAddonEditorDownloadDisplay>();
            addonEditorDownloadDisplay.Initialize(addonDownloadInfo, downloads);
            _instantiatedEntries.Add(addonEditorDownloadDisplay);
        }

        public void OnGetInfoFileClicked()
        {
            _getInfoFileButton.interactable = false;
            AddonManager.Instance.GetDownloadList(onGotDownloadList);
        }

        public void OnSavesFolderButtonClicked()
        {
            ModFileUtils.OpenFileExplorer(ModCore.DeveloperFolder);
        }
    }
}
