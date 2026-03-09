using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAddonsMenu : OverhaulUIBehaviour
    {
        private static AddonDownloadListInfo s_contentLiftInfo;

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnAddonsEditorButtonClicked))]
        [UIElement("EditorButton")]
        private readonly Button _addonsEditorButton;

        [UIElementAction(nameof(OnAddonsDownloadEditorButtonClicked))]
        [UIElement("AddonDownloadsEditorButton")]
        private readonly Button _addonsDownloadEditorButton;

        [UIElement("LocalAddons")]
        private readonly ModdedObject _localAddonsTab;
        [UIElement("NetworkAddons")]
        private readonly ModdedObject _networkAddonsTab;

        [TabManager(typeof(UIElementTab), null, null, null, nameof(OnTabSelected))]
        private readonly TabManager _tabs;

        [UIElement("LocalContentDisplay", false)]
        private readonly ModdedObject _localContentDisplay;
        [UIElement("NetworkContentDisplay", false)]
        private readonly ModdedObject _networkContentDisplay;

        [UIElement("Content")]
        private readonly Transform _container;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject _loadingIndicator;

        private bool _shouldSuggestGameRestart;

        public override bool HideTitleScreen => true;

        protected override void OnInitialized()
        {
            _tabs.AddTab(_localAddonsTab.gameObject, "local addons");
            _tabs.AddTab(_networkAddonsTab.gameObject, "network addons");
            _tabs.SelectTab("local addons");

            GlobalEventManager.Instance.AddEventListener<string>(AddonManager.ADDON_DOWNLOADED_EVENT, onContentDownloaded);
        }

        public override void Show()
        {
            base.Show();

            _addonsEditorButton.gameObject.SetActive(ModUserInfo.isDeveloper);
            _addonsDownloadEditorButton.gameObject.SetActive(ModUserInfo.isDeveloper);
        }

        public override void Hide()
        {
            base.Hide();
            if (_shouldSuggestGameRestart)
            {
                _ = ModUIConstants.ShowRestartRequiredScreen(true);
                _shouldSuggestGameRestart = false;
            }
        }

        private void onContentDownloaded(string error)
        {
            if (error.IsNullOrEmpty())
            {
                _shouldSuggestGameRestart = true;
            }
        }

        public void OnTabSelected(UIElementTab elementTab)
        {
            bool local = elementTab.tabId == "local addons";

            UIElementTab oldTab = _tabs.PreviousSelectedTab;
            UIElementTab newTab = _tabs.SelectedTab;
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

        public void Populate()
        {
            populateLocalContent();
        }

        private void populateLocalContent()
        {
            if (_container.childCount != 0)
                TransformUtils.DestroyAllChildren(_container);

            System.Collections.Generic.List<AddonInfo> list = AddonManager.Instance.GetLoadedAddons();
            if (list.IsNullOrEmpty())
                return;

            foreach (AddonInfo addon in list)
            {
                ModdedObject moddedObject = Instantiate(_localContentDisplay, _container);
                moddedObject.gameObject.SetActive(true);

                UIElementLocalAddonDisplay localAddonDisplay = moddedObject.gameObject.AddComponent<UIElementLocalAddonDisplay>();
                localAddonDisplay.Initialize(addon, this);
            }
        }

        private void populateNetworkContent()
        {
            if (_container.childCount != 0)
                TransformUtils.DestroyAllChildren(_container);

            if (s_contentLiftInfo != null)
            {
                populate(s_contentLiftInfo);
                return;
            }

            _loadingIndicator.SetActive(true);
            _tabs.IsInteractable = false;
            AddonManager.Instance.DownloadAddonsList(out _, populate, delegate (string error)
            {
                ModUIUtils.MessagePopupOK("Error", error, true);

                _loadingIndicator.SetActive(false);
                _tabs.IsInteractable = true;
                _tabs.SelectTab("local addons");
            });
        }

        private void populate(AddonDownloadListInfo contentListInfo)
        {
            s_contentLiftInfo = contentListInfo;
            foreach (AddonDownloadInfo addonDownloadInfo in contentListInfo.Addons)
            {
                ModdedObject moddedObject = Instantiate(_networkContentDisplay, _container);
                moddedObject.gameObject.SetActive(true);
                UIElementNetworkAddonDisplay networkAddonDisplay = moddedObject.gameObject.AddComponent<UIElementNetworkAddonDisplay>();
                networkAddonDisplay.Initialize(addonDownloadInfo, base.transform);
            }

            _loadingIndicator.SetActive(false);
            _tabs.IsInteractable = true;
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
