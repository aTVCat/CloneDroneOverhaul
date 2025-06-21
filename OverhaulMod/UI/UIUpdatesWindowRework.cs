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
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnEditorButtonClicked))]
        [UIElement("EditorButton")]
        private readonly Button m_editorButton;

        [UIElementAction(nameof(OnCheckForUpdatesButtonClicked))]
        [UIElement("CheckForUpdatesButton")]
        private readonly Button m_checkForUpdatesButton;

        [UIElementAction(nameof(OnPatchNotesButtonClicked))]
        [UIElement("InstalledBuildChangelogButton")]
        private readonly Button m_patchNotesButton;

        [UIElementAction(nameof(OnCheckUpdatesOnStartToggled))]
        [UIElement("CheckUpdatesOnStartToggle")]
        private readonly Toggle m_checkUpdatesOnStartToggle;

        [UIElementAction(nameof(OnNotifyAboutTestBuildsToggled))]
        [UIElement("NotifyAboutTestBuildsToggle")]
        private readonly Toggle m_notifyAboutTestBuildsToggle;

        [UIElement("NotifyAboutTestBuildsToggle_Shading")]
        private readonly GameObject m_notifyAboutTestBuildsToggleShading;

        [UIElement("InstalledVersionText")]
        private readonly Text m_installedVersionText;

        [UIElement("IdleElements", true)]
        private readonly GameObject m_idleElements;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject m_loadingIndicator;

        [UIElement("ResultElements", false)]
        private readonly GameObject m_resultElements;

        [UIElement("IdleHeader")]
        private readonly Text m_idleHeaderText;

        [UIElement("IdleDescription")]
        private readonly Text m_idleDescriptionText;

        [UIElement("NewBuildDisplay", false)]
        private readonly ModdedObject m_buildDisplay;

        [UIElement("Content")]
        private readonly Transform m_content;

        public override bool hideTitleScreen => true;

        protected override void OnInitialized()
        {
            m_installedVersionText.text = ModBuildInfo.versionStringNoBranch;
            m_checkUpdatesOnStartToggle.isOn = UpdateManager.CheckForUpdatesOnStartup;
            m_notifyAboutTestBuildsToggle.isOn = UpdateManager.NotifyAboutNewTestBuilds;
            m_notifyAboutTestBuildsToggle.interactable = UpdateManager.CheckForUpdatesOnStartup;
            m_notifyAboutTestBuildsToggleShading.SetActive(!UpdateManager.CheckForUpdatesOnStartup);
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

            m_idleHeaderText.text = LocalizationManager.Instance.GetTranslatedString("updates_text_overhaul_mod_is_up_to_date");
            m_idleDescriptionText.text = $"{LocalizationManager.Instance.GetTranslatedString("updates_tooltip_last_checked")} {(dateTime == DateTime.MinValue ? "unknown" : dateTime.ToShortDateString())}";
        }

        public void OnCheckUpdatesOnStartToggled(bool value)
        {
            ModSettingsManager.SetBoolValue(ModSettingsConstants.CHECK_UPDATES_ON_NEXT_START, true);
            ModSettingsManager.SetBoolValue(ModSettingsConstants.CHECK_FOR_UPDATES_ON_STARTUP, value, true);

            m_notifyAboutTestBuildsToggleShading.SetActive(!value);
            m_notifyAboutTestBuildsToggle.interactable = value;
        }

        public void OnNotifyAboutTestBuildsToggled(bool value)
        {
            ModSettingsManager.SetBoolValue(ModSettingsConstants.CHECK_UPDATES_ON_NEXT_START, true);
            ModSettingsManager.SetBoolValue(ModSettingsConstants.NOTIFY_ABOUT_NEW_TEST_BUILDS, value, true);
        }

        public void OnEditorButtonClicked()
        {
            ModUIConstants.ShowUpdatesEditor(base.transform);
        }

        public void OnPatchNotesButtonClicked()
        {
            Hide();
            ModUIConstants.ShowPatchNotes(new UIPatchNotes.ShowArguments()
            {
                CloseButtonActive = true,
                PanelOffset = Vector2.zero,
                ShrinkPanel = false,
                HideVersionList = false,
            }).ClickOnFirstButton();
        }

        public void OnCheckForUpdatesButtonClicked()
        {
            m_idleElements.SetActive(false);
            m_loadingIndicator.SetActive(true);
            m_resultElements.SetActive(false);
            m_checkForUpdatesButton.interactable = false;

            UpdateManager.Instance.DownloadUpdatesList(delegate (UpdateManager.GetUpdatesResult result)
            {
                m_loadingIndicator.SetActive(false);
                m_checkForUpdatesButton.interactable = true;

                if (result.IsError())
                {
                    m_idleElements.SetActive(true);
                    m_idleHeaderText.text = "An error occurred.";
                    m_idleDescriptionText.text = result.Error;
                    return;
                }

                bool showBuildsAnyway = ExclusivePerkManager.Instance.HasUnlockedPerk(ExclusivePerkType.SpecialRole);

                UpdateInfoList updateInfoList = result.Updates;
                if (showBuildsAnyway || updateInfoList.HasAnyNewBuildAvailable())
                {
                    m_resultElements.SetActive(true);

                    if (m_content.childCount != 0)
                        TransformUtils.DestroyAllChildren(m_content);

                    foreach (System.Collections.Generic.KeyValuePair<string, UpdateInfo> build in updateInfoList.Builds)
                    {
                        if ((build.Value.IsOlderBuild() || !build.Value.CanBeInstalledByLocalUser()) && !showBuildsAnyway)
                            continue;

                        instantiateBuildDisplay(build.Key, build.Value);
                    }

                    ModSettingsManager.SetStringValue(ModSettingsConstants.UPDATES_LAST_CHECKED_DATE, DateTime.Now.ToString());
                    ModSettingsDataManager.Instance.Save();
                }
                else
                {
                    m_idleElements.SetActive(true);

                    displayUpdatesLastCheckedIdleText();
                }
            });
        }

        private void instantiateBuildDisplay(string branch, UpdateInfo updateInfo)
        {
            ModdedObject moddedObject = Instantiate(m_buildDisplay, m_content);
            moddedObject.gameObject.SetActive(true);
            moddedObject.GetObject<Text>(0).text = updateInfo.ModVersion?.ToString();
            moddedObject.GetObject<Text>(1).text = branch.ToUpper();
            moddedObject.GetObject<GameObject>(2).SetActive(branch != UpdateInfoList.RELEASE_BRANCH && branch != UpdateInfoList.PREVIEW_BRANCH);

            Button button = moddedObject.GetComponent<Button>();
            button.onClick.AddListener(delegate
            {
                ModUIConstants.ShowUpdateDetailsWindow(base.transform, updateInfo, branch);
            });
        }
    }
}
