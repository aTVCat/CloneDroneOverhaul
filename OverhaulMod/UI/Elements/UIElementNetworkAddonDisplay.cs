using OverhaulMod.Content;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;
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

        [UIElementAction(nameof(OnImagesButtonClicked))]
        [UIElement("ImagesButton")]
        private readonly Button m_imagesButton;

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

        public bool isLarge
        {
            get;
            set;
        }

        public Version minModVersion
        {
            get;
            set;
        }

        public List<string> images
        {
            get;
            set;
        }

        public Transform imageExplorerParentTransform
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
            m_minVersionText.text = $"New mod version is required: {versionString}";

            m_imagesButton.interactable = !images.IsNullOrEmpty();
        }

        public override void Update()
        {
            ContentManager contentManager = m_contentManager;
            string file = contentFile;

            bool isDownloadingContent = contentManager.IsDownloading(file);
            bool hasDownloaded = contentManager.HasContent(file, true);

            m_progressBar.SetActive(isDownloadingContent);
            m_contentSizeText.SetActive(!isDownloadingContent && isSupported);
            m_downloadButton.gameObject.SetActive(!isDownloadingContent && isSupported);
            m_downloadButton.interactable = !hasDownloaded;
            m_imagesButton.gameObject.SetActive(!isDownloadingContent && isSupported);

            if (isDownloadingContent)
                m_progressBarFill.fillAmount = contentManager.GetDownloadProgress(file);
        }

        public void OnDownloadButtonClicked()
        {
            if (isLarge)
            {
                ModUIUtils.MessagePopup(true, $"This add-on is large", "The download can take a while. Continue?", 125f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
                {
                    _ = m_contentManager.DownloadContent(contentFile, null, null);
                });
                return;
            }
            _ = m_contentManager.DownloadContent(contentFile, null, null);
        }

        public void OnImagesButtonClicked()
        {
            ModUIUtils.ImageExplorer(images, imageExplorerParentTransform);
        }
    }
}
