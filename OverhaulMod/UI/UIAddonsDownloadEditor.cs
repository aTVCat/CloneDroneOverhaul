using OverhaulMod.Content;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAddonsDownloadEditor : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button m_saveButton;

        [UIElementAction(nameof(OnCreateNewButtonClicked))]
        [UIElement("CreateNewButton")]
        private readonly Button m_createNewButton;

        [UIElementAction(nameof(OnGetDataFromServersButtonClicked))]
        [UIElement("GetDataFromServersButton")]
        private readonly Button m_getDataFromServersButton;

        [UIElementAction(nameof(OnEditedInfosDropdown))]
        [UIElement("AddonsDropdown")]
        private readonly Dropdown m_infosDropdown;

        [UIElement("AddonNameInputField")]
        private readonly InputField m_contentNameField;
        [UIElement("AddonSizeInputField")]
        private readonly InputField m_contentSizeField;
        [UIElement("AddonFilePathInputField")]
        private readonly InputField m_contentPathField;
        [UIElement("AddonMinModVersionInputField")]
        private readonly InputField m_contentMinModVersionField;
        [UIElement("AddonDescriptionField")]
        private readonly InputField m_contentDescriptionField;
        [UIElement("AddonImagesField")]
        private readonly InputField m_contentImagesField;
        [UIElement("AddonVersionField")]
        private readonly InputField m_contentVersionField;

        [UIElement("FileDisplay", false)]
        private readonly ModdedObject m_fileDisplayPrefab;
        [UIElement("AddFileButton", false)]
        private readonly Button m_addFileButton;

        [UIElement("Content")]
        private readonly Transform m_filesContainer;

        public AddonListInfo editingContentList
        {
            get;
            private set;
        }

        public AddonDownloadInfo editingContentDownloadInfo
        {
            get;
            private set;
        }

        public override bool hideTitleScreen => true;

        public override void Show()
        {
            base.Show();
        }

        private void populateDropdown()
        {
            List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();

            AddonListInfo cl = editingContentList;
            if (cl == null || cl.Addons.IsNullOrEmpty())
                return;

            foreach (AddonDownloadInfo c in cl.Addons)
            {
                list.Add(new Dropdown.OptionData(c.GetDisplayName()));
            }
            m_infosDropdown.options = list;
        }

        private void populateFields()
        {
            AddonDownloadInfo contentDownloadInfo = editingContentDownloadInfo;
            if (contentDownloadInfo == null)
                return;

            m_contentNameField.text = contentDownloadInfo.GetDisplayName();
            m_contentSizeField.text = contentDownloadInfo.PackageFileSize.ToString();
            m_contentPathField.text = contentDownloadInfo.PackageFileURL;
            m_contentMinModVersionField.text = contentDownloadInfo.MinModVersion?.ToString();
            m_contentDescriptionField.text = contentDownloadInfo.GetDescription(LocalizationManager.Instance.GetCurrentLanguageCode());
            m_contentImagesField.text = contentDownloadInfo.Images;
            m_contentVersionField.text = contentDownloadInfo.GetVersion().ToString();
        }

        private void readFileFromDisk()
        {
            ModDataManager manager = ModDataManager.Instance;
            AddonListInfo contentListInfo;
            if (manager.FileExists(AddonManager.ADDONS_LIST_REPOSITORY_FILE, true))
            {
                string rawData = ModDataManager.Instance.ReadFile(AddonManager.ADDONS_LIST_REPOSITORY_FILE, true);
                contentListInfo = ModJsonUtils.Deserialize<AddonListInfo>(rawData);
                contentListInfo.FixValues();
            }
            else
            {
                contentListInfo = new AddonListInfo();
                contentListInfo.FixValues();
            }

            editingContentList = contentListInfo;
            m_getDataFromServersButton.interactable = true;

            populateDropdown();
        }

        public void OnEditedInfosDropdown(int index)
        {
            AddonListInfo cl = editingContentList;
            if (cl == null || cl.Addons.IsNullOrEmpty())
                return;

            editingContentDownloadInfo = cl.Addons[index];
            populateFields();
        }

        public void OnSaveButtonClicked()
        {
            if (!Version.TryParse(m_contentMinModVersionField.text, out Version version))
            {
                ModUIUtils.MessagePopupOK("Could not parse version", "check if version is typed correctly", false);
                return;
            }

            AddonDownloadInfo contentDownloadInfo = editingContentDownloadInfo;
            if (contentDownloadInfo != null)
            {
                /*
                contentDownloadInfo.DisplayName = m_contentNameField.text;
                contentDownloadInfo.PackageFileSize = long.Parse(m_contentSizeField.text);
                contentDownloadInfo.PackageFileURL = m_contentPathField.text;
                contentDownloadInfo.MinModVersion = version;
                contentDownloadInfo.Version = int.Parse(m_contentVersionField.text);
                contentDownloadInfo.Images = m_contentImagesField.text;
                contentDownloadInfo.Description = m_contentDescriptionField.text;*/
            }

            AddonListInfo contentListInfo = editingContentList;
            if (contentListInfo != null)
            {
                ModDataManager.Instance.SerializeToFile(AddonManager.ADDONS_LIST_REPOSITORY_FILE, contentListInfo, true);
            }

            populateDropdown();
        }

        public void OnCreateNewButtonClicked()
        {
            ModUIUtils.InputFieldWindow("Create new content folder", "Name it", null, 30, 125f, delegate (string value)
            {
                AddonListInfo contentListInfo = editingContentList;
                if (contentListInfo != null)
                {
                    AddonDownloadInfo contentDownloadInfo = new AddonDownloadInfo()
                    {
                        /*DisplayName = value,
                        PackageFileSize = 1,
                        Files = new List<string>()*/
                    };
                    contentListInfo.Addons.Add(contentDownloadInfo);

                    editingContentDownloadInfo = contentDownloadInfo;
                    populateDropdown();
                    populateFields();
                }
            });
        }

        public void OnGetDataFromServersButtonClicked()
        {
            m_getDataFromServersButton.interactable = false;

            AddonManager.Instance.DownloadAddonsList(out _, delegate (AddonListInfo contentListInfo)
            {
                editingContentList = contentListInfo;
                m_getDataFromServersButton.interactable = true;

                populateDropdown();
            }, delegate (string error)
            {
                readFileFromDisk();
            });
        }
    }
}
