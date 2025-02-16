using OverhaulMod.Content;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementLocalAddonDisplay : OverhaulUIBehaviour
    {
        [UIElement("ContentName")]
        private readonly Text m_addonNameText;

        [UIElement("ContentDescription")]
        private readonly Text m_addonDescriptionText;

        [UIElementAction(nameof(OnDeleteButtonClicked))]
        [UIElement("DeleteButton")]
        private readonly Button m_deleteButton;

        [UIElementAction(nameof(OnUpdateButtonClicked))]
        [UIElement("UpdateButton", false)]
        private readonly Button m_updateButton;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject m_loadingIndicatorObject;

        [UIElement("LoadingIndicatorText")]
        private readonly Text m_loadingIndicatorText;

        private AddonInfo m_addonInfo;

        private UIAddonsMenu m_addonMenu;

        private bool m_hasAddedEventListeners;

        public void Initialize(AddonInfo addonInfo, UIAddonsMenu addonsMenu)
        {
            base.InitializeElement();

            m_addonInfo = addonInfo;
            m_addonMenu = addonsMenu;

            m_addonNameText.text = addonInfo.GetDisplayName();
            m_addonDescriptionText.text = addonInfo.GetDescription();

            refreshUpdateElements();

            GlobalEventManager.Instance.AddEventListener(AddonManager.ADDON_UPDATES_REFRESHED, refreshUpdateElements);
            m_hasAddedEventListeners = true;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (m_hasAddedEventListeners)
            {
                GlobalEventManager.Instance.RemoveEventListener(AddonManager.ADDON_UPDATES_REFRESHED, refreshUpdateElements);
                m_hasAddedEventListeners = false;
            }
        }

        public override void Update()
        {
            bool isDownloading = AddonManager.Instance.IsDownloadingAddon(m_addonInfo.UniqueID);
            if (isDownloading)
            {
                m_loadingIndicatorText.text = $"{LocalizationManager.Instance.GetTranslatedString("updating...")}  {(Mathf.RoundToInt(Mathf.Clamp01(AddonManager.Instance.GetAddonDownloadProgress(m_addonInfo.UniqueID)) * 100f).ToString() + "%").AddColor(Color.white)}";
            }
        }

        private void refreshUpdateElements()
        {
            bool isDownloading = AddonManager.Instance.IsDownloadingAddon(m_addonInfo.UniqueID);
            bool hasUpdate = AddonManager.Instance.DoesAddonNeedUpdate(m_addonInfo);
            m_addonDescriptionText.gameObject.SetActive(!isDownloading && !hasUpdate);
            m_updateButton.gameObject.SetActive(hasUpdate && !isDownloading);
            m_loadingIndicatorObject.SetActive(isDownloading);
        }

        public void OnDeleteButtonClicked()
        {
            ModUIUtils.MessagePopup(true, $"{LocalizationManager.Instance.GetTranslatedString("addons_confirmdelete_header")} \"{m_addonInfo.GetDisplayName()}\"?", LocalizationManager.Instance.GetTranslatedString("action_cannot_be_undone"), 125f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
            {
                Directory.Delete(m_addonInfo.FolderPath, true);
                AddonManager.Instance.RefreshInstalledAddons();
                m_addonMenu.Populate();
            });
        }

        public void OnUpdateButtonClicked()
        {
            AddonManager.Instance.DownloadAddon(m_addonInfo.UniqueID, delegate (string error)
            {
                if (!string.IsNullOrEmpty(error))
                {
                    refreshUpdateElements();
                    ModUIUtils.MessagePopupOK("Error", error, 200f, true);
                    return;
                }
                m_addonMenu.Populate();
            });
            refreshUpdateElements();
        }
    }
}
