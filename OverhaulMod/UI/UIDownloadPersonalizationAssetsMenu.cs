using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIDownloadPersonalizationAssetsMenu : OverhaulUIBehaviour
    {
        public const float PANEL_HEIGHT_IDLE = 300f;

        public const float PANEL_HEIGHT_UPDATE = 455f;

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnInstallButtonClicked))]
        [UIElement("InstallButton")]
        private readonly Button _installButton;

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

        [UIElement("Header")]
        private readonly Text _header;

        [UIElement("MissingVersionHolder", false)]
        private readonly GameObject _missingVersionHolder;

        [BetterOutline]
        [UIElement("MissingVersionText")]
        private readonly Text _missingVersionText;

        [UIElement("LocalVersionHolder", false)]
        private readonly GameObject _localVersionHolder;

        [BetterOutline]
        [UIElement("LocalVersionText")]
        private readonly Text _localVersionText;

        [UIElement("RemoteVersionHolder", false)]
        private readonly GameObject _remoteVersionHolder;

        [BetterOutline]
        [UIElement("RemoteVersionText")]
        private readonly Text _remoteVersionText;

        [UIElement("ArrowGraphic", false)]
        private readonly GameObject _arrowGraphic;

        [UIElement("ChangesPane", false)]
        private readonly GameObject _changesPane;

        [UIElement("WeaponSkinsChangesHolder", false)]
        private readonly GameObject _weaponSkinsChangesHolder;

        [BetterOutline]
        [UIElement("WeaponSkinsChanges")]
        private readonly Text _weaponSkinsChangesText;

        [UIElement("AccessoriesChangesHolder", false)]
        private readonly GameObject _accessoriesChangesHolder;

        [BetterOutline]
        [UIElement("AccessoriesChanges")]
        private readonly Text _accessoriesChangesText;

        [UIElement("PetsChangesHolder", false)]
        private readonly GameObject _petsChangesHolder;

        [BetterOutline]
        [UIElement("PetsChanges")]
        private readonly Text _petsChangesText;

        [UIElement("Panel")]
        private readonly RectTransform _panel;

        private float _dontActuallyRefreshRemoteVersionUntilTime;

        private bool _isGettingRemoteVersion;

        private float _lockExitTimer;

        private bool _lockExit;

        public override bool CloseOnEscapeButtonPress => CanExit();

        protected override void OnInitialized()
        {
            _progressBarFill.fillAmount = 0f;

            PersonalizationAssetsState state = PersonalizationManager.Instance.GetPersonalizationAssetsState();
            if (state != PersonalizationAssetsState.Installed) _lockExitTimer = 3f;
        }

        public override void Show()
        {
            base.Show();
            refreshContents();
            refreshChangesPane();

            PersonalizationAssetsState state = PersonalizationManager.Instance.GetPersonalizationAssetsState();
            string translationKey = state == PersonalizationAssetsState.NotInstalled ? "customization_need_install_header" : "customization_need_update_header";
            _header.text = LocalizationManager.Instance.GetTranslatedString(translationKey);
        }

        public override void Update()
        {
            base.Update();

            _lockExitTimer = Mathf.Max(0f, _lockExitTimer - Time.unscaledDeltaTime);
            refreshProgressBarFill();

            _exitButton.interactable = CanExit();
        }

        public bool CanExit() => !_lockExit && _lockExitTimer == 0f;

        private void refreshContents()
        {
            PersonalizationManager personalizationManager = PersonalizationManager.Instance;

            PersonalizationAssetsState state = personalizationManager.GetPersonalizationAssetsState();
            bool isDownloading = _isGettingRemoteVersion || personalizationManager.IsDownloadingCustomizationFile();

            _missingVersionHolder.SetActive(state == PersonalizationAssetsState.NotInstalled);
            _localVersionHolder.SetActive(state != PersonalizationAssetsState.NotInstalled);
            _arrowGraphic.SetActive(state == PersonalizationAssetsState.NeedUpdate);
            _remoteVersionHolder.SetActive(state == PersonalizationAssetsState.NeedUpdate);

            _updateButton.interactable = true;
            _progressBar.SetActive(isDownloading);
            switch (state)
            {
                case PersonalizationAssetsState.NotInstalled:
                    _installButton.gameObject.SetActive(!isDownloading);
                    _refreshButton.gameObject.SetActive(false);
                    _updateButton.gameObject.SetActive(false);
                    break;
                case PersonalizationAssetsState.Installed:
                    _installButton.gameObject.SetActive(false);
                    _refreshButton.gameObject.SetActive(!isDownloading);
                    _updateButton.gameObject.SetActive(!isDownloading);
                    _updateButton.interactable = false;
                    break;
                case PersonalizationAssetsState.NeedUpdate:
                    _installButton.gameObject.SetActive(false);
                    _refreshButton.gameObject.SetActive(false);
                    _updateButton.gameObject.SetActive(!isDownloading);
                    break;
            }

            PersonalizationAssetsVersion localVersion = personalizationManager.LocalAssetsVersion;
            if (localVersion == null || localVersion.UpdateNumber == -1)
            {
                _localVersionText.text = "None";
            }
            else
            {
                _localVersionText.text = localVersion.UpdateNumber.ToString();
            }

            PersonalizationAssetsVersion remoteVersion = personalizationManager.RemoteAssetsVersion;
            if (remoteVersion == null || remoteVersion.UpdateNumber == -1)
            {
                _remoteVersionText.text = "None";
            }
            else
            {
                _remoteVersionText.text = remoteVersion.UpdateNumber.ToString();
            }
        }

        private void refreshProgressBarFill()
        {
            if (_isGettingRemoteVersion)
            {
                _progressBarFill.fillAmount = 0f;
                return;
            }

            PersonalizationManager personalizationManager = PersonalizationManager.Instance;
            if (personalizationManager.IsDownloadingCustomizationFile())
            {
                _progressBarFill.fillAmount = Mathf.Lerp(_progressBarFill.fillAmount, personalizationManager.GetCustomizationFileDownloadProgress(), Time.unscaledDeltaTime * 12.5f);
            }
        }

        private void refreshChangesPane()
        {
            bool hasAnyVisibleChanges = true;
            PersonalizationManager personalizationManager = PersonalizationManager.Instance;

            PersonalizationAssetsVersion localVersion = personalizationManager.LocalAssetsVersion;
            if (localVersion == null || localVersion.UpdateNumber == -1) hasAnyVisibleChanges = false;

            PersonalizationAssetsVersion remoteVersion = personalizationManager.RemoteAssetsVersion;
            if (remoteVersion == null || remoteVersion.UpdateNumber == -1) hasAnyVisibleChanges = false;

            if (hasAnyVisibleChanges)
            {
                if(!remoteVersion.IsSuitableForComparison() || !localVersion.IsSuitableForComparison())
                {
                    hasAnyVisibleChanges = false;
                }
                else if (remoteVersion.GetTotalVerifiedItems() - localVersion.GetTotalVerifiedItems() <= 0)
                {
                    hasAnyVisibleChanges = false;
                }
            }

            _changesPane.SetActive(hasAnyVisibleChanges);

            Vector2 panelSizeDelta = _panel.sizeDelta;
            if (hasAnyVisibleChanges)
            {
                panelSizeDelta.y = PANEL_HEIGHT_UPDATE;

                int newWeaponSkins = remoteVersion.WeaponSkins.VerifiedCount - localVersion.WeaponSkins.VerifiedCount;
                int newAccessories = remoteVersion.Accessories.VerifiedCount - localVersion.Accessories.VerifiedCount;
                int newPets = remoteVersion.Pets.VerifiedCount - localVersion.Pets.VerifiedCount;

                _weaponSkinsChangesHolder.SetActive(newWeaponSkins > 0);
                _accessoriesChangesHolder.SetActive(newAccessories > 0 && ModFeatures.IsEnabled(ModFeatures.FeatureType.Accessories));
                _petsChangesHolder.SetActive(newPets > 0 && ModFeatures.IsEnabled(ModFeatures.FeatureType.Pets));

                _weaponSkinsChangesText.text = $"{newWeaponSkins} new weapon skins!";
                _accessoriesChangesText.text = $"{newAccessories} new accessories!";
                _petsChangesText.text = $"{newPets} new pets!";
            }
            else
            {
                panelSizeDelta.y = PANEL_HEIGHT_IDLE;
            }
            _panel.sizeDelta = panelSizeDelta;
        }

        public void OnInstallButtonClicked()
        {
            _progressBarFill.fillAmount = 0f;
            _lockExit = true;

            _isGettingRemoteVersion = true;
            PersonalizationManager.Instance.RefreshRemoteCustomizationAssetsVersion(delegate (bool result)
            {
                _isGettingRemoteVersion = false;
                if (!result)
                {
                    _lockExit = false;
                    refreshContents();
                    ModUIUtils.MessagePopupOK("Error", $"Could not get info about the latest version", true);
                    return;
                }

                OnUpdateButtonClicked();
            });
            refreshContents();
        }

        public void OnUpdateButtonClicked()
        {
            _lockExit = true;
            PersonalizationManager.Instance.DownloadCustomizationFile(delegate (string error)
            {
                _lockExit = false;
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
                {
                    refreshContents();
                    refreshChangesPane();
                }
            });
        }
    }
}
