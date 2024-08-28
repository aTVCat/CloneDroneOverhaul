using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UILevelDescriptionListEditor : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnDataFolderButtonClicked))]
        [UIElement("DataFolderButton")]
        private readonly Button m_dataFolderButton;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button m_saveButton;

        [UIElementAction(nameof(OnCreateNewButtonClicked))]
        [UIElement("CreateNewButton")]
        private readonly Button m_createNewButton;

        [UIElementAction(nameof(OnCreateNewFileButtonClicked))]
        [UIElement("CreateNewFileButton")]
        private readonly Button m_createNewFileButton;

        [UIElement("EndlessLevelsText")]
        private readonly Text m_endlessLevelIDsText;

        [UIElementAction(nameof(OnLevelsDropdownEdited))]
        [UIElement("LevelsDropdown")]
        private readonly Dropdown m_levelsDropdown;

        [UIElement("LevelPath")]
        private readonly InputField m_levelPathInputField;

        [UIElement("LevelID")]
        private readonly InputField m_levelIdField;

        [UIElement("DifficultyIndex")]
        private readonly InputField m_difficultyIndexInputField;

        [UIElement("DropdownPrefab")]
        private readonly Dropdown m_difficultyDropdown;

        private LevelDescription m_editingLevelDescription;

        public override bool hideTitleScreen => true;

        protected override void OnInitialized()
        {
            List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
            foreach (string name in typeof(DifficultyTier).GetEnumNames())
                list.Add(new Dropdown.OptionData(name));

            list.Add(new Dropdown.OptionData("Nightmarium"));

            m_difficultyDropdown.options = list;
            RefreshDisplays();
            OnLevelsDropdownEdited(0);
        }

        public void RefreshDisplays()
        {
            ModLevelManager modLevelManager = ModLevelManager.Instance;
            if (!modLevelManager || modLevelManager.modLevelDescriptionsLoadError != null || modLevelManager.modLevelDescriptions.LevelDescriptions.IsNullOrEmpty())
                return;

            List<Dropdown.OptionData> levelOptions = m_levelsDropdown.options ?? new List<Dropdown.OptionData>();
            levelOptions.Clear();
            foreach (LevelDescription desc in modLevelManager.modLevelDescriptions.LevelDescriptions)
            {
                levelOptions.Add(new Dropdown.OptionData(desc.LevelID));
            }
            m_levelsDropdown.options = levelOptions;

            /*
            StringBuilder stringBuilder = new StringBuilder();
            foreach (LevelDescription endlessLevel in LevelManager.Instance._endlessLevels)
            {
                _ = stringBuilder.Append(endlessLevel.LevelID);
                _ = stringBuilder.Append("\n");
            }
            m_endlessLevelIDsText.text = stringBuilder.ToString();
            _ = stringBuilder.Clear();*/
        }

        public void Populate(LevelDescription levelDescription)
        {
            m_editingLevelDescription = levelDescription;

            m_levelPathInputField.text = levelDescription.LevelJSONPath;
            m_levelIdField.text = levelDescription.LevelID;
            m_difficultyDropdown.value = (int)levelDescription.DifficultyTier;
            m_difficultyIndexInputField.text = levelDescription.LevelEditorDifficultyIndex.ToString();
        }

        public void OnDataFolderButtonClicked()
        {
            _ = ModFileUtils.OpenFileExplorer(ModLevelManager.Instance.levelsFolder);
        }

        public void OnSaveButtonClicked()
        {
            LevelDescription levelDescription = m_editingLevelDescription;
            if (levelDescription == null)
            {
                ModUIUtils.MessagePopupOK("You're not editing any level description", "yes");
                return;
            }

            levelDescription.LevelJSONPath = m_levelPathInputField.text;
            levelDescription.LevelID = m_levelIdField.text;
            levelDescription.DifficultyTier = (DifficultyTier)m_difficultyDropdown.value;
            levelDescription.LevelEditorDifficultyIndex = ModParseUtils.TryParseToInt(m_difficultyIndexInputField.text, 0);

            ModFileUtils.WriteText(ModJsonUtils.Serialize(ModLevelManager.Instance.modLevelDescriptions), Path.Combine(ModLevelManager.Instance.levelsFolder, ModLevelManager.LEVEL_DESCRIPTIONS_FILE));
            RefreshDisplays();
        }

        public void OnCreateNewButtonClicked()
        {
            ModLevelManager modLevelManager = ModLevelManager.Instance;
            if (!modLevelManager || modLevelManager.modLevelDescriptionsLoadError != null || modLevelManager.modLevelDescriptions.LevelDescriptions.IsNullOrEmpty())
                return;

            List<LevelDescription> list = modLevelManager.modLevelDescriptions.LevelDescriptions;
            int index = list.Count;
            list.Add(new LevelDescription());
            RefreshDisplays();
            m_levelsDropdown.value = index;
        }

        public void OnCreateNewFileButtonClicked()
        {
            ModUIUtils.MessagePopup(true, "Create new file?", "ig you're not supposed to do this", 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
            {
                StreamWriter streamWriter = File.CreateText(Path.Combine(ModLevelManager.Instance.levelsFolder, ModLevelManager.LEVEL_DESCRIPTIONS_FILE));
                streamWriter.Close();
                ModLevelManager.Instance.LoadLevelDescriptions();
                RefreshDisplays();
            });
        }

        public void OnLevelsDropdownEdited(int value)
        {
            if (m_editingLevelDescription != null)
            {
                OnSaveButtonClicked();
            }

            ModLevelManager modLevelManager = ModLevelManager.Instance;
            if (!modLevelManager || modLevelManager.modLevelDescriptionsLoadError != null || modLevelManager.modLevelDescriptions.LevelDescriptions.IsNullOrEmpty())
                return;

            m_editingLevelDescription = modLevelManager.modLevelDescriptions.LevelDescriptions[value];
            Populate(m_editingLevelDescription);
        }
    }
}
