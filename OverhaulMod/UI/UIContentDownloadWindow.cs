using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIContentDownloadWindow : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton", true)]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnDoneButtonClicked))]
        [UIElement("CloseButtonALT", false)]
        private readonly Button m_altExitButton;

        [UIElement("ProgressBar", false)]
        private readonly GameObject m_progressBar;

        [UIElement("Fill")]
        private readonly Image m_progressBarFill;

        [UIElementAction(nameof(OnDownloadButtonClicked))]
        [UIElement("DownloadButton", true)]
        private readonly Button m_downloadButton;

        private UnityWebRequest m_webRequest;

        public override void Show()
        {
            base.Show();
            SetTitleScreenButtonActive(false);
        }

        public override void Hide()
        {
            base.Hide();
            SetTitleScreenButtonActive(true);
        }

        public override void Update()
        {
            base.Update();

            if (m_webRequest != null)
            {
                try
                {
                    m_progressBarFill.fillAmount = m_webRequest.downloadProgress;
                }
                catch
                {

                }
            }
            else
                m_progressBarFill.fillAmount = 0f;
        }

        public void OnDownloadButtonClicked()
        {
            m_downloadButton.gameObject.SetActive(false);
            m_exitButton.gameObject.SetActive(false);
            m_progressBar.SetActive(true);
            m_progressBarFill.fillAmount = 0f;

            ContentManager.Instance.DownloadDefaultContent(out m_webRequest, onDownloadSuccess, onDownloadFail);
        }

        public void OnDoneButtonClicked()
        {
            SceneTransitionManager.Instance.DisconnectAndExitToMainMenu();
        }

        private void onDownloadFail(string error)
        {
            ModUIUtils.MessagePopupOK("Mod content download error", error);
            m_downloadButton.gameObject.SetActive(true);
            m_exitButton.gameObject.SetActive(true);
            m_progressBar.SetActive(false);
        }

        private void onDownloadSuccess()
        {
            ModUIUtils.MessagePopupOK("Mod content download complete", "Press that \"Done!\" button");
            m_altExitButton.gameObject.SetActive(true);
            m_downloadButton.gameObject.SetActive(false);
            m_exitButton.gameObject.SetActive(false);
            m_progressBar.SetActive(false);
            m_webRequest = null;
        }
    }
}
