using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementAddonEmbed : OverhaulUIBehaviour
    {
        [UIElement("IdleDisplays", false)]
        public GameObject _idleDisplays;

        [UIElementAction(nameof(OnDownloadButtonClicked))]
        [UIElement("DownloadButton")]
        public Button _downloadButton;

        [UIElementAction(nameof(OnUpdateButtonClicked))]
        [UIElement("UpdateButton")]
        public Button _updateButton;

        [UIElement("LoadingIndicator", false)]
        public GameObject _loadingIndicatorObject;

        [UIElement("LoadingIndicatorText")]
        public Text _loadingIndicatorText;

        private float _timeLeftToRefresh;

        public string AddonId;

        public int Version;

        public UnityEvent onContentDownloaded { get; set; } = new UnityEvent();

        protected override void OnInitialized()
        {
            RefreshDisplays();
        }

        public override void OnEnable()
        {
            if (_initialized)
                RefreshDisplays();
        }

        public override void Update()
        {
            float d = Time.unscaledDeltaTime;

            _timeLeftToRefresh -= d;
            if (_timeLeftToRefresh <= 0f)
            {
                RefreshLoading();
                _timeLeftToRefresh = 0.1f;
            }
        }

        public bool ShouldBeHidden()
        {
            return AddonManager.Instance.HasInstalledAddon(AddonId);
        }

        public void RefreshDisplays()
        {
            if (AddonId.IsNullOrEmpty())
                return;

            AddonManager contentManager = AddonManager.Instance;
            bool hasInstalled = contentManager.HasInstalledAddon(AddonId);
            bool hasUpdates = hasInstalled && !contentManager.HasInstalledAddon(AddonId, Version);
            bool isDownloading = contentManager.IsDownloadingAddon(AddonId);

            _idleDisplays.SetActive((!hasInstalled || hasUpdates) && !isDownloading);
            _downloadButton.gameObject.SetActive(!hasInstalled);
            _updateButton.gameObject.SetActive(hasUpdates);
            _loadingIndicatorObject.SetActive(isDownloading);
        }

        public void RefreshLoading()
        {
            if (AddonId.IsNullOrEmpty())
                return;

            AddonManager contentManager = AddonManager.Instance;
            if (contentManager.IsDownloadingAddon(AddonId))
            {
                _loadingIndicatorText.text = $"{LocalizationManager.Instance.GetTranslatedString("downloading...")}  {(Mathf.RoundToInt(Mathf.Clamp01(contentManager.GetAddonDownloadProgress(AddonId)) * 100f).ToString() + "%").AddColor(Color.white)}";
            }
        }

        private void installAddon(bool update)
        {
            if (AddonId.IsNullOrEmpty())
                return;

            ModUIUtils.MessagePopup(true, $"{(update ? "Update" : "Download")} this addon?", string.Empty, 100f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
            {
                AddonManager contentManager = AddonManager.Instance;
                contentManager.DownloadAddon(AddonId, delegate (string error)
                {
                    RefreshDisplays();
                    if (!error.IsNullOrEmpty())
                    {
                        ModUIUtils.MessagePopupOK($"Addon {(update ? "update" : "download")} error", error);
                        return;
                    }

                    onContentDownloaded.Invoke();
                });
                RefreshDisplays();
                RefreshLoading();
            });
        }

        public void OnDownloadButtonClicked()
        {
            installAddon(false);
        }

        public void OnUpdateButtonClicked()
        {
            installAddon(true);
        }
    }
}
