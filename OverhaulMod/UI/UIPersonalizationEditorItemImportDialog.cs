using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditorItemImportDialog : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnDoneButtonClicked))]
        [UIElement("DoneButton")]
        private readonly Button _doneButton;

        [UIElementAction(nameof(OnChangeFolderNameToggle))]
        [UIElement("ChangeFolderNameToggle")]
        private readonly Toggle _changeFolderNameToggle;

        [UIElementAction(nameof(OnItemFolderNameChanged))]
        [UIElement("ItemFolderNameField")]
        private readonly InputField _itemFolderNameField;

        public UIPersonalizationEditorItemBrowser ItemBrowser;

        public string FilePath;

        protected override void OnInitialized()
        {
            _changeFolderNameToggle.isOn = true;
            _itemFolderNameField.text = string.Empty;
            RefreshDoneButton();
        }

        public override void Show()
        {
            base.Show();
        }

        public void RefreshDoneButton()
        {
            _doneButton.interactable = !_changeFolderNameToggle.isOn || (!_itemFolderNameField.text.IsNullOrEmpty() && !_itemFolderNameField.text.IsNullOrWhiteSpace());
        }

        private void importResult(PersonalizationItemImportResult result)
        {
            if (result.HasFailed())
            {
                ModUIUtils.MessagePopupOK("Import error", result.Error, true);
                return;
            }
            Hide();
            ItemBrowser.Hide();
        }

        public void OnChangeFolderNameToggle(bool value)
        {
            _itemFolderNameField.interactable = value;
            RefreshDoneButton();
        }

        public void OnItemFolderNameChanged(string value)
        {
            RefreshDoneButton();
        }

        public void OnDoneButtonClicked()
        {
            PersonalizationEditorDataManager.Instance.ImportOrUpdateItem(FilePath, importResult, true);
        }
    }
}
