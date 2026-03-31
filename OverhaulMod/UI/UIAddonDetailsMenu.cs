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
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnDownloadButtonClicked))]
        [UIElement("DownloadButton")]
        private readonly Button _downloadButton;

        [UIElementAction(nameof(OnDownloadButtonClicked))]
        [UIElement("UpdateButton")]
        private readonly Button _updateButton;

        [UIElement("InstallButtons")]
        private readonly GameObject _installButtons;

        [UIElement("Header")]
        private readonly Text _header;

        [UIElement("ContentDescription")]
        private readonly Text _addonDescription;

        [UIElement("ContentSize")]
        private readonly Text _addonSize;

        [UIElement("ContentVersion")]
        private readonly Text _addonVersion;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject _loadingIndicator;

        [UIElement("LoadingIndicatorText")]
        private readonly Text _loadingIndicatorText;

        [UIElement("ImageDisplayPrefab", false)]
        private readonly ModdedObject _imageDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform _imageDisplayContainer;

        [UIElement("NoPreviewsLabel")]
        private readonly GameObject _noPreviewsLabelObject;

        private AddonDownloadInfo _addonDownloadInfo;

        private bool _downloadedAddonViaThisMenu;

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
            if (_addonDownloadInfo == null)
                return;

            bool isDownloading = AddonManager.Instance.IsDownloadingAddon(_addonDownloadInfo.UniqueID);
            if (isDownloading)
            {
                _loadingIndicatorText.text = $"{LocalizationManager.Instance.GetTranslatedString("downloading...")}  {(Mathf.RoundToInt(Mathf.Clamp01(AddonManager.Instance.GetAddonDownloadProgress(_addonDownloadInfo.UniqueID)) * 100f).ToString() + "%").AddColor(Color.white)}";
            }
        }

        public void SetAddon(AddonDownloadInfo addonDownloadInfo)
        {
            _addonDownloadInfo = addonDownloadInfo;
            _header.text = addonDownloadInfo.GetDisplayName();
            _addonDescription.text = addonDownloadInfo.GetDescription();
            _addonSize.text = addonDownloadInfo.GetPackageSizeString();
            _addonVersion.text = $"{LocalizationManager.Instance.GetTranslatedString("version")} {addonDownloadInfo.Addon.Version}";
            refreshElements();
            refreshImages();
        }

        private void refreshImages()
        {
            if (_imageDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_imageDisplayContainer);

            if (_addonDownloadInfo.Images.IsNullOrEmpty())
            {
                _noPreviewsLabelObject.SetActive(true);
                return;
            }
            _noPreviewsLabelObject.SetActive(false);

            foreach (string p in _addonDownloadInfo.Images)
            {
                ModdedObject moddedObject = Instantiate(_imageDisplayPrefab, _imageDisplayContainer);
                moddedObject.gameObject.SetActive(true);
                UIElementImageDisplay imageDisplay = moddedObject.gameObject.AddComponent<UIElementImageDisplay>();
                imageDisplay.InitializeElement();
                imageDisplay.Populate(p, false);
                imageDisplay.imageViewerParentTransform = base.transform;
            }
        }

        private void refreshElements()
        {
            if (_addonDownloadInfo == null || _addonDownloadInfo.Addon == null)
                return;

            bool isSupported = !_addonDownloadInfo.Addon.IsSupported();
            bool isInstalled = AddonManager.Instance.HasInstalledAddon(_addonDownloadInfo.UniqueID, 0);
            bool isDownloading = AddonManager.Instance.IsDownloadingAddon(_addonDownloadInfo.UniqueID);
            bool isNewVersion = AddonManager.Instance.GetAddonVersion(_addonDownloadInfo.UniqueID) < _addonDownloadInfo.Addon.Version;

            _downloadButton.gameObject.SetActive(!isInstalled);
            _updateButton.gameObject.SetActive(isInstalled && isNewVersion);
            _installButtons.SetActive(isSupported && !isDownloading);
            _loadingIndicator.SetActive(isDownloading);
        }

        private void onDownloadedAddon(string error)
        {
            if (!_downloadedAddonViaThisMenu)
                return;

            _downloadedAddonViaThisMenu = false;
            refreshElements();

            if (!string.IsNullOrEmpty(error))
            {
                ModUIUtils.MessagePopupOK("Error", error, 200f, true);
                return;
            }
        }

        public void OnDownloadButtonClicked()
        {
            _downloadedAddonViaThisMenu = true;
            AddonManager.Instance.DownloadAddon(_addonDownloadInfo, null);
            refreshElements();
        }
    }
}
