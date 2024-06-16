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
            return ContentManager.Instance.HasContent(addonId, true);
        }

        public void RefreshDisplays()
        {
            if (addonId.IsNullOrEmpty())
                return;

            ContentManager contentManager = ContentManager.Instance;
            m_idleDisplays.SetActive(!contentManager.HasContent(addonId, true) && !contentManager.IsDownloading(addonId));
            m_loadingIndicatorObject.SetActive(contentManager.IsDownloading(addonId));
        }

        public void RefreshLoading()
        {
            if (addonId.IsNullOrEmpty())
                return;

            ContentManager contentManager = ContentManager.Instance;
            if (contentManager.IsDownloading(addonId))
            {
                m_loadingIndicatorText.text = $"Downloading...  {(Mathf.RoundToInt(Mathf.Clamp01(contentManager.GetDownloadProgress(addonId)) * 100f).ToString() + "%").AddColor(Color.white)}";
            }
        }

        public void OnDownloadButtonClicked()
        {
            if (addonId.IsNullOrEmpty())
                return;

            ModUIUtils.MessagePopup(true, $"Download {addonId}?", string.Empty, 100f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
            {
                ContentManager contentManager = ContentManager.Instance;
                _ = contentManager.DownloadContent(addonId, delegate
                {
                    onContentDownloaded.Invoke();
                    RefreshDisplays();
                }, delegate (string error)
                {
                    ModUIUtils.MessagePopupOK("Mod content download error", error);
                });
                RefreshDisplays();
                RefreshLoading();
            });
        }
    }
}
