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
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnDownloadButtonClicked))]
        [UIElement("DownloadButton")]
        private readonly Button _downloadButton;

        [UIElement("ProgressBar", false)]
        private readonly GameObject _progressBar;

        [UIElement("Fill")]
        private readonly Image _progressBarFill;

        [UIElement("ButtonsContainer", true)]
        private readonly GameObject _buttonsContainer;

        public override bool HideTitleScreen => true;

        public override bool CloseOnEscapeButtonPress => _allowHidingThisMenu;

        private bool _allowHidingThisMenu;

        private UpdateInfo _updateInfo;

        private string _branch;

        public void Populate(UpdateInfo updateInfo, string branch)
        {
            _updateInfo = updateInfo;
            _branch = branch;

            UIPatchNotes patchNotes = ModUIConstants.ShowPatchNotes(base.transform, new UIPatchNotes.ShowArguments
            {
                CloseButtonActive = false,
                PanelOffset = new UnityEngine.Vector2(0f, 65f),
                ShrinkPanel = true,
                HideVersionList = true,
                DisableShading = true,
            });
            patchNotes.PopulateText(updateInfo.DisplayVersion?.ToString(), updateInfo.Changelog);
        }

        public override void Show()
        {
            base.Show();
            _allowHidingThisMenu = true;
        }

        public override void Hide()
        {
            base.Hide();
            ModUIConstants.HidePatchNotes();
        }

        public override void Update()
        {
            _progressBarFill.fillAmount = UpdateManager.Instance.GetBuildDownloadProgress();
        }

        public void OnDownloadButtonClicked()
        {
            _allowHidingThisMenu = false;
            _downloadButton.interactable = false;
            _progressBar.SetActive(true);
            _progressBarFill.fillAmount = 0f;
            _buttonsContainer.SetActive(false);

            UpdateManager.Instance.DownloadBuild(_updateInfo.DownloadLink, _updateInfo.IsGoogleDriveLink, $"OverhaulMod_{_updateInfo.DisplayVersion}_{_branch}", delegate (UpdateManager.InstallUpdateResult installUpdateResult)
            {
                _allowHidingThisMenu = true;
                _progressBar.SetActive(false);
                _buttonsContainer.SetActive(true);

                if (!installUpdateResult.HasFailed())
                {
                    _ = ModUIConstants.ShowRestartRequiredScreen(false);
                }
                else
                {
                    _downloadButton.interactable = true;
                }
            });
        }
    }
}
