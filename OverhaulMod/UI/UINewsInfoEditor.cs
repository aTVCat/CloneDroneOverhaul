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
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnDataFolderButtonClicked))]
        [UIElement("DataFolderButton")]
        private readonly Button _dataFolderButton;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button _saveButton;

        [UIElementAction(nameof(OnRetrieveButtonClicked))]
        [UIElement("RetrieveDataFromServerButton")]
        private readonly Button _retrieveButton;

        [UIElementAction(nameof(OnCreateNewButtonClicked))]
        [UIElement("CreateNewButton")]
        private readonly Button _createNewButton;

        [UIElementAction(nameof(OnEditedNewsDropdown))]
        [UIElement("NewsDropdown")]
        private readonly Dropdown _newsDropdown;

        [UIElement("HeaderIF")]
        private readonly InputField _headerField;

        [UIElement("DescriptionIF")]
        private readonly InputField _descriptionField;

        [UIElement("SurveyIF")]
        private readonly InputField _surveyField;

        private NewsInfo _editingInfo;
        private NewsInfoList _editingInfoList;

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
            if (_editingInfoList == null)
                return;

            List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
            foreach (NewsInfo news in _editingInfoList.News)
            {
                list.Add(new Dropdown.OptionData() { text = news.Title });
            }
            _newsDropdown.options = list;
        }

        public void OnDataFolderButtonClicked()
        {
            _ = ModFileUtils.OpenFileExplorer(ModCore.SavesFolder);
        }

        public void OnSaveButtonClicked()
        {
            if (_editingInfo == null || _editingInfoList == null)
                return;

            _editingInfo.Title = _headerField.text;
            _editingInfo.Description = _descriptionField.text;
            _editingInfo.Survey = _surveyField.text;

            ModDataManager.Instance.SerializeToFile(NewsManager.REPOSITORY_FILE, _editingInfoList, true);
        }

        public void OnRetrieveButtonClicked()
        {
            _retrieveButton.interactable = false;
            NewsManager.Instance.DownloadNewsInfoFile(delegate (NewsInfoList newsInfoList)
            {
                _retrieveButton.interactable = true;
                _editingInfoList = newsInfoList;
                RefreshDropdown();
            }, delegate (string error)
            {
                _retrieveButton.interactable = true;
                ModUIUtils.MessagePopupOK("Error", error);
            });
        }

        public void OnCreateNewButtonClicked()
        {
            if (_editingInfoList == null)
                return;

            NewsInfo newsInfo = new NewsInfo()
            {
                Title = "Some news",
            };
            _editingInfoList.News.Insert(0, newsInfo);
            RefreshDropdown();
            OnEditedNewsDropdown(0);
        }

        public void OnEditedNewsDropdown(int value)
        {
            if (_editingInfoList == null)
                return;

            _editingInfo = _editingInfoList.News[value];
            _headerField.text = _editingInfo.Title;
            _descriptionField.text = _editingInfo.Description;
            _surveyField.text = _editingInfo.Survey;
            _newsDropdown.value = value;
        }
    }
}
