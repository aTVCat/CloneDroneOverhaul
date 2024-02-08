using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAddonsMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnLocalAddonsEditorButtonClicked))]
        [UIElement("EditorButton")]
        private readonly Button m_localAddonsEditorButton;

        [UIElementAction(nameof(OnNetworkAddonsEditorButtonClicked))]
        [UIElement("NetworkEditorButton")]
        private readonly Button m_networkAddonsEditorButton;

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

        private bool m_shouldSuggestGameRestart;

        public override bool hideTitleScreen => true;

        protected override void OnInitialized()
        {
            m_tabs.AddTab(m_localAddonsTab, "local addons");
            m_tabs.AddTab(m_networkAddonsTab, "network addons");
            m_tabs.SelectTab("local addons");

            GlobalEventManager.Instance.AddEventListener<string>(ContentManager.CONTENT_DOWNLOAD_DONE_EVENT, onContentDownloaded);
        }

        public override void Hide()
        {
            base.Hide();
            if (m_shouldSuggestGameRestart)
            {
                ModUIConstants.ShowRestartRequiredScreen(true);
                m_shouldSuggestGameRestart = false;
            }
        }

        private void onContentDownloaded(string error)
        {
            if (!string.IsNullOrEmpty(error))
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

            System.Collections.Generic.List<ContentInfo> list = ContentManager.Instance.GetContent();
            if (list.IsNullOrEmpty())
                return;

            foreach (ContentInfo content in list)
            {
                ModdedObject moddedObject = Instantiate(m_localContentDisplay, m_container);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = content.DisplayName;
            }
        }

        private void populateNetworkContent()
        {
            if (m_container.childCount != 0)
                TransformUtils.DestroyAllChildren(m_container);

            m_tabs.interactable = false;
            ContentManager.Instance.DownloadContentList(out _, delegate (ContentListInfo contentListInfo)
            {
                foreach (ContentDownloadInfo content in contentListInfo.ContentToDownload)
                {
                    if (content == null || content.File.IsNullOrEmpty() || content.DisplayName.IsNullOrEmpty())
                        continue;

                    ModdedObject moddedObject = Instantiate(m_networkContentDisplay, m_container);
                    moddedObject.gameObject.SetActive(true);
                    moddedObject.GetObject<Text>(0).text = content.DisplayName;
                    moddedObject.GetObject<Text>(1).text = (Mathf.RoundToInt(Mathf.Round(content.Size / 1048576f * 10f)) / 10f).ToString() + " Megabytes";

                    UIElementNetworkAddonDisplay networkAddonDisplay = moddedObject.gameObject.AddComponent<UIElementNetworkAddonDisplay>();
                    networkAddonDisplay.contentFile = content.File;
                    networkAddonDisplay.InitializeElement();
                }

                m_tabs.interactable = true;
            }, delegate (string error)
            {
                ModUIUtils.MessagePopupOK("Error", error, true);

                m_tabs.interactable = true;
                m_tabs.SelectTab("local addons");
            });
        }

        public void OnLocalAddonsEditorButtonClicked()
        {
            ModUIConstants.ShowAddonsEditor(base.transform);
        }

        public void OnNetworkAddonsEditorButtonClicked()
        {
            ModUIConstants.ShowAddonsDownloadEditor(base.transform);
        }
    }
}
