using OverhaulMod.Content;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAddonsMenu : OverhaulUIBehaviour
    {
        private static AddonDownloadListInfo s_contentLiftInfo;

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnAddonsEditorButtonClicked))]
        [UIElement("EditorButton")]
        private readonly Button m_addonsEditorButton;

        [UIElementAction(nameof(OnAddonsDownloadEditorButtonClicked))]
        [UIElement("AddonDownloadsEditorButton")]
        private readonly Button m_addonsDownloadEditorButton;

        [UIElement("LocalAddons")]
        private readonly ModdedObject m_localAddonsTab;
        [UIElement("NetworkAddons")]
        private readonly ModdedObject m_networkAddonsTab;

        [TabManager(typeof(UIElementTab), null, null, null, nameof(OnTabSelected))]
        private readonly TabManager m_tabs;

        [UIElement("LocalContentDisplay", false)]
        private readonly ModdedObject m_localContentDisplay;
        [UIElement("NetworkContentDisplay", false)]
        private readonly ModdedObject m_networkContentDisplay;

        [UIElement("Content")]
        private readonly Transform m_container;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject m_loadingIndicator;

        private bool m_shouldSuggestGameRestart;

        public override bool hideTitleScreen => true;

        protected override void OnInitialized()
        {
            m_tabs.AddTab(m_localAddonsTab.gameObject, "local addons");
            m_tabs.AddTab(m_networkAddonsTab.gameObject, "network addons");
            m_tabs.SelectTab("local addons");

            GlobalEventManager.Instance.AddEventListener<string>(AddonManager.ADDON_DOWNLOADED_EVENT, onContentDownloaded);

            if (TitleScreenCustomizationManager.IntroduceCustomization)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.INTRODUCE_TITLE_SCREEN_CUSTOMIZATION, false);
            }
        }

        public override void Show()
        {
            base.Show();

            m_addonsEditorButton.gameObject.SetActive(ModUserInfo.isDeveloper);
            m_addonsDownloadEditorButton.gameObject.SetActive(ModUserInfo.isDeveloper);
        }

        public override void Hide()
        {
            base.Hide();
            if (m_shouldSuggestGameRestart)
            {
                _ = ModUIConstants.ShowRestartRequiredScreen(true);
                m_shouldSuggestGameRestart = false;
            }
        }

        private void onContentDownloaded(string error)
        {
            if (!error.IsNullOrEmpty())
            {
                ModUIUtils.MessagePopupOK("Content download error", "Details:\n" + error, 350f, true);
            }
            else
            {
                m_shouldSuggestGameRestart = true;
            }
        }

        public void OnTabSelected(UIElementTab elementTab)
        {
            bool local = elementTab.tabId == "local addons";

            UIElementTab oldTab = m_tabs.prevSelectedTab;
            UIElementTab newTab = m_tabs.selectedTab;
            if (oldTab)
            {
                RectTransform rt = oldTab.transform as RectTransform;
                Vector2 vector = rt.sizeDelta;
                vector.y = 25f;
                rt.sizeDelta = vector;
            }
            if (newTab)
            {
                RectTransform rt = newTab.transform as RectTransform;
                Vector2 vector = rt.sizeDelta;
                vector.y = 30f;
                rt.sizeDelta = vector;
            }

            if (local)
                populateLocalContent();
            else
                populateNetworkContent();
        }

        private void populateLocalContent()
        {
            if (m_container.childCount != 0)
                TransformUtils.DestroyAllChildren(m_container);

            System.Collections.Generic.List<AddonInfo> list = AddonManager.Instance.GetLoadedAddons();
            if (list.IsNullOrEmpty())
                return;

            foreach (AddonInfo content in list)
            {
                string displayName = content.GetDisplayName();

                ModdedObject moddedObject = Instantiate(m_localContentDisplay, m_container);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = displayName;
                moddedObject.GetObject<Text>(1).text = content.GetDescription();
                moddedObject.GetObject<Button>(2).onClick.AddListener(delegate
                {
                    ModUIUtils.MessagePopup(true, $"{LocalizationManager.Instance.GetTranslatedString("addons_confirmdelete_header")} \"{displayName}\"?", LocalizationManager.Instance.GetTranslatedString("action_cannot_be_undone"), 125f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
                    {
                        if (moddedObject && moddedObject.gameObject)
                        {
                            Directory.Delete(content.FolderPath, true);
                            AddonManager.Instance.RefreshInstalledAddons();
                            populateLocalContent();
                        }
                    });
                });
            }
        }

        private void populateNetworkContent()
        {
            if (m_container.childCount != 0)
                TransformUtils.DestroyAllChildren(m_container);

            if (s_contentLiftInfo != null)
            {
                populate(s_contentLiftInfo);
                return;
            }

            m_loadingIndicator.SetActive(true);
            m_tabs.interactable = false;
            AddonManager.Instance.DownloadAddonsList(out _, populate, delegate (string error)
            {
                ModUIUtils.MessagePopupOK("Error", error, true);

                m_loadingIndicator.SetActive(false);
                m_tabs.interactable = true;
                m_tabs.SelectTab("local addons");
            });
        }

        private void populate(AddonDownloadListInfo contentListInfo)
        {
            s_contentLiftInfo = contentListInfo;
            foreach (AddonDownloadInfo addonDownloadInfo in contentListInfo.Addons)
            {
                ModdedObject moddedObject = Instantiate(m_networkContentDisplay, m_container);
                moddedObject.gameObject.SetActive(true);
                UIElementNetworkAddonDisplay networkAddonDisplay = moddedObject.gameObject.AddComponent<UIElementNetworkAddonDisplay>();
                networkAddonDisplay.Initialize(addonDownloadInfo, base.transform);
            }

            m_loadingIndicator.SetActive(false);
            m_tabs.interactable = true;
        }

        public void OnAddonsEditorButtonClicked()
        {
            _ = ModUIConstants.ShowAddonsEditor(base.transform);
        }

        public void OnAddonsDownloadEditorButtonClicked()
        {
            _ = ModUIConstants.ShowAddonsDownloadEditor(base.transform);
        }
    }
}
