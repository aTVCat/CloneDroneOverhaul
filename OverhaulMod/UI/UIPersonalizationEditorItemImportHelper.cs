using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditorItemImportHelper : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnAddItemClicked))]
        [UIElement("AddItemButton")]
        private readonly Button m_addItemButton;

        [UIElementAction(nameof(OnStartVerifyingButtonClicked))]
        [UIElement("StartVerifyingButton")]
        private readonly Button m_startVerifyingButton;

        [UIElement("ItemDisplayPrefab", false)]
        private readonly ModdedObject m_itemDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform m_content;

        [UIElement("Shading", true)]
        private readonly GameObject m_shading;

        [UIElement("ImportPanel", true)]
        private readonly GameObject m_importPanel;

        [UIElement("TaskPanel", false)]
        private readonly GameObject m_taskPanel;

        [UIElement("ProgressFill")]
        private readonly Image m_progressFill;

        [UIElement("ProgressText")]
        private readonly Text m_progressText;

        [UIElementAction(nameof(OnDeclineButtonClicked))]
        [UIElement("DeclineButton")]
        private readonly Button m_declineButton;

        [UIElementAction(nameof(OnVerifyButtonClicked))]
        [UIElement("VerifyButton")]
        private readonly Button m_verifyButton;

        [UIElement("DeleteCheckedItemFilesToggle")]
        private readonly Toggle m_deleteCheckedFiles;

        private List<string> m_files;

        private int m_currentItemIndex;

        private bool m_isExecutingTasks;

        public override bool closeOnEscapeButtonPress => !m_isExecutingTasks;

        protected override void OnInitialized()
        {
            m_files = new List<string>();
        }

        public override void Show()
        {
            base.Show();

            m_files.Clear();
            populate();

            ShowImportPanel();

            UIDownloadPersonalizationAssetsMenu menu = ModUIConstants.ShowDownloadPersonalizationAssetsMenu(base.transform);
            menu.OnRefreshButtonClicked();
        }

        public void ShowImportPanel()
        {
            m_shading.SetActive(true);
            m_importPanel.SetActive(true);
            m_taskPanel.SetActive(false);
            m_isExecutingTasks = false;
        }

        public void ShowTaskPanel()
        {
            m_shading.SetActive(false);
            m_importPanel.SetActive(false);
            m_taskPanel.SetActive(true);
            m_isExecutingTasks = true;
        }

        private void refreshTaskProgressBar()
        {
            m_progressFill.fillAmount = m_currentItemIndex / (float)m_files.Count;
            m_progressText.text = $"{m_currentItemIndex + 1}/{m_files.Count}";
        }

        private void continueOrEndVerifyingItems()
        {
            if (m_deleteCheckedFiles.isOn)
            {
                string path = m_files[m_currentItemIndex];
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            m_currentItemIndex++;
            if (m_currentItemIndex >= m_files.Count)
            {
                Hide();
                ModUIUtils.MessagePopup(true, "All items verfied!", "Would you like to export all items?", 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "Ok", "Yes", "No", null, exportAllItems);
                return;
            }

            refreshTaskProgressBar();
            importAndEditCurrentItem();
        }

        private void importAndEditCurrentItem()
        {
            PersonalizationEditorManager.Instance.ImportItem(m_files[m_currentItemIndex], out string error, true, null);
            if (!string.IsNullOrEmpty(error))
            {
                ModUIUtils.MessagePopupOK("Import error", error, true);
                return;
            }
        }

        private void exportAllItems()
        {
            Hide();
            ModUIConstants.ShowPersonalizationEditorExportAllMenu(UIPersonalizationEditor.instance.transform);
        }

        private void onSelectedFiles(List<string> files)
        {
            if (files == null || files.Count == 0) return;

            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];
                if (!m_files.Contains(file))
                {
                    m_files.Add(file);
                }
            }

            populate();
        }

        private void populate()
        {
            if (m_content.childCount != 0)
                TransformUtils.DestroyAllChildren(m_content);

            List<string> list = m_files;
            if (list == null || list.Count == 0)
            {
                m_startVerifyingButton.interactable = false;
                return;
            }

            for (int i = 0; i < list.Count; i++)
            {
                int index = i;

                string file = list[i];
                ModdedObject display = Instantiate(m_itemDisplayPrefab, m_content);
                display.gameObject.SetActive(true);
                display.GetObject<Text>(2).text = $"{i + 1}.";
                display.GetObject<Text>(0).text = Path.GetFileName(file);
                display.GetObject<Button>(1).onClick.AddListener(delegate
                {
                    m_files.RemoveAt(index);
                    populate();
                });
            }
            m_startVerifyingButton.interactable = true;
        }

        public void OnAddItemClicked()
        {
            ModUIUtils.FileExplorer(base.transform, true, onSelectedFiles, null, "*.zip");
        }

        public void OnStartVerifyingButtonClicked()
        {
            ShowTaskPanel();

            m_currentItemIndex = 0;
            refreshTaskProgressBar();
            importAndEditCurrentItem();
        }

        public void OnDeclineButtonClicked()
        {
            PersonalizationItemInfo info = PersonalizationEditorManager.Instance.currentEditingItemInfo;
            info.IsVerified = false;
            info.IsSentForVerification = false;
            info.ReuploadedTheItem = false;

            PersonalizationEditorManager.Instance.SaveItem(out string error, true);
            if (!string.IsNullOrEmpty(error))
            {
                UIPersonalizationEditor.instance.ShowSaveErrorMessage(error);
                return;
            }

            continueOrEndVerifyingItems();
        }

        public void OnVerifyButtonClicked()
        {
            PersonalizationItemInfo info = PersonalizationEditorManager.Instance.currentEditingItemInfo;
            info.IsVerified = true;
            info.IsSentForVerification = false;
            info.ReuploadedTheItem = false;

            PersonalizationEditorManager.Instance.SaveItem(out string error, true);
            if (!string.IsNullOrEmpty(error))
            {
                UIPersonalizationEditor.instance.ShowSaveErrorMessage(error);
                return;
            }

            continueOrEndVerifyingItems();
        }
    }
}
