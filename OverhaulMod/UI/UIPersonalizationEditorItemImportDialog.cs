using ICSharpCode.SharpZipLib.Zip;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
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
            Hide();
            string folderName = m_changeFolderNameToggle.isOn ? $"{Path.GetFileName(FilePath).Replace("PersonalizationItem_", string.Empty).Remove(8)}_{m_itemFolderNameField.text.Replace(" ", string.Empty)}" : Path.GetFileNameWithoutExtension(FilePath);
            string folderPath = Path.Combine(ModCore.customizationFolder, folderName);
            _ = Directory.CreateDirectory(folderPath);

            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(FilePath, folderPath, null);

            PersonalizationItemList itemList = PersonalizationManager.Instance.itemList;
            PersonalizationItemInfo info;
            try
            {
                info = itemList.LoadItemInfo(folderPath);
            }
            catch (Exception exc)
            {
                ModUIUtils.MessagePopupOK("Import error", exc.ToString(), true);
                return;
            }

            foreach (PersonalizationEditorObjectInfo child in info.RootObject.Children)
            {
                if (child.Path == "Volume")
                {
                    if (child.PropertyValues.TryGetValue(nameof(PersonalizationEditorObjectVolume.volumeSettingPresets), out object obj) && obj is Dictionary<WeaponVariant, VolumeSettingsPreset> dictionary && !dictionary.IsNullOrEmpty())
                    {
                        foreach (VolumeSettingsPreset value in dictionary.Values)
                        {
                            string voxFilePath = value.VoxFilePath;
                            if (!voxFilePath.IsNullOrEmpty() && !voxFilePath.StartsWith(folderName))
                            {
                                string sub = voxFilePath.Substring(voxFilePath.IndexOf(Path.DirectorySeparatorChar) + 1);
                                voxFilePath = $"{folderName}{Path.DirectorySeparatorChar}{sub}";
                                value.VoxFilePath = voxFilePath;
                            }
                        }
                    }
                }
            }

            void finalAction()
            {
                itemList.Items.Add(info);

                UIPersonalizationEditor.instance.ShowEverything();
                PersonalizationEditorManager.Instance.EditItem(info, info.FolderPath);

                Hide();
                ItemBrowser.Hide();
            }

            PersonalizationItemInfo existingItem = itemList.GetItem(info.ItemID);
            if (existingItem != null)
            {
                ModUIUtils.MessagePopup(true, "An item with the same ID has been already imported!", "Do you want to replace the old version with the new one?", 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "Ok", "Yes", "No", null, delegate
                {
                    if (Path.GetFullPath(existingItem.FolderPath) == Path.GetFullPath(info.FolderPath))
                    {
                        ModUIUtils.MessagePopupOK("Both items have been in the same folder", "Just a notification");
                    }
                    else
                    {
                        Directory.Delete(existingItem.FolderPath, true);
                    }
                    _ = itemList.Items.Remove(existingItem);
                    finalAction();
                });
            }
            else
            {
                finalAction();
            }
        }
    }
}
