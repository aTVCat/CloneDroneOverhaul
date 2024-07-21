using OverhaulMod.Content;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIExclusiveContentEditor : OverhaulUIBehaviour
    {
        private List<UIElementContentCustomPropertyDisplay> m_instantiatedPropertyDisplays;

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnDataFolderButtonClicked))]
        [UIElement("DataFolderButton")]
        private readonly Button m_dataFolderButton;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button m_saveButton;

        [UIElementAction(nameof(OnNewButtonClicked))]
        [UIElement("NewButton")]
        private readonly Button m_newButton;

        [UIElement("NameInputField")]
        private readonly InputField m_contentNameInputField;

        [UIElement("SteamIDInputField")]
        private readonly InputField m_steamIdInputField;

        [UIElement("PlayFabIDInputField")]
        private readonly InputField m_playFabIdInputField;

        [UIElementAction(nameof(EditContentInfo))]
        [UIElement("ContentDropdown")]
        private readonly Dropdown m_contentDropdown;

        [UIElement("CreateContentPanel", false)]
        private readonly GameObject m_createContentPanel;

        [UIElementAction(nameof(HideCreateContentPanel))]
        [UIElement("CreateContentCloseButton")]
        private readonly Button m_createContentPanelExitButton;

        [UIElementAction(nameof(OnCreateButtonClicked))]
        [UIElement("CreateNewButton")]
        private readonly Button m_createContentPanelCreateButton;

        [UIElement("TypeDropdown")]
        private readonly Dropdown m_contentTypeDropdown;

        [UIElement("ColorFieldPrefab", false)]
        private readonly ModdedObject m_colorFieldPrefab;

        [UIElement("InputFieldPrefab", false)]
        private readonly ModdedObject m_inputFieldPrefab;

        [UIElement("DropdownPrefab", false)]
        private readonly ModdedObject m_dropdownPrefab;

        [UIElement("Content")]
        private readonly Transform m_container;

        protected override void OnInitialized()
        {
            m_instantiatedPropertyDisplays = new List<UIElementContentCustomPropertyDisplay>();

            List<Dropdown.OptionData> contentTypeOptions = new List<Dropdown.OptionData>();
            foreach (string str in typeof(ExclusiveContentType).GetEnumNames())
            {
                contentTypeOptions.Add(new Dropdown.OptionData() { text = str });
            }
            m_contentTypeDropdown.options = contentTypeOptions;
        }

        public override void Show()
        {
            base.Show();

            if (ExclusiveContentEditor.contentList?.List == null)
                return;

            PopulateDropdown();
            if (ExclusiveContentEditor.contentList.List.Count != 0)
                EditContentInfo(0);
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void ShowCreateContentPanel()
        {
            m_createContentPanel.SetActive(true);
        }

        public void HideCreateContentPanel()
        {
            m_createContentPanel.SetActive(false);
        }

        public void EditContentInfo(int index)
        {
            ExclusiveContentEditor.EditInfo(index);
            Populate();
        }

        public void PopulateDropdown(bool setLastIndex = false)
        {
            List<Dropdown.OptionData> contentEntryOptions = new List<Dropdown.OptionData>();
            foreach (ExclusiveContentInfo entry in ExclusiveContentEditor.contentList.List)
            {
                contentEntryOptions.Add(new Dropdown.OptionData() { text = entry.Name });
            }
            m_contentDropdown.options = contentEntryOptions;
            if (setLastIndex && contentEntryOptions.Count != 0)
                m_contentDropdown.value = contentEntryOptions.Count - 1;
        }

        public void Populate()
        {
            ExclusiveContentInfo contentInfo = ExclusiveContentEditor.editingContentInfo;
            if (contentInfo == null)
                return;

            m_contentNameInputField.text = contentInfo.Name;
            m_steamIdInputField.text = contentInfo.SteamID.ToString();
            m_playFabIdInputField.text = contentInfo.PlayFabID;

            m_instantiatedPropertyDisplays.Clear();
            if (m_container.childCount != 0)
                TransformUtils.DestroyAllChildren(m_container);

            foreach (System.Reflection.FieldInfo field in ExclusiveContentEditor.GetContentFields())
            {
                Type type = field.FieldType;
                UIElementContentCustomPropertyDisplay customPropertyDisplay = null;
                if (type == typeof(Color))
                {
                    ModdedObject moddedObject = Instantiate(m_colorFieldPrefab, m_container);
                    customPropertyDisplay = moddedObject.gameObject.AddComponent<UIElementContentColorPropertyDisplay>();
                }
                else if (type == typeof(int))
                {
                    ModdedObject moddedObject = Instantiate(m_inputFieldPrefab, m_container);
                    customPropertyDisplay = moddedObject.gameObject.AddComponent<UIElementContentIntPropertyDisplay>();
                }
                else if (type.IsEnum)
                {
                    ModdedObject moddedObject = Instantiate(m_dropdownPrefab, m_container);
                    customPropertyDisplay = moddedObject.gameObject.AddComponent<UIElementContentEnumPropertyDisplay>();
                }

                if (customPropertyDisplay)
                {
                    customPropertyDisplay.gameObject.SetActive(true);
                    customPropertyDisplay.Populate(field, contentInfo.Content);
                    customPropertyDisplay.InitializeElement();
                    m_instantiatedPropertyDisplays.Add(customPropertyDisplay);
                }
            }
        }

        public void OnDataFolderButtonClicked()
        {
            _ = ModIOUtils.OpenFileExplorer(ModCache.dataRepository.GetRootDataPath(false));
        }

        public void OnSaveButtonClicked()
        {
            ExclusiveContentInfo contentInfo = ExclusiveContentEditor.editingContentInfo;
            contentInfo.Name = m_contentNameInputField.text;
            contentInfo.SteamID = ModParseUtils.TryParseToULong(m_steamIdInputField.text, 0);
            contentInfo.PlayFabID = m_playFabIdInputField.text;

            foreach (UIElementContentCustomPropertyDisplay display in m_instantiatedPropertyDisplays)
            {
                object obj = display.contentReference;
                object value = display.GetValue();
                System.Reflection.FieldInfo fieldRef = display.fieldReference;
                fieldRef.SetValue(obj, value);
            }

            ExclusiveContentEditor.Save();

            ModUIUtils.MessagePopupOK("Content file saved!", string.Empty);
        }

        public void OnNewButtonClicked()
        {
            ShowCreateContentPanel();
        }

        public void OnCreateButtonClicked()
        {
            ExclusiveContentType exclusiveContentType = (ExclusiveContentType)(m_contentTypeDropdown.value + 1);
            ExclusiveContentEditor.CreateInfo(exclusiveContentType, 0, string.Empty);
            ExclusiveContentEditor.EditInfo(ExclusiveContentEditor.contentList.List.Count - 1);
            HideCreateContentPanel();
            Populate();
            PopulateDropdown(true);
        }
    }
}
