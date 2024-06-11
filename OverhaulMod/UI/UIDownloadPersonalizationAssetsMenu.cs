using OverhaulMod.Content.Personalization;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIDownloadPersonalizationAssetsMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnDownloadButtonClicked))]
        [UIElement("DownloadButton")]
        private readonly Button m_downloadButton;

        [UIElementAction(nameof(OnUpdateButtonClicked))]
        [UIElement("UpdateButton")]
        private readonly Button m_updateButton;

        [UIElement("ProgressBar", false)]
        private readonly GameObject m_progressBar;

        [UIElement("Fill")]
        private readonly Image m_progressBarFill;

        [UIElement("VersionText")]
        private readonly Text m_versionText;

        [UIElement("Header")]
        private readonly Text m_header;

        private PersonalizationManager m_personalizationManager;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            m_personalizationManager = PersonalizationManager.Instance;
        }

        public override void Show()
        {
            base.Show();
            refreshContents();

            PersonalizationManager personalizationManager = m_personalizationManager;
            if (personalizationManager.GetPersonalizationAssetsState() == PersonalizationAssetsState.NotInstalled)
            {
                m_header.text = "Download player\r\ncustomization assets";
            }
            else
            {
                m_header.text = "Update player\r\ncustomization assets";
            }
        }

        public override void Update()
        {
            base.Update();
            refreshProgressBarFill();
        }

        public bool CanExit()
        {
            return m_exitButton.gameObject.activeSelf;
        }

        private void refreshContents()
        {
            PersonalizationManager personalizationManager = m_personalizationManager;

            m_updateButton.interactable = true;
            m_progressBar.SetActive(personalizationManager.IsDownloadingCustomizationFile());
            switch (personalizationManager.GetPersonalizationAssetsState())
            {
                case PersonalizationAssetsState.NotInstalled:
                    m_downloadButton.gameObject.SetActive(!personalizationManager.IsDownloadingCustomizationFile());
                    m_updateButton.gameObject.SetActive(false);
                    break;
                case PersonalizationAssetsState.Installed:
                    m_downloadButton.gameObject.SetActive(false);
                    m_updateButton.gameObject.SetActive(!personalizationManager.IsDownloadingCustomizationFile());
                    m_updateButton.interactable = false;
                    break;
                case PersonalizationAssetsState.NeedUpdate:
                    m_downloadButton.gameObject.SetActive(false);
                    m_updateButton.gameObject.SetActive(!personalizationManager.IsDownloadingCustomizationFile());
                    break;
            }

            PersonalizationAssetsInfo personalizationAssetsInfo = personalizationManager.localAssetsInfo;
            if (personalizationAssetsInfo == null || personalizationAssetsInfo.AssetsVersion == null)
            {
                m_versionText.text = "None";
            }
            else
            {
                m_versionText.text = personalizationAssetsInfo.AssetsVersion.ToString();
            }
        }

        private void refreshProgressBarFill()
        {
            PersonalizationManager personalizationManager = m_personalizationManager;
            if (personalizationManager.IsDownloadingCustomizationFile())
            {
                m_progressBarFill.fillAmount = Mathf.Lerp(m_progressBarFill.fillAmount, personalizationManager.GetCustomizationFileDownloadProgress(), Time.unscaledDeltaTime * 12.5f);
            }
        }

        public void OnDownloadButtonClicked()
        {
            m_progressBarFill.fillAmount = 0f;
            m_exitButton.gameObject.SetActive(false);
            m_personalizationManager.DownloadCustomizationFile(delegate
            {
                m_exitButton.gameObject.SetActive(true);
                refreshContents();
            });
            refreshContents();
        }

        public void OnUpdateButtonClicked()
        {
            m_progressBarFill.fillAmount = 0f;
            m_exitButton.gameObject.SetActive(false);
            m_personalizationManager.DownloadCustomizationFile(delegate
            {
                m_exitButton.gameObject.SetActive(true);
                refreshContents();
            });
            refreshContents();
        }
    }
}
