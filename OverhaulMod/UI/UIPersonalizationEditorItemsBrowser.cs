using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditorItemsBrowser : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnReloadButtonClicked))]
        [UIElement("ReloadButton")]
        private readonly Button m_reloadButton;

        [UIElementAction(nameof(OnFolderButtonClicked))]
        [UIElement("FolderButton")]
        private readonly Button m_folderButton;

        [UIElementAction(nameof(OnCreateNewButtonClicked))]
        [UIElement("CreateNewButton")]
        private readonly Button m_createNewButton;

        [UIElementAction(nameof(OnFolderButtonClicked))]
        [UIElement("ImportButton")]
        private readonly Button m_importButton;

        [UIElementAction(nameof(OnViewAllItemsToggleChanged))]
        [UIElement("ViewAllItemsToggle")]
        private readonly Toggle m_viewAllItemsToggle;

        [UIElement("ItemDisplayPrefab", false)]
        private readonly ModdedObject m_itemDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform m_container;

        public override void Show()
        {
            base.Show();
            Populate();
        }

        public void Populate()
        {
            bool getAll = m_viewAllItemsToggle.isOn && PersonalizationEditorManager.Instance.canEditNonOwnItems;

            List<PersonalizationItemInfo> list = new List<PersonalizationItemInfo>();
            foreach (PersonalizationItemInfo item in PersonalizationManager.Instance.itemList.Items)
            {
                if (item.CanBeEdited() || getAll)
                {
                    list.Add(item);
                }
            }
            populate(list);
        }

        private void populate(List<PersonalizationItemInfo> list)
        {
            if (m_container.childCount != 0)
                TransformUtils.DestroyAllChildren(m_container);

            if (list.IsNullOrEmpty())
                return;

            foreach (PersonalizationItemInfo item in list)
            {
                ModdedObject moddedObject = Instantiate(m_itemDisplayPrefab, m_container);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = item.Name;
                moddedObject.GetObject<Text>(1).text = item.Category.ToString();

                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    PersonalizationEditorManager.Instance.EditItem(item, item.FolderPath);
                    Hide();
                });
            }
        }

        public void OnViewAllItemsToggleChanged(bool value)
        {
            Populate();
        }

        public void OnReloadButtonClicked()
        {
            PersonalizationManager.Instance.itemList.Load();
            Populate();
        }

        public void OnFolderButtonClicked()
        {
            _ = ModIOUtils.OpenFileExplorer(ModCore.customizationFolder);
        }

        public void OnCreateNewButtonClicked()
        {
            ModUIUtils.InputFieldWindow("Create new item", "Enter folder name", 150f, delegate (string str)
            {
                if (PersonalizationManager.Instance.CreateItem(str, out PersonalizationItemInfo personalizationItem))
                {
                    PersonalizationEditorManager.Instance.EditItem(personalizationItem, personalizationItem.FolderPath);
                    Hide();
                }
                else
                {
                    ModUIUtils.MessagePopupOK("Item creation error", "A folder with the name has been already created.\nTry giving your folder an alternate name.", true);
                }
            });
        }
    }
}
