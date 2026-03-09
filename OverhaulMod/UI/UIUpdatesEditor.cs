using OverhaulMod.Content;
using OverhaulMod.Utils;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIUpdatesEditor : OverhaulUIBehaviour
    {
        private static readonly char[] s_allowedChars = "1234567890.".ToCharArray();

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button _saveButton;

        [UIElement("NeedsSaveIcon", false)]
        private readonly GameObject _needsSaveIcon;

        [UIElementAction(nameof(OnSavesFolderButtonClicked))]
        [UIElement("SavesFolderButton")]
        private readonly Button _savesFolderButton;

        [UIElementAction(nameof(OnNewBranchButtonClicked))]
        [UIElement("NewBranchButton")]
        private readonly Button _newBranchButton;

        [UIElementAction(nameof(OnGetUpdatesFileButtonClicked))]
        [UIElement("GetUpdatesFileButton")]
        private readonly Button _getUpdatesFileButton;

        [UIElementAction(nameof(OnPreviewChangelogButtonClicked))]
        [UIElement("PreviewChangelogButton")]
        private readonly Button _previewChangelogButton;

        [UIElementAction(nameof(OnVersionFieldChanged))]
        [UIElement("BuildVersionField")]
        private readonly InputField _buildVersionField;

        [UIElementAction(nameof(OnBranchNameFieldChanged))]
        [UIElement("BuildBranchField")]
        private readonly InputField _buildBranchField;

        [UIElementAction(nameof(OnAllowedUsersFieldChanged))]
        [UIElement("AllowedUsersField")]
        private readonly InputField _allowedUsersField;

        [UIElementAction(nameof(OnRequiredExclusivePerkDropdownChanged))]
        [UIElement("ExclusivePerkRequirementDropdown")]
        private readonly Dropdown _exclusivePerkRequirementDropdown;

        [UIElementAction(nameof(OnBuildFileURLFieldChanged))]
        [UIElement("BuildFileURLField")]
        private readonly InputField _buildFileURLField;

        [UIElementAction(nameof(OnGoogleDriveLinkToggleChanged))]
        [UIElement("IsGoogleDriveLinkToggle")]
        private readonly Toggle _isGoogleDriveLinkToggle;

        [UIElementAction(nameof(OnChangelogFileFieldChanged))]
        [UIElement("ChangelogFileField")]
        private readonly InputField _changelogFileField;

        [UIElement("ChangelogFileExistsIcon", false)]
        private readonly GameObject _changelogFileExistsIcon;

        [UIElementAction(nameof(OnEditPatchNotesFileButtonClicked))]
        [UIElement("EditChangelogFileButton")]
        private readonly Button _editChangelogFileButton;

        [UIElementAction(nameof(OnRefreshChangelogButtonClicked))]
        [UIElement("RefreshChangelogButton")]
        private readonly Button _refreshChangelogButton;

        [UIElement("BuildDisplay", false)]
        private readonly ModdedObject _branchDisplay;

        [UIElement("Content")]
        private readonly Transform _content;

        private bool _disallowCallbacks;

        private UpdateInfoList _updatesList;

        private UpdateInfo _editingUpdate;

        private string _editingBranch;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            UpdateManager.Instance.LoadDataFromDisk();
            _updatesList = UpdateManager.Instance.GetUpdatesList();

            System.Collections.Generic.List<Dropdown.OptionData> list = _exclusivePerkRequirementDropdown.options;
            list.Clear();
            foreach (ExclusivePerkType perk in typeof(ExclusivePerkType).GetEnumValues())
            {
                list.Add(new Dropdown.OptionData() { text = StringUtils.AddSpacesToCamelCasedString(perk.ToString()) });
            }
            _exclusivePerkRequirementDropdown.RefreshShownValue();

            populateBranches();
        }

        private void populateBranches()
        {
            if (_content.childCount != 0)
                TransformUtils.DestroyAllChildren(_content);

            foreach (System.Collections.Generic.KeyValuePair<string, UpdateInfo> update in _updatesList.Builds)
            {
                ModdedObject moddedObject = Instantiate(_branchDisplay, _content);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = $"{update.Value.ModVersion} ({update.Value.DisplayVersion})";
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
            _disallowCallbacks = true;

            _editingUpdate = update;
            _editingBranch = branch;

            _buildVersionField.text = update.ModVersion?.ToString();
            _buildBranchField.text = branch;
            _allowedUsersField.text = update.AllowedUsers;
            _exclusivePerkRequirementDropdown.value = (int)update.RequireExclusivePerk;
            _buildFileURLField.text = update.DownloadLink;
            _isGoogleDriveLinkToggle.isOn = update.IsGoogleDriveLink;

            _needsSaveIcon.SetActive(false);

            _disallowCallbacks = false;
        }

        private void setUpdateInfoFromInputs()
        {
            if (_editingUpdate == null)
                return;

            Version version;
            if (!Version.TryParse(_buildVersionField.text, out version))
                version = new Version(0, 0, 0, 0);

            OnRefreshChangelogButtonClicked();
            _editingUpdate.ModVersion = version;
            _editingUpdate.AllowedUsers = _allowedUsersField.text;
            _editingUpdate.RequireExclusivePerk = (ExclusivePerkType)_exclusivePerkRequirementDropdown.value;
            _editingUpdate.DownloadLink = _buildFileURLField.text;
            _editingUpdate.IsGoogleDriveLink = _isGoogleDriveLinkToggle.isOn;
            _editingUpdate.FixValues();
        }

        public void OnSaveButtonClicked()
        {
            setUpdateInfoFromInputs();
            _updatesList.SetReleasesValuesForOldVersions();

            ModJsonUtils.WriteStream(Path.Combine(ModCore.DeveloperFolder, UpdateManager.REPOSITORY_FILE), _updatesList);

            _needsSaveIcon.SetActive(false);
        }

        public void OnSavesFolderButtonClicked()
        {
            ModFileUtils.OpenFileExplorer(ModCore.DeveloperFolder);
        }

        public void OnGetUpdatesFileButtonClicked()
        {
            _getUpdatesFileButton.interactable = false;
            UpdateManager.Instance.DownloadUpdatesList(delegate (UpdateManager.GetUpdatesResult updateInfoList)
            {
                _getUpdatesFileButton.interactable = true;
                if (updateInfoList.IsError())
                {
                    ModUIUtils.MessagePopupOK("Error", updateInfoList.Error, true);
                    return;
                }

                _updatesList = updateInfoList.Updates;
                populateBranches();
            });
        }

        public void OnNewBranchButtonClicked()
        {
            if (_updatesList.Builds.ContainsKey("new branch"))
                return;

            OnSaveButtonClicked();

            UpdateInfo updateInfo = new UpdateInfo();
            string branch = "new branch";

            _updatesList.Builds.Add(branch, updateInfo);

            populateBranches();
            editUpdate(updateInfo, branch);
        }

        public void OnEditPatchNotesFileButtonClicked()
        {
            string path = _changelogFileField.text;
            if (!File.Exists(path))
            {
                return;
            }

            ModFileUtils.OpenFile(path);
        }

        public void OnRefreshChangelogButtonClicked()
        {
            string path = _changelogFileField.text;
            if (!File.Exists(path))
            {
                _changelogFileExistsIcon.SetActive(false);
                return;
            }

            string content = ModFileUtils.ReadText(path);
            _editingUpdate.Changelog = content;

            _changelogFileExistsIcon.SetActive(true);
        }

        public void OnPreviewChangelogButtonClicked()
        {
            UIPatchNotes patchNotes = ModUIConstants.ShowPatchNotes(base.transform, new UIPatchNotes.ShowArguments()
            {
                CloseButtonActive = true,
                PanelOffset = Vector2.zero,
                ShrinkPanel = true,
                HideVersionList = true,
            });
            patchNotes.PopulateText($"{_editingUpdate.DisplayVersion} [{_editingBranch}]", _editingUpdate.Changelog);
        }

        public void OnVersionFieldChanged(string value)
        {
            if (_disallowCallbacks)
                return;

            Version version;
            if (!Version.TryParse(_buildVersionField.text, out version))
                version = new Version(0, 0, 0);

            _editingUpdate.ModVersion = version;
            _editingUpdate.FixValues();

            _needsSaveIcon.SetActive(true);
            populateBranches();
        }

        public void OnBranchNameFieldChanged(string value)
        {
            if (_disallowCallbacks)
                return;

            UpdateInfo updateInfo = _updatesList.Builds[_editingBranch];
            _updatesList.Builds.Remove(_editingBranch);
            _updatesList.Builds.Add(value, updateInfo);
            _editingBranch = value;

            _needsSaveIcon.SetActive(true);
            populateBranches();
        }

        public void OnAllowedUsersFieldChanged(string value)
        {
            if (_disallowCallbacks)
                return;

            _needsSaveIcon.SetActive(true);
        }

        public void OnRequiredExclusivePerkDropdownChanged(int value)
        {
            if (_disallowCallbacks)
                return;

            _needsSaveIcon.SetActive(true);
        }

        public void OnBuildFileURLFieldChanged(string value)
        {
            if (_disallowCallbacks)
                return;

            _needsSaveIcon.SetActive(true);
        }

        public void OnChangelogFileFieldChanged(string value)
        {
            if (_disallowCallbacks)
                return;

            _needsSaveIcon.SetActive(true);
        }

        public void OnGoogleDriveLinkToggleChanged(bool value)
        {
            if (_disallowCallbacks)
                return;

            _needsSaveIcon.SetActive(true);
        }
    }
}
