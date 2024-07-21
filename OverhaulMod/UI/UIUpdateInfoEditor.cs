using OverhaulMod.Content;
using OverhaulMod.Utils;
using System;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIUpdateInfoEditor : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnDataFolderButtonClicked))]
        [UIElement("DataFolderButton")]
        private readonly Button m_dataFolderButton;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button m_saveButton;

        [UIElementAction(nameof(OnRetrieveButtonClicked))]
        [UIElement("RetrieveDataFromServerButton")]
        private readonly Button m_retrieveButton;

        [UIElementAction(nameof(OnBranchDropdownEdited))]
        [UIElement("BranchDropdown")]
        private readonly Dropdown m_branchDropdown;

        [UIElement("ModVersionIF")]
        private readonly InputField m_modVersionField;

        [UIElement("ModBotVersionIF")]
        private readonly InputField m_modBotVersionField;

        [UIElement("DownloadIF")]
        private readonly InputField m_downloadLinkField;

        [UIElement("ChangelogIF")]
        private readonly InputField m_changelogField;

        private UpdateInfo m_editingInfo;
        private UpdateInfoList m_editingInfoList;

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void PopulateFields()
        {
            if (m_editingInfo == null)
                return;

            m_modVersionField.text = m_editingInfo.ModVersion?.ToString();
            m_modBotVersionField.text = m_editingInfo.ModBotVersion.ToString();
            m_downloadLinkField.text = m_editingInfo.DownloadLink;
            m_changelogField.text = m_editingInfo.Changelog;
        }

        public void OnDataFolderButtonClicked()
        {
            _ = ModIOUtils.OpenFileExplorer(ModCore.savesFolder);
        }

        public void OnSaveButtonClicked()
        {
            if (m_editingInfo == null)
                return;

            if (m_editingInfoList == null)
                m_editingInfoList = new UpdateInfoList();

            Version parsedModVersion;
            try
            {
                parsedModVersion = Version.Parse(m_modVersionField.text);
            }
            catch (Exception exc)
            {
                ModUIUtils.MessagePopupOK("Error - Mod ver", exc.ToString(), 300f);
                return;
            }

            int parsedModBotVersion;
            try
            {
                parsedModBotVersion = int.Parse(m_modBotVersionField.text);
            }
            catch (Exception exc)
            {
                ModUIUtils.MessagePopupOK("Error - ModBot ver", exc.ToString(), 300f);
                return;
            }

            string downloadLink = m_downloadLinkField.text;
            string changelog = m_changelogField.text;
            m_editingInfo.ModVersion = parsedModVersion;
            m_editingInfo.ModBotVersion = parsedModBotVersion;
            m_editingInfo.DownloadLink = downloadLink;
            m_editingInfo.Changelog = changelog;
            switch (m_branchDropdown.value)
            {
                case 0:
                    m_editingInfoList.ModBotRelease = m_editingInfo;
                    break;
                case 1:
                    m_editingInfoList.GitHubRelease = m_editingInfo;
                    break;
                case 2:
                    m_editingInfoList.InternalRelease = m_editingInfo;
                    break;
            }

            ModDataManager.Instance.SerializeToFile(UpdateManager.REPOSITORY_FILE, m_editingInfoList, true);
        }

        public void OnRetrieveButtonClicked()
        {
            m_retrieveButton.interactable = false;
            UpdateManager.Instance.DownloadUpdateInfoFile(delegate (UpdateInfoList updateInfoList)
            {
                m_retrieveButton.interactable = true;
                m_editingInfoList = updateInfoList;
                OnBranchDropdownEdited(0);
            }, delegate (string error)
            {
                m_retrieveButton.interactable = true;
                ModUIUtils.MessagePopupOK("Error", error);
            });
        }

        public void OnBranchDropdownEdited(int value)
        {
            if (m_editingInfoList != null)
            {
                switch (value)
                {
                    case 0:
                        m_editingInfo = m_editingInfoList.ModBotRelease;
                        break;
                    case 1:
                        m_editingInfo = m_editingInfoList.GitHubRelease;
                        break;
                    case 2:
                        m_editingInfo = m_editingInfoList.InternalRelease;
                        break;
                }
            }

            if (m_editingInfo == null)
                m_editingInfo = new UpdateInfo();

            PopulateFields();
        }
    }
}
