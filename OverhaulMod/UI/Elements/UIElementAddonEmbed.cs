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

        public string AddonId;

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
            return AddonManager.Instance.HasInstalledAddon(AddonId);
        }

        public void RefreshDisplays()
        {
            if (AddonId.IsNullOrEmpty())
                return;

            AddonManager contentManager = AddonManager.Instance;
            m_idleDisplays.SetActive(!contentManager.HasInstalledAddon(AddonId) && !contentManager.IsDownloadingAddon(AddonId));
            m_loadingIndicatorObject.SetActive(contentManager.IsDownloadingAddon(AddonId));
        }

        public void RefreshLoading()
        {
            if (AddonId.IsNullOrEmpty())
                return;

            AddonManager contentManager = AddonManager.Instance;
            if (contentManager.IsDownloadingAddon(AddonId))
            {
                m_loadingIndicatorText.text = $"{LocalizationManager.Instance.GetTranslatedString("downloading...")}  {(Mathf.RoundToInt(Mathf.Clamp01(contentManager.GetAddonDownloadProgress(AddonId)) * 100f).ToString() + "%").AddColor(Color.white)}";
            }
        }

        public void OnDownloadButtonClicked()
        {
            if (AddonId.IsNullOrEmpty())
                return;

            ModUIUtils.MessagePopup(true, $"Download {AddonId}?", string.Empty, 100f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
            {
                AddonManager contentManager = AddonManager.Instance;
                contentManager.DownloadAddon(AddonId, delegate (string error)
                {
                    RefreshDisplays();
                    if (!error.IsNullOrEmpty())
                    {
                        ModUIUtils.MessagePopupOK("Mod content download error", error);
                        return;
                    }

                    onContentDownloaded.Invoke();
                });
                RefreshDisplays();
                RefreshLoading();
            });
        }
    }
}
