using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAddonDetailsMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnDownloadButtonClicked))]
        [UIElement("DownloadButton")]
        private readonly Button m_downloadButton;

        [UIElement("Header")]
        private readonly Text m_header;

        [UIElement("ContentDescription")]
        private readonly Text m_addonDescription;

        [UIElement("ContentSize")]
        private readonly Text m_addonSize;

        [UIElement("ContentVersion")]
        private readonly Text m_addonVersion;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject m_loadingIndicator;

        [UIElement("LoadingIndicatorText")]
        private readonly Text m_loadingIndicatorText;

        [UIElement("ImageDisplayPrefab", false)]
        private readonly ModdedObject m_imageDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform m_imageDisplayContainer;

        [UIElement("NoPreviewsLabel")]
        private readonly GameObject m_noPreviewsLabelObject;

        private AddonDownloadInfo m_addonDownloadInfo;

        private bool m_downloadedAddonViaThisMenu;

        protected override void OnInitialized()
        {
            GlobalEventManager.Instance.AddEventListener<string>(AddonManager.ADDON_DOWNLOADED_EVENT, onDownloadedAddon);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            GlobalEventManager.Instance.RemoveEventListener<string>(AddonManager.ADDON_DOWNLOADED_EVENT, onDownloadedAddon);
        }

        public override void Update()
        {
            if (m_addonDownloadInfo == null)
                return;

            bool isDownloading = AddonManager.Instance.IsDownloadingAddon(m_addonDownloadInfo.UniqueID);
            if (isDownloading)
            {
                m_loadingIndicatorText.text = $"{LocalizationManager.Instance.GetTranslatedString("downloading...")}  {(Mathf.RoundToInt(Mathf.Clamp01(AddonManager.Instance.GetAddonDownloadProgress(m_addonDownloadInfo.UniqueID)) * 100f).ToString() + "%").AddColor(Color.white)}";
            }
        }

        public void SetAddon(AddonDownloadInfo addonDownloadInfo)
        {
            m_addonDownloadInfo = addonDownloadInfo;
            m_header.text = addonDownloadInfo.GetDisplayName();
            m_addonDescription.text = addonDownloadInfo.GetDescription();
            m_addonSize.text = addonDownloadInfo.GetPackageSizeString();
            m_addonVersion.text = $"Version {addonDownloadInfo.Addon.Version}";
            refreshElements();
            refreshImages();
        }

        private void refreshImages()
        {
            if (m_imageDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(m_imageDisplayContainer);

            if (m_addonDownloadInfo.Images.IsNullOrEmpty())
            {
                m_noPreviewsLabelObject.SetActive(true);
                return;
            }
            m_noPreviewsLabelObject.SetActive(false);

            foreach (string p in m_addonDownloadInfo.Images)
            {
                ModdedObject moddedObject = Instantiate(m_imageDisplayPrefab, m_imageDisplayContainer);
                moddedObject.gameObject.SetActive(true);
                UIElementImageDisplay imageDisplay = moddedObject.gameObject.AddComponent<UIElementImageDisplay>();
                imageDisplay.InitializeElement();
                imageDisplay.Populate(p, false);
                imageDisplay.imageViewerParentTransform = base.transform;
            }
        }

        private void refreshElements()
        {
            if (m_addonDownloadInfo == null)
                return;

            bool isInstalled = AddonManager.Instance.HasInstalledAddon(m_addonDownloadInfo.UniqueID, 0);
            bool isDownloading = AddonManager.Instance.IsDownloadingAddon(m_addonDownloadInfo.UniqueID);
            m_downloadButton.gameObject.SetActive(!isInstalled && !isDownloading);
            m_loadingIndicator.SetActive(isDownloading);
        }

        private void onDownloadedAddon(string error)
        {
            if (!m_downloadedAddonViaThisMenu)
                return;

            m_downloadedAddonViaThisMenu = false;
            refreshElements();

            if (!string.IsNullOrEmpty(error))
            {
                ModUIUtils.MessagePopupOK("Error", error, 200f, true);
                return;
            }
        }

        public void OnDownloadButtonClicked()
        {
            m_downloadedAddonViaThisMenu = true;
            AddonManager.Instance.DownloadAddon(m_addonDownloadInfo, null);
            refreshElements();
        }
    }
}
