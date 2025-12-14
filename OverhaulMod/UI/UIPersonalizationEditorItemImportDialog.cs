using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.IO;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditorItemImportDialog : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnDoneButtonClicked))]
        [UIElement("DoneButton")]
        private readonly Button m_doneButton;

        [UIElementAction(nameof(OnChangeFolderNameToggle))]
        [UIElement("ChangeFolderNameToggle")]
        private readonly Toggle m_changeFolderNameToggle;

        [UIElementAction(nameof(OnItemFolderNameChanged))]
        [UIElement("ItemFolderNameField")]
        private readonly InputField m_itemFolderNameField;

        public UIPersonalizationEditorItemBrowser ItemBrowser;

        public string FilePath;

        protected override void OnInitialized()
        {
            m_changeFolderNameToggle.isOn = true;
            m_itemFolderNameField.text = string.Empty;
            RefreshDoneButton();
        }

        public override void Show()
        {
            base.Show();
        }

        public void RefreshDoneButton()
        {
            m_doneButton.interactable = !m_changeFolderNameToggle.isOn || (!m_itemFolderNameField.text.IsNullOrEmpty() && !m_itemFolderNameField.text.IsNullOrWhiteSpace());
        }

        public void OnChangeFolderNameToggle(bool value)
        {
            m_itemFolderNameField.interactable = value;
            RefreshDoneButton();
        }

        public void OnItemFolderNameChanged(string value)
        {
            RefreshDoneButton();
        }

        public void OnDoneButtonClicked()
        {
            string folderName = m_changeFolderNameToggle.isOn ? $"{Path.GetFileName(FilePath).Replace("PersonalizationItem_", string.Empty).Remove(8)}_{m_itemFolderNameField.text.Replace(" ", string.Empty)}" : null;
            PersonalizationEditorManager.Instance.ImportItem(FilePath, folderName, out string error, true);
            if (!string.IsNullOrEmpty(error))
            {
                ModUIUtils.MessagePopupOK("Import error", error, true);
                return;
            }

            Hide();
            ItemBrowser.Hide();
        }
    }
}
