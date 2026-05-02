using OverhaulMod.Content;
using OverhaulMod.Utils;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementLocalAddonDisplay : OverhaulUIBehaviour
    {
        [UIElement("ContentName")]
        private readonly Text _addonNameText;

        [UIElement("ContentDescription")]
        private readonly Text _addonDescriptionText;

        [UIElementAction(nameof(OnDeleteButtonClicked))]
        [UIElement("DeleteButton")]
        private readonly Button _deleteButton;

        [UIElementAction(nameof(OnUpdateButtonClicked))]
        [UIElement("UpdateButton", false)]
        private readonly Button _updateButton;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject _loadingIndicatorObject;

        [UIElement("LoadingIndicatorText")]
        private readonly Text _loadingIndicatorText;

        private AddonInfo _addonInfo;

        private UIAddonsMenu _addonMenu;

        private bool _hasAddedEventListeners;

        public void Initialize(AddonInfo addonInfo, UIAddonsMenu addonsMenu)
        {
            base.InitializeAsElement();

            _addonInfo = addonInfo;
            _addonMenu = addonsMenu;

            _addonNameText.text = addonInfo.GetDisplayName();
            _addonDescriptionText.text = addonInfo.GetDescription();

            refreshUpdateElements();

            GlobalEventManager.Instance.AddEventListener(AddonManager.ADDON_UPDATES_REFRESHED_EVENT, refreshUpdateElements);
            _hasAddedEventListeners = true;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (_hasAddedEventListeners)
            {
                GlobalEventManager.Instance.RemoveEventListener(AddonManager.ADDON_UPDATES_REFRESHED_EVENT, refreshUpdateElements);
                _hasAddedEventListeners = false;
            }
        }

        public override void Update()
        {
            bool isDownloading = AddonManager.Instance.IsDownloadingAddon(_addonInfo.UniqueID);
            if (isDownloading)
            {
                _loadingIndicatorText.text = $"{LocalizationManager.Instance.GetTranslatedString("updating...")}  {(Mathf.RoundToInt(Mathf.Clamp01(AddonManager.Instance.GetAddonDownloadProgress(_addonInfo.UniqueID)) * 100f).ToString() + "%").AddColor(Color.white)}";
            }
        }

        private void refreshUpdateElements()
        {
            bool isDownloading = AddonManager.Instance.IsDownloadingAddon(_addonInfo.UniqueID);
            bool hasUpdate = AddonManager.Instance.DoesAddonNeedUpdate(_addonInfo);
            _addonDescriptionText.gameObject.SetActive(!isDownloading && !hasUpdate);
            _updateButton.gameObject.SetActive(hasUpdate && !isDownloading);
            _loadingIndicatorObject.SetActive(isDownloading);
        }

        public void OnDeleteButtonClicked()
        {
            ModUIUtils.MessagePopup(true, $"{LocalizationManager.Instance.GetTranslatedString("addons_confirmdelete_header")} \"{_addonInfo.GetDisplayName()}\"?", LocalizationManager.Instance.GetTranslatedString("action_cannot_be_undone"), 125f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
            {
                Directory.Delete(_addonInfo.FolderPath, true);
                AddonManager.Instance.RefreshInstalledAddons();
                _addonMenu.Populate();
            });
        }

        public void OnUpdateButtonClicked()
        {
            AddonManager.Instance.DownloadAddon(_addonInfo.UniqueID, delegate (string error)
            {
                if (!string.IsNullOrEmpty(error))
                {
                    refreshUpdateElements();
                    ModUIUtils.MessagePopupOK("Error", error, 200f, true);
                    return;
                }
                _addonMenu.Populate();
            });
            refreshUpdateElements();
        }
    }
}
