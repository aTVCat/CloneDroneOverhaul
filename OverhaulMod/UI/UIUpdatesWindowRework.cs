using OverhaulMod.Content;
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
            ModUIConstants.ShowPatchNotes().ClickOnFirstButton();
        }

        public void OnCheckForUpdatesButtonClicked()
        {
            m_idleElements.SetActive(false);
            m_loadingIndicator.SetActive(true);
            m_resultElements.SetActive(false);
            m_checkForUpdatesButton.interactable = false;

            UpdateManager.Instance.DownloadUpdateInfoFile(delegate (UpdateInfoList updateInfoList)
            {
                m_loadingIndicator.SetActive(false);
                if (updateInfoList.HasAnyNewBuildAvailable())
                {
                    m_resultElements.SetActive(true);
                }
                else
                {
                    m_idleElements.SetActive(true);

                    m_checkForUpdatesButton.interactable = true;

                    displayUpdatesLastCheckedIdleText();
                }

            }, delegate (string error)
            {
                m_idleElements.SetActive(true);
                m_loadingIndicator.SetActive(false);
                m_checkForUpdatesButton.interactable = true;

                m_idleHeaderText.text = "An error occurred.";
                m_idleDescriptionText.text = error;
            });
        }
    }
}
