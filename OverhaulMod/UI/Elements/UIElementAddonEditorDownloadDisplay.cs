using OverhaulMod.Content;
using OverhaulMod.Utils;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementAddonEditorDownloadDisplay : OverhaulUIBehaviour
    {
        [UIElement("AddonReferenceExistIcon", false)]
        private readonly GameObject m_addonReferenceExistIconObject;

        [UIElement("AddonIDField")]
        private readonly InputField m_addonIdField;

        [UIElementAction(nameof(OnRefreshAddonReferenceButtonClicked))]
        [UIElement("RefreshAddonReferenceButton")]
        private readonly Button m_refreshAddonReferenceButton;

        [UIElement("PackageURLField")]
        private readonly InputField m_packageUrlField;

        [UIElement("PackageCompressedFilePathField")]
        private readonly InputField m_packageCompressedFilePathField;

        [UIElement("PackageSizeField")]
        private readonly InputField m_packageSizeField;

        [UIElementAction(nameof(OnCalculatePackageSizeButtonClicked))]
        [UIElement("CalculatePackageSizeButton")]
        private readonly Button m_calculatePackageSizeButton;

        private AddonDownloadInfo m_addonDownloadInfo;

        private AddonDownloadListInfo m_addonDownloadListInfo;

        private UIAddonsDownloadEditor m_addonDownloadEditor;

        public void Initialize(AddonDownloadInfo addonDownloadInfo, AddonDownloadListInfo addonDownloadListInfo)
        {
            base.InitializeElement();
            m_addonDownloadInfo = addonDownloadInfo;
            m_addonDownloadListInfo = addonDownloadListInfo;
            m_addonIdField.text = addonDownloadInfo.UniqueID;
            m_packageUrlField.text = addonDownloadInfo.PackageFileURL;
            m_packageSizeField.text = addonDownloadInfo.PackageFileSize.ToString();
        }

        public void UpdateAddonDownloadInfo()
        {
            m_addonDownloadInfo.UniqueID = m_addonIdField.text;
            m_addonDownloadInfo.PackageFileURL = m_packageUrlField.text;
            m_addonDownloadInfo.PackageFileSize = int.Parse(m_packageSizeField.text);
        }

        private void refreshAddonReferencePresence()
        {
            m_addonReferenceExistIconObject.SetActive(m_addonDownloadInfo.HasAddonInfo());
        }

        public void OnRefreshAddonReferenceButtonClicked()
        {
            m_addonDownloadInfo.Addon = AddonManager.Instance.GetAddonInfo(m_addonIdField.text);
            refreshAddonReferencePresence();
        }

        public void OnCalculatePackageSizeButtonClicked()
        {
            string path = m_packageCompressedFilePathField.text;
            if (!File.Exists(path))
            {
                m_packageSizeField.text = "0";
                return;
            }

            FileInfo fileInfo = new FileInfo(path);
            m_packageSizeField.text = fileInfo.Length.ToString();
        }

        public void OnDeleteButtonClicked()
        {
            ModUIUtils.MessagePopup(true, $"Delete \"{(m_addonDownloadInfo.Addon != null ? m_addonDownloadInfo.GetDisplayName() : m_addonDownloadInfo.UniqueID)}\"?", "so what", 125f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
            {
                m_addonDownloadEditor.RemoveEntry(this);
                m_addonDownloadListInfo.Addons.Remove(m_addonDownloadInfo);
                Destroy(base.gameObject);
            });
        }
    }
}
