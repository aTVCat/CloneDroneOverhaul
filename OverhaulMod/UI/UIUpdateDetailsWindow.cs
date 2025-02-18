using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIUpdateDetailsWindow : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnDownloadButtonClicked))]
        [UIElement("DownloadButton")]
        private readonly Button m_downloadButton;

        [UIElement("ProgressBar", false)]
        private readonly GameObject m_progressBar;

        [UIElement("Fill")]
        private readonly Image m_progressBarFill;

        [UIElement("ButtonsContainer", true)]
        private readonly GameObject m_buttonsContainer;

        public override bool hideTitleScreen => true;

        public override bool closeOnEscapeButtonPress => m_allowHidingThisMenu;

        private bool m_allowHidingThisMenu;

        private UpdateInfo m_updateInfo;

        private string m_branch;

        public void Populate(UpdateInfo updateInfo, string branch)
        {
            m_updateInfo = updateInfo;
            m_branch = branch;

            ModUIConstants.ShowPatchNotes(base.transform, new UIPatchNotes.ShowArguments
            {
                CloseButtonActive = false,
                PanelOffset = new UnityEngine.Vector2(0f, 65f),
                ShrinkPanel = true,
                HideVersionList = true,
                DisableShading = true,
            });
        }

        public override void Show()
        {
            base.Show();
            m_allowHidingThisMenu = true;
        }

        public override void Hide()
        {
            base.Hide();
            ModUIConstants.HidePatchNotes();
        }

        public override void Update()
        {
            m_progressBarFill.fillAmount = UpdateManager.Instance.GetBuildDownloadProgress();
        }

        public void OnDownloadButtonClicked()
        {
            m_allowHidingThisMenu = false;
            m_downloadButton.interactable = false;
            m_progressBar.SetActive(true);
            m_progressBarFill.fillAmount = 0f;
            m_buttonsContainer.SetActive(false);

            UpdateManager.Instance.DownloadBuild(m_updateInfo.DownloadLink, m_updateInfo.IsGoogleDriveLink, $"OverhaulMod_{m_updateInfo.ModVersion}_{m_branch}", delegate (UpdateManager.InstallUpdateResult installUpdateResult)
            {
                m_allowHidingThisMenu = true;
                m_progressBar.SetActive(false);
                m_buttonsContainer.SetActive(true);

                if (!installUpdateResult.IsError())
                {
                    _ = ModUIConstants.ShowRestartRequiredScreen(false);
                }
                else
                {
                    m_downloadButton.interactable = true;
                }
            });
        }
    }
}
