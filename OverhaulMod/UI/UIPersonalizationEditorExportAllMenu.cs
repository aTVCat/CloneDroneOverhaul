using ICSharpCode.SharpZipLib.Zip;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.IO;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditorExportAllMenu : OverhaulUIBehaviour
    {
        [ModSetting(ModSettingsConstants.PERSONALIZATION_ITEMS_EXPORT_PATH, null)]
        public static string ExportFolderPath;

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("ExportVersionField")]
        private readonly InputField m_exportVersionField;

        [UIElementAction(nameof(OnBumpUpVersionButtonClicked))]
        [UIElement("BumpVersionButton")]
        private readonly Button m_bumpUpVersionButton;

        [UIElement("ExportFolderField")]
        private readonly InputField m_exportFolderField;

        [UIElementAction(nameof(OnEditExportFolderButtonClicked))]
        [UIElement("EditExportFolderButton")]
        private readonly Button m_editExportFolderButton;

        [UIElementAction(nameof(OnExportAllButtonClicked))]
        [UIElement("ExportAllButton")]
        private readonly Button m_exportAllButton;

        protected override void OnInitialized()
        {
            m_exportVersionField.text = PersonalizationManager.Instance.localAssetsInfo.AssetVersionNumber.ToString();
            m_exportFolderField.text = ExportFolderPath;
        }

        private void onExportFolderSelected(string path)
        {
            m_exportFolderField.text = path;
            ModSettingsManager.SetStringValue(ModSettingsConstants.PERSONALIZATION_ITEMS_EXPORT_PATH, path, true);
        }

        public void OnBumpUpVersionButtonClicked()
        {
            m_exportVersionField.text = (PersonalizationManager.Instance.localAssetsInfo.AssetVersionNumber + 1).ToString();
        }

        public void OnEditExportFolderButtonClicked()
        {
            ModUIUtils.FileExplorer(base.transform, true, onExportFolderSelected, null, null, true);
        }

        public void OnExportAllButtonClicked()
        {
            if (!int.TryParse(m_exportVersionField.text, out int versionNumber))
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
                folder = ModCore.savesFolder;
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
            fastZip.CreateZip(Path.Combine(folder, "customization.zip"), ModCore.customizationFolder, true, string.Empty);

            PersonalizationAssetsInfo personalizationAssetsInfo = new PersonalizationAssetsInfo
            {
                AssetVersionNumber = versionNumber
            };
            personalizationAssetsInfo.SetAssetVersionForOldBuilds();
            _ = PersonalizationManager.Instance.SetLocalAssetsVersion(versionNumber);
            ModJsonUtils.WriteStream(Path.Combine(folder, PersonalizationManager.ASSETS_VERSION_FILE), personalizationAssetsInfo);

            _ = ModFileUtils.OpenFileExplorer(folder);
        }
    }
}
