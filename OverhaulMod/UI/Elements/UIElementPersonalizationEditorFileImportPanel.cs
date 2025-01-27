using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationEditorFileImportPanel : OverhaulUIBehaviour
    {
        public static bool HasShownMagicaVoxelTip;

        [UIElementAction(nameof(OnHelpButtonClicked))]
        [UIElement("HelpButton")]
        private readonly Button m_helpButton;

        [UIElementAction(nameof(OnImportVoxButtonClicked))]
        [UIElement("ImportVoxButton")]
        private readonly Button m_importVoxButton;

        [UIElementAction(nameof(OnImportCvmButtonClicked))]
        [UIElement("ImportCvmButton")]
        private readonly Button m_importCvmButton;

        [UIElementAction(nameof(OnMagicaVoxelTipButtonClicked))]
        [UIElement("MagicaVoxelTipButton")]
        private readonly Button m_magicaVoxelTipButton;

        [UIElement("FileDisplayPrefab", false)]
        private readonly ModdedObject m_fileDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform m_fileDisplayContainer;

        private PersonalizationItemInfo m_itemInfo;
        public PersonalizationItemInfo itemInfo
        {
            get
            {
                return m_itemInfo;
            }
            set
            {
                m_itemInfo = value;
                Populate();
            }
        }

        public void Populate()
        {
            if (m_fileDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(m_fileDisplayContainer);

            foreach (string file in itemInfo.ImportedFiles)
            {
                ModdedObject moddedObject = Instantiate(m_fileDisplayPrefab, m_fileDisplayContainer);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(1).text = file;
                moddedObject.GetObject<Button>(2).onClick.AddListener(delegate
                {
                    ModUIUtils.MessagePopup(true, $"Delete {file}?", LocalizationManager.Instance.GetTranslatedString("action_cannot_be_undone"), 125f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
                    {
                        string path = PersonalizationItemInfo.GetImportedFileFullPath(itemInfo, file);
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        PersonalizationCacheManager.Instance.Remove(path.Replace("/", "\\"));

                        itemInfo.GetImportedFiles();
                        Populate();
                        GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                    });
                });
            }
        }

        private void importFileDialog(string initialFolder, string pattern)
        {
            PersonalizationItemInfo item = itemInfo;
            ModUIUtils.FileExplorer(UIPersonalizationEditor.instance.transform, true, delegate (string path)
            {
                if (!File.Exists(path))
                    return;

                string fn = Path.GetFileName(path);
                string fnNoExtension = Path.GetFileNameWithoutExtension(path);
                if (ModFileUtils.HasUnsupportedCharacters(fnNoExtension))
                {
                    ModUIUtils.MessagePopupOK("This file contains unsupported characters.", "Only Latin, number and a few special characters are supported for technical reasons.", 150f, true);
                    return;
                }

                string dest = Path.Combine(PersonalizationItemInfo.GetImportedFilesFolder(item), fn);
                if (File.Exists(dest))
                {
                    ModUIUtils.MessagePopup(true, "File with the same name is already imported!", "Replace the file?", 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Replace", "No", null, delegate
                    {
                        File.Delete(dest);
                        File.Copy(path, dest);
                        PersonalizationCacheManager.Instance.Remove(dest.Replace("/", "\\"));

                        item.GetImportedFiles();
                        Populate();
                        GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                    });
                    return;
                }
                else
                {
                    File.Copy(path, dest);
                    PersonalizationCacheManager.Instance.Remove(dest.Replace("/", "\\"));
                }

                item.GetImportedFiles();
                Populate();
                GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
            }, initialFolder, pattern);
        }

        private void voxFileImportDialog()
        {
            importFileDialog(InternalModBot.ModsManager.Instance.ModFolderPath, "*.vox");
        }

        public void OnHelpButtonClicked()
        {
            _ = ModUIConstants.ShowPersonalizationEditorHelpMenu(UIPersonalizationEditor.instance.transform);
        }

        public void OnImportVoxButtonClicked()
        {
            if (!HasShownMagicaVoxelTip)
            {
                UIPersonalizationEditorMagicaVoxelTip tip = ModUIConstants.ShowPersonalizationEditorMagicaVoxelTip(UIPersonalizationEditor.instance.transform);
                tip.Callback = voxFileImportDialog;
                return;
            }
            voxFileImportDialog();
        }

        public void OnImportCvmButtonClicked()
        {
            importFileDialog(Path.Combine(Application.persistentDataPath, "CustomModels"), "*.cvm");
        }

        public void OnMagicaVoxelTipButtonClicked()
        {
            UIPersonalizationEditorMagicaVoxelTip tip = ModUIConstants.ShowPersonalizationEditorMagicaVoxelTip(UIPersonalizationEditor.instance.transform);
            tip.Callback = null;
        }
    }
}
