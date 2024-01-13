using OverhaulMod.Content;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UINewsInfoEditor : OverhaulUIBehaviour
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

        [UIElementAction(nameof(OnCreateNewButtonClicked))]
        [UIElement("CreateNewButton")]
        private readonly Button m_createNewButton;

        [UIElementAction(nameof(OnEditedNewsDropdown))]
        [UIElement("NewsDropdown")]
        private readonly Dropdown m_newsDropdown;

        [UIElement("HeaderIF")]
        private readonly InputField m_headerField;

        [UIElement("DescriptionIF")]
        private readonly InputField m_descriptionField;

        private NewsInfo m_editingInfo;
        private NewsInfoList m_editingInfoList;

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void RefreshDropdown()
        {
            if (m_editingInfoList == null)
                return;

            List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
            foreach (NewsInfo news in m_editingInfoList.News)
            {
                list.Add(new Dropdown.OptionData() { text = news.Title });
            }
            m_newsDropdown.options = list;
        }

        public void OnDataFolderButtonClicked()
        {
            _ = ModIOUtils.OpenFileExplorer(ModCore.savesFolder);
        }

        public void OnSaveButtonClicked()
        {
            if (m_editingInfo == null || m_editingInfoList == null)
                return;

            m_editingInfo.Title = m_headerField.text;
            m_editingInfo.Description = m_descriptionField.text;

            ModDataManager.Instance.WriteFile(NewsManager.REPOSITORY_FILE, ModJsonUtils.Serialize(m_editingInfoList), true);
        }

        public void OnRetrieveButtonClicked()
        {
            m_retrieveButton.interactable = false;
            NewsManager.Instance.DownloadNewsInfoFile(delegate (NewsInfoList newsInfoList)
            {
                m_retrieveButton.interactable = true;
                m_editingInfoList = newsInfoList;
                RefreshDropdown();
            }, delegate (string error)
            {
                m_retrieveButton.interactable = true;
                ModUIUtils.MessagePopupOK("Error", error);
            });
        }

        public void OnCreateNewButtonClicked()
        {
            if (m_editingInfoList == null)
                return;

            NewsInfo newsInfo = new NewsInfo()
            {
                Title = "Some news",
            };
            m_editingInfoList.News.Add(newsInfo);
            RefreshDropdown();
            OnEditedNewsDropdown(m_editingInfoList.News.Count - 1);
        }

        public void OnEditedNewsDropdown(int value)
        {
            if (m_editingInfoList == null)
                return;

            m_editingInfo = m_editingInfoList.News[value];
            m_headerField.text = m_editingInfo.Title;
            m_descriptionField.text = m_editingInfo.Description;
            m_newsDropdown.value = value;
        }
    }
}
