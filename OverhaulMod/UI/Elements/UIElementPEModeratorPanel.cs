using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPEModeratorPanel : OverhaulUIBehaviour
    {
        [UIElement("NewEditorIdField")]
        private readonly InputField _editorIdField;

        [UIElement("NewItemIdField")]
        private readonly InputField _itemIdField;

        [UIElement("VersionField")]
        private readonly InputField _versionField;

        [UIElement("VerifiedToggle")]
        private readonly Toggle _verifiedToggle;

        [UIElement("SentToVerificationToggle")]
        private readonly Toggle _sentToVerificationToggle;

        [UIElement("ReuploadedToggle")]
        private readonly Toggle _reuploadedToggle;

        [UIElement("ExportedFileNameText")]
        private readonly Text _exportedFileNameText;

        [UIElementAction(nameof(OnExportButtonClicked))]
        [UIElement("ExportButton")]
        private readonly Button _exportButton;

        [UIElementAction(nameof(OnSavesFolderButtonClicked))]
        [UIElement("SavesFolderButton")]
        private readonly Button _savesFolderButton;

        [UIElementAction(nameof(OnItemFolderFolderButtonClicked))]
        [UIElement("ItemFolderButton")]
        private readonly Button _itemFolderButton;

        [UIElementAction(nameof(OnRevealEditorIDButtonClicked))]
        [UIElement("RevealEditorIDButton")]
        private readonly Button _revealEditorIDButton;

        private bool _disallowCallbacks;

        public PersonalizationItemInfo EditingItemInfo
        {
            get => PersonalizationEditorManager.Instance.EditingItemInfo;
        }

        protected override void OnInitialized()
        {
        }

        public void Populate()
        {
            PersonalizationItemInfo itemInfo = EditingItemInfo;
            if (itemInfo == null) return;

            _disallowCallbacks = true;

            _editorIdField.text = itemInfo.EditorID;
            _itemIdField.text = itemInfo.ItemID;
            _versionField.text = itemInfo.Version.ToString();
            _sentToVerificationToggle.isOn = itemInfo.IsSentForVerification;
            _verifiedToggle.isOn = itemInfo.IsVerified;
            _reuploadedToggle.isOn = itemInfo.ReuploadedTheItem;
            _exportedFileNameText.text = $"Will be exported as:\n{PersonalizationEditorDataManager.Instance.GetExportedItemFileName(itemInfo)}";

            _editorIdField.interactable = false;
            _revealEditorIDButton.gameObject.SetActive(true);

            _disallowCallbacks = false;
        }

        public void ApplyValues()
        {
            PersonalizationItemInfo itemInfo = EditingItemInfo;
            if (itemInfo == null) return;

            itemInfo.IsVerified = _verifiedToggle.isOn;
            if (itemInfo.IsVerified)
            {
                itemInfo.IsSentForVerification = false;
                itemInfo.ReuploadedTheItem = false;
                _sentToVerificationToggle.isOn = false;
                _reuploadedToggle.isOn = false;
            }

            int prevVersion = itemInfo.Version;
            if (!int.TryParse(_versionField.text, out int version))
                version = prevVersion;

            itemInfo.Version = version;
        }

        public void OnExportButtonClicked()
        {
            PersonalizationItemSaveResult result = PersonalizationEditorManager.Instance.SaveItem();
            if (result.HasFailed())
            {
                ModUIUtils.MessagePopupOK("Error", result.Error, true);
            }
            else
            {
                PersonalizationItemInfo itemInfo = PersonalizationEditorManager.Instance.EditingItemInfo;
                PersonalizationEditorDataManager.Instance.ExportItem(itemInfo, out _, ModDirectories.SavesFolder, PersonalizationEditorDataManager.Instance.GetExportedItemFileName(itemInfo));
                ModUIUtils.MessagePopupOK("Exported the item", "for real", false);
            }
        }

        public void OnSavesFolderButtonClicked()
        {
            ModFileUtils.OpenFileExplorer(ModDirectories.SavesFolder);
        }

        public void OnItemFolderFolderButtonClicked()
        {
            ModFileUtils.OpenFileExplorer(PersonalizationEditorManager.Instance.EditingItemInfo.FolderPath);
        }

        public void OnRevealEditorIDButtonClicked()
        {
            _editorIdField.interactable = true;
            _revealEditorIDButton.gameObject.SetActive(false);
        }
    }
}