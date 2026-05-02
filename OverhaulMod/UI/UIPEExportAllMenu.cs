using ICSharpCode.SharpZipLib.Zip;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.IO;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPEExportAllMenu : OverhaulUIBehaviour
    {
        [ModSetting(ModSettingIDs.PERSONALIZATION_ITEMS_EXPORT_PATH, null)]
        public static string ExportFolderPath;

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElement("ExportVersionField")]
        private readonly InputField _exportVersionField;

        [UIElementAction(nameof(OnBumpUpVersionButtonClicked))]
        [UIElement("BumpVersionButton")]
        private readonly Button _bumpUpVersionButton;

        [UIElement("ExportFolderField")]
        private readonly InputField _exportFolderField;

        [UIElementAction(nameof(OnEditExportFolderButtonClicked))]
        [UIElement("EditExportFolderButton")]
        private readonly Button _editExportFolderButton;

        [UIElementAction(nameof(OnExportAllButtonClicked))]
        [UIElement("ExportAllButton")]
        private readonly Button _exportAllButton;

        protected override void OnInitialized()
        {
            _exportVersionField.text = PersonalizationManager.Instance.LocalAssetsVersion.UpdateNumber.ToString();
            _exportFolderField.text = ExportFolderPath;
        }

        private void onExportFolderSelected(string path)
        {
            _exportFolderField.text = path;
            ModSettingsManager.SetStringValue(ModSettingIDs.PERSONALIZATION_ITEMS_EXPORT_PATH, path, true);
        }

        public void OnBumpUpVersionButtonClicked()
        {
            _exportVersionField.text = (PersonalizationManager.Instance.LocalAssetsVersion.UpdateNumber + 1).ToString();
        }

        public void OnEditExportFolderButtonClicked()
        {
            ModUIUtils.FileExplorer(base.transform, true, onExportFolderSelected, null, null, true);
        }

        public void OnExportAllButtonClicked()
        {
            if (!int.TryParse(_exportVersionField.text, out int versionNumber))
            {
                ModUIUtils.MessagePopupOK("Error", "Could not parse text from version input field");
                return;
            }

            if (versionNumber < 0)
            {
                ModUIUtils.MessagePopupOK("Error", "Version number must be greater than zero");
                return;
            }

            string folder;
            if (string.IsNullOrEmpty(ExportFolderPath))
            {
                folder = ModDirectories.SavesFolder;
            }
            else
            {
                folder = ExportFolderPath;
            }

            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
            {
                ModUIUtils.MessagePopupOK("Error", "Export directory not found");
                return;
            }

            FastZip fastZip = new FastZip();
            fastZip.CreateZip(Path.Combine(folder, PersonalizationEditorDataManager.ITEMS_ARCHIVE_FILE), ModDirectories.CustomizationFolder, true, string.Empty);

            PersonalizationManager.Instance.SetLocalAssetsVersion(versionNumber);
            ModJsonUtils.WriteStream(Path.Combine(folder, PersonalizationManager.ASSETS_VERSION_FILE), PersonalizationManager.Instance.LocalAssetsVersion);

            _ = ModFileUtils.OpenFileExplorer(folder);
        }
    }
}
