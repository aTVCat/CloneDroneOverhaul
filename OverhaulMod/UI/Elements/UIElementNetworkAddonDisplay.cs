using OverhaulMod.Content;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementNetworkAddonDisplay : OverhaulUIBehaviour
    {
        [UIElement("ContentSize")]
        private readonly GameObject m_contentSizeText;

        [UIElementAction(nameof(OnDownloadButtonClicked))]
        [UIElement("DownloadButton")]
        private readonly Button m_downloadButton;

        [UIElement("ProgressBar")]
        private readonly GameObject m_progressBar;

        [UIElement("NotSupportedText", false)]
        private readonly GameObject m_notSupportedText;

        [UIElement("MinVersionText")]
        private readonly Text m_minVersionText;

        [UIElement("Fill")]
        private readonly Image m_progressBarFill;

        private ContentManager m_contentManager;

        public string contentFile
        {
            get;
            set;
        }

        public bool isSupported
        {
            get;
            set;
        }

        public Version minModVersion
        {
            get;
            set;
        }

        protected override void OnInitialized()
        {
            m_contentManager = ContentManager.Instance;

            string versionString = "N/A";
            if (minModVersion != null)
                versionString = minModVersion.ToString();

            m_minVersionText.enabled = !isSupported;
            m_minVersionText.text = $"Newer version required: {versionString}";
        }

        public override void Update()
        {
            ContentManager contentManager = m_contentManager;
            string file = contentFile;

            bool isDownloadingContent = contentManager.IsDownloading(file);
            bool hasDownloaded = contentManager.HasContent(file, true);

            m_progressBar.SetActive(isDownloadingContent);
            m_contentSizeText.SetActive(!isDownloadingContent);
            m_downloadButton.gameObject.SetActive(!isDownloadingContent && isSupported);
            m_downloadButton.interactable = !hasDownloaded;

            if (isDownloadingContent)
                m_progressBarFill.fillAmount = contentManager.GetDownloadProgress(file);
        }

        public void OnDownloadButtonClicked()
        {
            _ = m_contentManager.DownloadContent(contentFile, null, null);
        }
    }
}
