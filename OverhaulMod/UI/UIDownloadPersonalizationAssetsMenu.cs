using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIDownloadPersonalizationAssetsMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnDownloadButtonClicked))]
        [UIElement("DownloadButton")]
        private readonly Button _downloadButton;

        [UIElementAction(nameof(OnUpdateButtonClicked))]
        [UIElement("UpdateButton")]
        private readonly Button _updateButton;

        [UIElementAction(nameof(OnRefreshButtonClicked))]
        [UIElement("RefreshButton")]
        private readonly Button _refreshButton;

        [UIElement("ProgressBar", false)]
        private readonly GameObject _progressBar;

        [UIElement("Fill")]
        private readonly Image _progressBarFill;

        [UIElement("VersionText")]
        private readonly Text _versionText;

        [UIElement("Header")]
        private readonly Text _header;

        private float _dontActuallyRefreshRemoteVersionUntilTime;

        public override void Show()
        {
            base.Show();
            refreshContents();

            PersonalizationManager personalizationManager = PersonalizationManager.Instance;
            if (personalizationManager.GetPersonalizationAssetsState() == PersonalizationAssetsState.NotInstalled)
            {
                _header.text = LocalizationManager.Instance.GetTranslatedString("customization_need_install_header");
            }
            else
            {
                _header.text = LocalizationManager.Instance.GetTranslatedString("customization_need_update_header");
            }
        }

        public override void Update()
        {
            base.Update();
            refreshProgressBarFill();
        }

        public bool CanExit()
        {
            return _exitButton.gameObject.activeSelf;
        }

        private void refreshContents()
        {
            PersonalizationManager personalizationManager = PersonalizationManager.Instance;

            _updateButton.interactable = true;
            _progressBar.SetActive(personalizationManager.IsDownloadingCustomizationFile());
            switch (personalizationManager.GetPersonalizationAssetsState())
            {
                case PersonalizationAssetsState.NotInstalled:
                    _downloadButton.gameObject.SetActive(!personalizationManager.IsDownloadingCustomizationFile());
                    _refreshButton.gameObject.SetActive(false);
                    _updateButton.gameObject.SetActive(false);
                    break;
                case PersonalizationAssetsState.Installed:
                    _downloadButton.gameObject.SetActive(false);
                    _refreshButton.gameObject.SetActive(!personalizationManager.IsDownloadingCustomizationFile());
                    _updateButton.gameObject.SetActive(!personalizationManager.IsDownloadingCustomizationFile());
                    _updateButton.interactable = false;
                    break;
                case PersonalizationAssetsState.NeedUpdate:
                    _downloadButton.gameObject.SetActive(false);
                    _refreshButton.gameObject.SetActive(false);
                    _updateButton.gameObject.SetActive(!personalizationManager.IsDownloadingCustomizationFile());
                    break;
            }

            PersonalizationAssetsInfo personalizationAssetsInfo = personalizationManager.LocalAssetsInfo;
            if (personalizationAssetsInfo == null || personalizationAssetsInfo.AssetVersionNumber == -1)
            {
                _versionText.text = "None";
            }
            else
            {
                _versionText.text = personalizationAssetsInfo.AssetVersionNumber.ToString();
            }
        }

        private void refreshProgressBarFill()
        {
            PersonalizationManager personalizationManager = PersonalizationManager.Instance;
            if (personalizationManager.IsDownloadingCustomizationFile())
            {
                _progressBarFill.fillAmount = Mathf.Lerp(_progressBarFill.fillAmount, personalizationManager.GetCustomizationFileDownloadProgress(), Time.unscaledDeltaTime * 12.5f);
            }
        }

        public void OnDownloadButtonClicked()
        {
            _progressBarFill.fillAmount = 0f;
            _exitButton.gameObject.SetActive(false);
            PersonalizationManager.Instance.DownloadCustomizationFile(delegate (string error)
            {
                _exitButton.gameObject.SetActive(true);
                refreshContents();

                if (!error.IsNullOrEmpty())
                {
                    ModUIUtils.MessagePopupOK("Error", $"Something went wrong while processing customization assets:\n{error}", true);
                }
            });
            refreshContents();
        }

        public void OnUpdateButtonClicked()
        {
            _progressBarFill.fillAmount = 0f;
            _exitButton.gameObject.SetActive(false);
            PersonalizationManager.Instance.DownloadCustomizationFile(delegate (string error)
            {
                _exitButton.gameObject.SetActive(true);
                refreshContents();

                if (!error.IsNullOrEmpty())
                {
                    ModUIUtils.MessagePopupOK("Error", $"Something went wrong while processing customization assets:\n{error}", true);
                }
            });
            refreshContents();
        }

        public void OnRefreshButtonClicked()
        {
            _refreshButton.interactable = false;
            if (Time.realtimeSinceStartup < _dontActuallyRefreshRemoteVersionUntilTime)
            {
                DelegateScheduler.Instance.Schedule(delegate
                {
                    _refreshButton.interactable = true;
                }, 1f);
                return;
            }

            PersonalizationManager.Instance.RefreshRemoteCustomizationAssetsVersion(delegate (bool result)
            {
                _dontActuallyRefreshRemoteVersionUntilTime = Time.realtimeSinceStartup + 15f;
                _refreshButton.interactable = true;
                if (result)
                    refreshContents();
            });
        }
    }
}
