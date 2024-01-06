using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIUpdatesWindow : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnEditorButtonClicked))]
        [UIElement("EditorButton")]
        private readonly Button m_editorButton;

        [UIElementAction(nameof(OnCheckForUpdatesButtonClicked))]
        [UIElement("CheckForUpdatesButton")]
        private readonly Button m_checkForUpdatesButton;

        [UIElementAction(nameof(OnDirectUpdateButtonClicked))]
        [UIElement("UpdateButton")]
        private readonly Button m_directUpdateButton;

        [UIElement("BranchDropdown")]
        private readonly Dropdown m_branchDropdown;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject m_loadingIndicator;

        [UIElement("ChangelogText")]
        private readonly Text m_changelogText;

        [UIElement("ProgressBar", false)]
        private readonly GameObject m_progressBar;

        [UIElement("ProgressBarFill")]
        private readonly Image m_progressBarFill;

        private UnityWebRequest m_webRequest;
        private string m_downloadSource, m_directoryName;

        public bool disallowClosing
        {
            get;
            set;
        }

        protected override void OnInitialized()
        {
            m_directUpdateButton.interactable = false;
            m_changelogText.text = string.Empty;
            m_branchDropdown.value = 0;
        }

        public override void Show()
        {
            base.Show();
            SetTitleScreenButtonActive(false);
        }

        public override void Hide()
        {
            if (disallowClosing)
                return;

            base.Hide();
            SetTitleScreenButtonActive(true);
        }

        public override void Update()
        {
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

        public void SetUIInteractable(bool value)
        {
            m_branchDropdown.interactable = value;
            m_checkForUpdatesButton.interactable = value;
            m_loadingIndicator.SetActive(!value);
        }

        private void onCheckedUpdates(UpdateInfoList updateInfoList)
        {
            SetUIInteractable(true);
            UpdateInfo updateInfo = null;
            switch (m_branchDropdown.value)
            {
                case 0:
                    updateInfo = updateInfoList.ModBotRelease;
                    break;
                case 1:
                    updateInfo = updateInfoList.GitHubRelease;
                    break;
                case 2:
                    updateInfo = updateInfoList.InternalRelease;
                    break;
            }

            if (updateInfo == null || updateInfo.IsCurrentBuild() || updateInfo.IsOldBuild())
            {
                m_changelogText.text = "No updates found.";
                return;
            }
            m_directoryName = "OverhaulMod_V" + updateInfo.ModVersion;
            m_downloadSource = updateInfo.DownloadLink;
            m_changelogText.text = $"Update available: {updateInfo.ModVersion} ({updateInfo.ModBotVersion})\nChangelog:\n{updateInfo.Changelog}";
            m_directUpdateButton.interactable = true;
        }

        private void onFailedToCheckUpdates(string error)
        {
            m_changelogText.text = $"Error.\n{error}".AddColor(Color.red);
            SetUIInteractable(true);
        }

        private void onInstalledNewBuild()
        {
            m_progressBar.SetActive(false);
            ModUIConstants.ShowRestartRequiredScreen(false);
        }

        private void onFailedToInstallNewBuild(string error)
        {
            disallowClosing = false;
            m_exitButton.interactable = true;
            m_progressBar.SetActive(false);
            SetUIInteractable(true);

            ModUIUtility.MessagePopupOK("Installation error", error, 250f);
        }

        public void OnCheckForUpdatesButtonClicked()
        {
            m_directUpdateButton.interactable = false;
            m_changelogText.text = string.Empty;
            SetUIInteractable(false);

            UpdateManager.Instance.DownloadUpdateInfoFile(onCheckedUpdates, onFailedToCheckUpdates, true);
        }

        public void OnDirectUpdateButtonClicked()
        {
            disallowClosing = true;
            m_exitButton.interactable = false;
            m_changelogText.text = string.Empty;
            m_directUpdateButton.interactable = false;
            m_progressBar.SetActive(true);
            SetUIInteractable(false);

            UpdateManager.Instance.DownloadBuildFromSource(m_downloadSource, m_directoryName, onInstalledNewBuild, onFailedToInstallNewBuild, out m_webRequest);
        }

        public void OnEditorButtonClicked()
        {
            ModUIConstants.ShowUpdateInfoEditor(base.transform);
        }
    }
}
