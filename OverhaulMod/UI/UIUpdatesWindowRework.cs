using OverhaulMod.Content;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIUpdatesWindowRework : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnEditorButtonClicked))]
        [UIElement("EditorButton")]
        private readonly Button _editorButton;

        [UIElementAction(nameof(OnCheckForUpdatesButtonClicked))]
        [UIElement("CheckForUpdatesButton")]
        private readonly Button _checkForUpdatesButton;

        [UIElementAction(nameof(OnPatchNotesButtonClicked))]
        [UIElement("InstalledBuildChangelogButton")]
        private readonly Button _patchNotesButton;

        [UIElementAction(nameof(OnCheckUpdatesOnStartToggled))]
        [UIElement("CheckUpdatesOnStartToggle")]
        private readonly Toggle _checkUpdatesOnStartToggle;

        [UIElementAction(nameof(OnNotifyAboutTestBuildsToggled))]
        [UIElement("NotifyAboutTestBuildsToggle")]
        private readonly Toggle _notifyAboutTestBuildsToggle;

        [UIElement("NotifyAboutTestBuildsToggle_Shading")]
        private readonly GameObject _notifyAboutTestBuildsToggleShading;

        [UIElement("InstalledVersionText")]
        private readonly Text _installedVersionText;

        [UIElement("IdleElements", true)]
        private readonly GameObject _idleElements;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject _loadingIndicator;

        [UIElement("ResultElements", false)]
        private readonly GameObject _resultElements;

        [UIElement("IdleHeader")]
        private readonly Text _idleHeaderText;

        [UIElement("IdleDescription")]
        private readonly Text _idleDescriptionText;

        [UIElement("NewBuildDisplay", false)]
        private readonly ModdedObject _buildDisplay;

        [UIElement("Content")]
        private readonly Transform _content;

        public override bool HideTitleScreen => true;

        protected override void OnInitialized()
        {
            _installedVersionText.text = ModBuild.VersionString;
            _checkUpdatesOnStartToggle.isOn = UpdateManager.CheckForUpdatesOnStartup;
            _notifyAboutTestBuildsToggle.isOn = UpdateManager.NotifyAboutNewTestBuilds;
            _notifyAboutTestBuildsToggle.interactable = UpdateManager.CheckForUpdatesOnStartup;
            _notifyAboutTestBuildsToggleShading.SetActive(!UpdateManager.CheckForUpdatesOnStartup);
            displayUpdatesLastCheckedIdleText();
        }

        public override void Hide()
        {
            ModSettingsDataManager.Instance.Save();
            base.Hide();
        }

        private void displayUpdatesLastCheckedIdleText()
        {
            DateTime dateTime;
            if (UpdateManager.UpdatesLastCheckedDate.IsNullOrEmpty() || !DateTime.TryParse(UpdateManager.UpdatesLastCheckedDate, out dateTime))
                dateTime = DateTime.MinValue;

            _idleHeaderText.text = LocalizationManager.Instance.GetTranslatedString("updates_text_overhaul_mod_is_up_to_date");
            _idleDescriptionText.text = $"{LocalizationManager.Instance.GetTranslatedString("updates_tooltip_last_checked")} {(dateTime == DateTime.MinValue ? "unknown" : dateTime.ToShortDateString())}";
        }

        public void OnCheckUpdatesOnStartToggled(bool value)
        {
            ModSettingsManager.SetBoolValue(ModSettingIDs.CHECK_UPDATES_ON_NEXT_START, true);
            ModSettingsManager.SetBoolValue(ModSettingIDs.CHECK_FOR_UPDATES_ON_STARTUP, value, true);

            _notifyAboutTestBuildsToggleShading.SetActive(!value);
            _notifyAboutTestBuildsToggle.interactable = value;
        }

        public void OnNotifyAboutTestBuildsToggled(bool value)
        {
            ModSettingsManager.SetBoolValue(ModSettingIDs.CHECK_UPDATES_ON_NEXT_START, true);
            ModSettingsManager.SetBoolValue(ModSettingIDs.NOTIFY_ABOUT_NEW_TEST_BUILDS, value, true);
        }

        public void OnEditorButtonClicked()
        {
            ModUIs.ShowUpdatesEditor(base.transform);
        }

        public void OnPatchNotesButtonClicked()
        {
            Hide();
            ModUIs.ShowPatchNotes(new UIPatchNotes.ShowArguments()
            {
                CloseButtonActive = true,
                PanelOffset = Vector2.zero,
                ShrinkPanel = false,
                HideVersionList = false,
            }).ClickOnFirstButton();
        }

        public void OnCheckForUpdatesButtonClicked()
        {
            _idleElements.SetActive(false);
            _loadingIndicator.SetActive(true);
            _resultElements.SetActive(false);
            _checkForUpdatesButton.interactable = false;

            UpdateManager.Instance.DownloadUpdatesList(delegate (UpdateManager.GetUpdatesResult result)
            {
                _loadingIndicator.SetActive(false);
                _checkForUpdatesButton.interactable = true;

                if (result.HasFailed())
                {
                    _idleElements.SetActive(true);
                    _idleHeaderText.text = "An error occurred.";
                    _idleDescriptionText.text = result.Error;
                    return;
                }

                bool showBuildsAnyway = ExclusivePerkManager.Instance.HasUnlockedPerk(ExclusivePerkType.SpecialRole);

                UpdateInfoList updateInfoList = result.Updates;
                if (showBuildsAnyway || updateInfoList.HasAnyNewBuildAvailable())
                {
                    _resultElements.SetActive(true);

                    if (_content.childCount != 0)
                        TransformUtils.DestroyAllChildren(_content);

                    foreach (System.Collections.Generic.KeyValuePair<string, UpdateInfo> build in updateInfoList.Builds)
                    {
                        if ((build.Value.IsOlderBuild() || !build.Value.CanBeInstalledByLocalUser()) && !showBuildsAnyway)
                            continue;

                        instantiateBuildDisplay(build.Key, build.Value);
                    }

                    ModSettingsManager.SetStringValue(ModSettingIDs.UPDATES_LAST_CHECKED_DATE, DateTime.Now.ToString());
                    ModSettingsDataManager.Instance.Save();
                }
                else
                {
                    _idleElements.SetActive(true);

                    displayUpdatesLastCheckedIdleText();
                }
            });
        }

        private void instantiateBuildDisplay(string branch, UpdateInfo updateInfo)
        {
            ModdedObject moddedObject = Instantiate(_buildDisplay, _content);
            moddedObject.gameObject.SetActive(true);
            moddedObject.GetObject<Text>(0).text = updateInfo.DisplayVersion?.ToString();
            moddedObject.GetObject<Text>(1).text = branch.ToUpper();
            moddedObject.GetObject<GameObject>(2).SetActive(branch != UpdateInfoList.RELEASE_BRANCH && branch != UpdateInfoList.PREVIEW_BRANCH);

            Button button = moddedObject.GetComponent<Button>();
            button.onClick.AddListener(delegate
            {
                ModUIs.ShowUpdateDetailsWindow(base.transform, updateInfo, branch);
            });
        }
    }
}
