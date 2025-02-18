using OverhaulMod.Content;
using OverhaulMod.Utils;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIUpdatesEditor : OverhaulUIBehaviour
    {
        private static readonly char[] s_allowedChars = "1234567890.".ToCharArray();

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button m_saveButton;

        [UIElement("NeedsSaveIcon", false)]
        private readonly GameObject m_needsSaveIcon;

        [UIElementAction(nameof(OnSavesFolderButtonClicked))]
        [UIElement("SavesFolderButton")]
        private readonly Button m_savesFolderButton;

        [UIElementAction(nameof(OnNewBranchButtonClicked))]
        [UIElement("NewBranchButton")]
        private readonly Button m_newBranchButton;

        [UIElementAction(nameof(OnGetUpdatesFileButtonClicked))]
        [UIElement("GetUpdatesFileButton")]
        private readonly Button m_getUpdatesFileButton;

        [UIElementAction(nameof(OnPreviewChangelogButtonClicked))]
        [UIElement("PreviewChangelogButton")]
        private readonly Button m_previewChangelogButton;

        [UIElementAction(nameof(OnVersionFieldChanged))]
        [UIElement("BuildVersionField")]
        private readonly InputField m_buildVersionField;

        [UIElementAction(nameof(OnBranchNameFieldChanged))]
        [UIElement("BuildBranchField")]
        private readonly InputField m_buildBranchField;

        [UIElementAction(nameof(OnAllowedUsersFieldChanged))]
        [UIElement("AllowedUsersField")]
        private readonly InputField m_allowedUsersField;

        [UIElementAction(nameof(OnRequiredExclusivePerkDropdownChanged))]
        [UIElement("ExclusivePerkRequirementDropdown")]
        private readonly Dropdown m_exclusivePerkRequirementDropdown;

        [UIElementAction(nameof(OnBuildFileURLFieldChanged))]
        [UIElement("BuildFileURLField")]
        private readonly InputField m_buildFileURLField;

        [UIElementAction(nameof(OnGoogleDriveLinkToggleChanged))]
        [UIElement("IsGoogleDriveLinkToggle")]
        private readonly Toggle m_isGoogleDriveLinkToggle;

        [UIElementAction(nameof(OnChangelogFileFieldChanged))]
        [UIElement("ChangelogFileField")]
        private readonly InputField m_changelogFileField;

        [UIElement("ChangelogFileExistsIcon", false)]
        private readonly GameObject m_changelogFileExistsIcon;

        [UIElementAction(nameof(OnEditPatchNotesFileButtonClicked))]
        [UIElement("EditChangelogFileButton")]
        private readonly Button m_editChangelogFileButton;

        [UIElementAction(nameof(OnRefreshChangelogButtonClicked))]
        [UIElement("RefreshChangelogButton")]
        private readonly Button m_refreshChangelogButton;

        [UIElement("BuildDisplay", false)]
        private readonly ModdedObject m_branchDisplay;

        [UIElement("Content")]
        private readonly Transform m_content;

        private bool m_disallowCallbacks;

        private UpdateInfoList m_updatesList;

        private UpdateInfo m_editingUpdate;

        private string m_editingBranch;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            UpdateManager.Instance.LoadDataFromDisk();
            m_updatesList = UpdateManager.Instance.GetUpdatesList();

            System.Collections.Generic.List<Dropdown.OptionData> list = m_exclusivePerkRequirementDropdown.options;
            list.Clear();
            foreach (ExclusivePerkType perk in typeof(ExclusivePerkType).GetEnumValues())
            {
                list.Add(new Dropdown.OptionData() { text = StringUtils.AddSpacesToCamelCasedString(perk.ToString()) });
            }
            m_exclusivePerkRequirementDropdown.RefreshShownValue();

            populateBranches();
        }

        private void populateBranches()
        {
            if (m_content.childCount != 0)
                TransformUtils.DestroyAllChildren(m_content);

            foreach (System.Collections.Generic.KeyValuePair<string, UpdateInfo> update in m_updatesList.Builds)
            {
                string displayName;
                if (update.Value.ModVersion == null)
                {
                    displayName = "NULL";
                }
                else
                {
                    displayName = update.Value.ModVersion.ToString();
                }

                ModdedObject moddedObject = Instantiate(m_branchDisplay, m_content);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = displayName;
                moddedObject.GetObject<Text>(1).text = update.Key.ToUpper();

                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    editUpdate(update.Value, update.Key);
                });
            }
        }

        private void editUpdate(UpdateInfo update, string branch)
        {
            m_disallowCallbacks = true;

            m_editingUpdate = update;
            m_editingBranch = branch;

            m_buildVersionField.text = update.ModVersion?.ToString();
            m_buildBranchField.text = branch;
            m_allowedUsersField.text = update.AllowedUsers;
            m_exclusivePerkRequirementDropdown.value = (int)update.RequireExclusivePerk;
            m_buildFileURLField.text = update.DownloadLink;
            m_isGoogleDriveLinkToggle.isOn = update.IsGoogleDriveLink;

            m_needsSaveIcon.SetActive(false);

            m_disallowCallbacks = false;
        }

        private void setUpdateInfoFromInputs()
        {
            if (m_editingUpdate == null)
                return;

            Version version;
            if(!Version.TryParse(m_buildVersionField.text, out version))
                version = new Version(0, 0, 0, 0);

            OnRefreshChangelogButtonClicked();
            m_editingUpdate.ModVersion = version;
            m_editingUpdate.AllowedUsers = m_allowedUsersField.text;
            m_editingUpdate.RequireExclusivePerk = (ExclusivePerkType)m_exclusivePerkRequirementDropdown.value;
            m_editingUpdate.DownloadLink = m_buildFileURLField.text;
            m_editingUpdate.IsGoogleDriveLink = m_isGoogleDriveLinkToggle.isOn;
        }

        public void OnSaveButtonClicked()
        {
            setUpdateInfoFromInputs();
            m_updatesList.SetReleasesValuesForOldVersions();

            ModJsonUtils.WriteStream(Path.Combine(ModCore.developerFolder, UpdateManager.REPOSITORY_FILE), m_updatesList);

            m_needsSaveIcon.SetActive(false);
        }

        public void OnSavesFolderButtonClicked()
        {
            ModFileUtils.OpenFileExplorer(ModCore.developerFolder);
        }

        public void OnGetUpdatesFileButtonClicked()
        {
            m_getUpdatesFileButton.interactable = false;
            UpdateManager.Instance.DownloadUpdatesList(delegate (UpdateManager.GetUpdatesResult updateInfoList)
            {
                m_getUpdatesFileButton.interactable = true;
                if (updateInfoList.IsError())
                {
                    ModUIUtils.MessagePopupOK("Error", updateInfoList.Error, true);
                    return;
                }

                m_updatesList = updateInfoList.Updates;
                populateBranches();
            });
        }

        public void OnNewBranchButtonClicked()
        {
            if (m_updatesList.Builds.ContainsKey("new branch"))
                return;

            OnSaveButtonClicked();

            UpdateInfo updateInfo = new UpdateInfo();
            string branch = "new branch";

            m_updatesList.Builds.Add(branch, updateInfo);

            populateBranches();
            editUpdate(updateInfo, branch);
        }

        public void OnEditPatchNotesFileButtonClicked()
        {
            string path = m_changelogFileField.text;
            if (!File.Exists(path))
            {
                return;
            }

            ModFileUtils.OpenFile(path);
        }

        public void OnRefreshChangelogButtonClicked()
        {
            string path = m_changelogFileField.text;
            if (!File.Exists(path))
            {
                m_changelogFileExistsIcon.SetActive(false);
                return;
            }

            string content = ModFileUtils.ReadText(path);
            m_editingUpdate.Changelog = content;

            m_changelogFileExistsIcon.SetActive(true);
        }

        public void OnPreviewChangelogButtonClicked()
        {
            UIPatchNotes patchNotes = ModUIConstants.ShowPatchNotes(base.transform, false, false);
            patchNotes.PopulateText($"{m_editingUpdate.ModVersion} [{m_editingBranch}]", m_editingUpdate.Changelog);
        }

        public void OnVersionFieldChanged(string value)
        {
            if (m_disallowCallbacks)
                return;

            Version version;
            if (!Version.TryParse(m_buildVersionField.text, out version))
                version = new Version(0, 0, 0, 0);

            m_editingUpdate.ModVersion = version;

            m_needsSaveIcon.SetActive(true);
            populateBranches();
        }

        public void OnBranchNameFieldChanged(string value)
        {
            if (m_disallowCallbacks)
                return;

            UpdateInfo updateInfo = m_updatesList.Builds[m_editingBranch];
            m_updatesList.Builds.Remove(m_editingBranch);
            m_updatesList.Builds.Add(value, updateInfo);
            m_editingBranch = value;

            m_needsSaveIcon.SetActive(true);
            populateBranches();
        }

        public void OnAllowedUsersFieldChanged(string value)
        {
            if (m_disallowCallbacks)
                return;

            m_needsSaveIcon.SetActive(true);
        }

        public void OnRequiredExclusivePerkDropdownChanged(int value)
        {
            if (m_disallowCallbacks)
                return;

            m_needsSaveIcon.SetActive(true);
        }

        public void OnBuildFileURLFieldChanged(string value)
        {
            if (m_disallowCallbacks)
                return;

            m_needsSaveIcon.SetActive(true);
        }

        public void OnChangelogFileFieldChanged(string value)
        {
            if (m_disallowCallbacks)
                return;

            m_needsSaveIcon.SetActive(true);
        }

        public void OnGoogleDriveLinkToggleChanged(bool value)
        {
            if (m_disallowCallbacks)
                return;

            m_needsSaveIcon.SetActive(true);
        }
    }
}
