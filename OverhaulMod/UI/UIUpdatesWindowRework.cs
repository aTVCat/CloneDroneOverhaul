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
            displayUpdatesLastCheckedIdleText();
        }

        private void displayUpdatesLastCheckedIdleText()
        {
            DateTime dateTime;
            if (UpdateManager.UpdatesLastCheckedDate.IsNullOrEmpty() || !DateTime.TryParse(UpdateManager.UpdatesLastCheckedDate, out dateTime))
                dateTime = DateTime.MinValue;

            m_idleHeaderText.text = "Overhaul mod is up-to-date!";
            m_idleDescriptionText.text = $"Last checked: {(dateTime == DateTime.MinValue ? "unknown" : dateTime.ToShortDateString())}";
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

                UpdateInfoList updateInfoList = result.Updates;
                if (updateInfoList.HasAnyNewBuildAvailable())
                {
                    m_resultElements.SetActive(true);

                    if (m_content.childCount != 0)
                        TransformUtils.DestroyAllChildren(m_content);

                    foreach (System.Collections.Generic.KeyValuePair<string, UpdateInfo> build in updateInfoList.Builds)
                    {
                        if (build.Value.IsOlderBuild() || !build.Value.CanBeInstalledByLocalUser())
                            continue;

                        instantiateBuildDisplay(build.Key, build.Value);
                    }

                    string n = UpdateManager.NotifyAboutNewVersionFromBranch;
                    if (!n.IsNullOrEmpty())
                    {
                        if (updateInfoList.Builds.ContainsKey(n))
                        {
                            UpdateInfo updateInfo = updateInfoList.Builds[n];
                            if (updateInfo.CanBeInstalledByLocalUser() && updateInfo.IsNewerBuild())
                            {
                                ModSettingsManager.SetStringValue(ModSettingsConstants.SAVED_NEW_VERSION, updateInfo.ModVersion.ToString());
                                ModSettingsManager.SetStringValue(ModSettingsConstants.SAVED_NEW_VERSION_BRANCH, n);
                            }
                        }
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
