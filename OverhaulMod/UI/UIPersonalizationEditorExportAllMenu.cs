using ICSharpCode.SharpZipLib.Zip;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.IO;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditorExportAllMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("ExportVersionField")]
        private readonly InputField m_exportVersionField;

        [UIElementAction(nameof(OnBumpUpVersionButtonClicked))]
        [UIElement("BumpVersionButton")]
        private readonly Button m_bumpUpVersionButton;

        [UIElementAction(nameof(OnExportAllButtonClicked))]
        [UIElement("ExportAllButton")]
        private readonly Button m_exportAllButton;

        protected override void OnInitialized()
        {
            m_exportVersionField.text = PersonalizationManager.Instance.localAssetsInfo.AssetVersionNumber.ToString();
        }

        public void OnBumpUpVersionButtonClicked()
        {
            m_exportVersionField.text = (PersonalizationManager.Instance.localAssetsInfo.AssetVersionNumber + 1).ToString();
        }

        public void OnExportAllButtonClicked()
        {
            if (!int.TryParse(m_exportVersionField.text, out int versionNumber))
            {
                ModUIUtils.MessagePopupOK("Error", "Could not parse text from version input field");
                return;
            }

            FastZip fastZip = new FastZip();
            fastZip.CreateZip(Path.Combine(ModCore.savesFolder, "customization.zip"), ModCore.customizationFolder, true, string.Empty);

            PersonalizationAssetsInfo personalizationAssetsInfo = new PersonalizationAssetsInfo
            {
                AssetVersionNumber = versionNumber
            };
            personalizationAssetsInfo.SetAssetVersionForOldBuilds();
            _ = PersonalizationManager.Instance.SetLocalAssetsVersion(versionNumber);
            ModJsonUtils.WriteStream(Path.Combine(ModCore.savesFolder, PersonalizationManager.ASSETS_VERSION_FILE), personalizationAssetsInfo);

            _ = ModFileUtils.OpenFileExplorer(ModCore.savesFolder);
        }
    }
}
