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
        public GameObject m_idleDisplays;

        [UIElementAction(nameof(OnDownloadButtonClicked))]
        [UIElement("DownloadButton")]
        public Button m_downloadButton;

        [UIElement("LoadingIndicator", false)]
        public GameObject m_loadingIndicatorObject;

        [UIElement("LoadingIndicatorText")]
        public Text m_loadingIndicatorText;

        private float m_timeLeftToRefresh;

        public string addonId
        {
            get;
            set;
        }

        public string addonFile
        {
            get;
            set;
        }

        public UnityEvent onContentDownloaded { get; set; } = new UnityEvent();

        protected override void OnInitialized()
        {
            RefreshDisplays();
        }

        public override void OnEnable()
        {
            if (m_initialized)
                RefreshDisplays();
        }

        public override void Update()
        {
            float d = Time.unscaledDeltaTime;

            m_timeLeftToRefresh -= d;
            if (m_timeLeftToRefresh <= 0f)
            {
                RefreshLoading();
                m_timeLeftToRefresh = 0.1f;
            }
        }

        public bool ShouldBeHidden()
        {
            return AddonManager.Instance.HasInstalledAddon(addonId, true);
        }

        public void RefreshDisplays()
        {
            if (addonId.IsNullOrEmpty())
                return;

            AddonManager contentManager = AddonManager.Instance;
            m_idleDisplays.SetActive(!contentManager.HasInstalledAddon(addonId, true) && !contentManager.IsDownloadingAddon(addonId));
            m_loadingIndicatorObject.SetActive(contentManager.IsDownloadingAddon(addonId));
        }

        public void RefreshLoading()
        {
            if (addonId.IsNullOrEmpty())
                return;

            AddonManager contentManager = AddonManager.Instance;
            if (contentManager.IsDownloadingAddon(addonId))
            {
                m_loadingIndicatorText.text = $"{LocalizationManager.Instance.GetTranslatedString("downloading...")}  {(Mathf.RoundToInt(Mathf.Clamp01(contentManager.GetDownloadProgressOfAddon(addonId)) * 100f).ToString() + "%").AddColor(Color.white)}";
            }
        }

        public void OnDownloadButtonClicked()
        {
            if (addonId.IsNullOrEmpty())
                return;

            ModUIUtils.MessagePopup(true, $"Download {addonId}?", string.Empty, 100f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
            {
                AddonManager contentManager = AddonManager.Instance;
                _ = contentManager.DownloadAddon(addonId, addonFile, delegate (string error)
                {
                    if (error != null)
                    {
                        ModUIUtils.MessagePopupOK("Mod content download error", error);
                        return;
                    }

                    onContentDownloaded.Invoke();
                    RefreshDisplays();
                });
                RefreshDisplays();
                RefreshLoading();
            });
        }
    }
}
