using OverhaulMod.Content;
using OverhaulMod.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIUpdatesEditor : OverhaulUIBehaviour
    {
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

        private bool m_disallowCallbacks;

        public void OnSaveButtonClicked()
        {

        }

        public void OnSavesFolderButtonClicked()
        {

        }

        public void OnGetUpdatesFileButtonClicked()
        {

        }

        public void OnNewBranchButtonClicked()
        {

        }

        public void OnEditPatchNotesFileButtonClicked()
        {

        }

        public void OnRefreshChangelogButtonClicked()
        {

        }

        public void OnVersionFieldChanged(string value)
        {
            if (m_disallowCallbacks)
                return;
        }

        public void OnBranchNameFieldChanged(string value)
        {
            if (m_disallowCallbacks)
                return;
        }

        public void OnAllowedUsersFieldChanged(string value)
        {
            if (m_disallowCallbacks)
                return;
        }

        public void OnRequiredExclusivePerkDropdownChanged(int value)
        {
            if (m_disallowCallbacks)
                return;
        }

        public void OnBuildFileURLFieldChanged(string value)
        {
            if (m_disallowCallbacks)
                return;
        }

        public void OnChangelogFileFieldChanged(string value)
        {
            if (m_disallowCallbacks)
                return;
        }

        public void OnGoogleDriveLinkToggleChanged(bool value)
        {
            if (m_disallowCallbacks)
                return;
        }
    }
}
