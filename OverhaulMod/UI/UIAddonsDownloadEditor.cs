using OverhaulMod.Content;
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

        [UIElement("NeedsSaveIcon", false)]
        private readonly GameObject _needsSaveIcon;

        private List<UIElementAddonEditorDownloadDisplay> _instantiatedEntries;

        protected override void OnInitialized()
        {
            _instantiatedEntries = new List<UIElementAddonEditorDownloadDisplay>();
        }

        public override void Show()
        {
            base.Show();
            populate();
        }

        private void populate()
        {
            _instantiatedEntries.Clear();
            if (_container.childCount != 0)
                TransformUtils.DestroyAllChildren(_container);

            AddonDownloadListInfo downloads = AddonManager.Instance.GetDownloadsFromDisk();
            foreach (AddonDownloadInfo download in downloads.Addons)
            {
                ModdedObject moddedObject = Instantiate(_addonDisplay, _container);
                moddedObject.gameObject.SetActive(true);
                UIElementAddonEditorDownloadDisplay addonEditorDownloadDisplay = moddedObject.gameObject.AddComponent<UIElementAddonEditorDownloadDisplay>();
                addonEditorDownloadDisplay.Initialize(download, downloads);
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
            AddonManager.Instance.SaveDownloadsToDisk();
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
            AddonDownloadListInfo downloads = AddonManager.Instance.GetDownloadsFromDisk();
            downloads.Addons.Add(addonDownloadInfo);
            ModdedObject moddedObject = Instantiate(_addonDisplay, _container);
            moddedObject.gameObject.SetActive(true);
            UIElementAddonEditorDownloadDisplay addonEditorDownloadDisplay = moddedObject.gameObject.AddComponent<UIElementAddonEditorDownloadDisplay>();
            addonEditorDownloadDisplay.Initialize(addonDownloadInfo, downloads);
            _instantiatedEntries.Add(addonEditorDownloadDisplay);
        }
    }
}
