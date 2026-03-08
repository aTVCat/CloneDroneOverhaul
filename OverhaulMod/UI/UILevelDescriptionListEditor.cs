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
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnDataFolderButtonClicked))]
        [UIElement("DataFolderButton")]
        private readonly Button _dataFolderButton;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button _saveButton;

        [UIElementAction(nameof(OnCreateNewButtonClicked))]
        [UIElement("CreateNewButton")]
        private readonly Button _createNewButton;

        [UIElementAction(nameof(OnCreateNewFileButtonClicked))]
        [UIElement("CreateNewFileButton")]
        private readonly Button _createNewFileButton;

        [UIElement("EndlessLevelsText")]
        private readonly Text _endlessLevelIDsText;

        [UIElementAction(nameof(OnLevelsDropdownEdited))]
        [UIElement("LevelsDropdown")]
        private readonly Dropdown _levelsDropdown;

        [UIElement("LevelPath")]
        private readonly InputField _levelPathInputField;

        [UIElement("LevelID")]
        private readonly InputField _levelIdField;

        [UIElement("DifficultyIndex")]
        private readonly InputField _difficultyIndexInputField;

        [UIElement("DropdownPrefab")]
        private readonly Dropdown _difficultyDropdown;

        private LevelDescription _editingLevelDescription;

        public override bool hideTitleScreen => true;

        protected override void OnInitialized()
        {
            List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
            foreach (string name in typeof(DifficultyTier).GetEnumNames())
                list.Add(new Dropdown.OptionData(name));

            list.Add(new Dropdown.OptionData("Nightmarium"));

            _difficultyDropdown.options = list;
            RefreshDisplays();
            OnLevelsDropdownEdited(0);
        }

        public void RefreshDisplays()
        {
            ModLevelManager modLevelManager = ModLevelManager.Instance;
            if (!modLevelManager || modLevelManager.modLevelDescriptionsLoadError != null || modLevelManager.modLevelDescriptions.LevelDescriptions.IsNullOrEmpty())
                return;

            List<Dropdown.OptionData> levelOptions = _levelsDropdown.options ?? new List<Dropdown.OptionData>();
            levelOptions.Clear();
            foreach (LevelDescription desc in modLevelManager.modLevelDescriptions.LevelDescriptions)
            {
                levelOptions.Add(new Dropdown.OptionData(desc.LevelID));
            }
            _levelsDropdown.options = levelOptions;

            /*
            StringBuilder stringBuilder = new StringBuilder();
            foreach (LevelDescription endlessLevel in LevelManager.Instance._endlessLevels)
            {
                _ = stringBuilder.Append(endlessLevel.LevelID);
                _ = stringBuilder.Append("\n");
            }
            _endlessLevelIDsText.text = stringBuilder.ToString();
            _ = stringBuilder.Clear();*/
        }

        public void Populate(LevelDescription levelDescription)
        {
            _editingLevelDescription = levelDescription;

            _levelPathInputField.text = levelDescription.LevelJSONPath;
            _levelIdField.text = levelDescription.LevelID;
            _difficultyDropdown.value = (int)levelDescription.DifficultyTier;
            _difficultyIndexInputField.text = levelDescription.LevelEditorDifficultyIndex.ToString();
        }

        public void OnDataFolderButtonClicked()
        {
            _ = ModFileUtils.OpenFileExplorer(ModLevelManager.Instance.levelsFolder);
        }

        public void OnSaveButtonClicked()
        {
            LevelDescription levelDescription = _editingLevelDescription;
            if (levelDescription == null)
            {
                ModUIUtils.MessagePopupOK("You're not editing any level description", "yes");
                return;
            }

            levelDescription.LevelJSONPath = _levelPathInputField.text;
            levelDescription.LevelID = _levelIdField.text;
            levelDescription.DifficultyTier = (DifficultyTier)_difficultyDropdown.value;
            levelDescription.LevelEditorDifficultyIndex = ModParseUtils.TryParseToInt(_difficultyIndexInputField.text, 0);

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
            _levelsDropdown.value = index;
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
            if (_editingLevelDescription != null)
            {
                OnSaveButtonClicked();
            }

            ModLevelManager modLevelManager = ModLevelManager.Instance;
            if (!modLevelManager || modLevelManager.modLevelDescriptionsLoadError != null || modLevelManager.modLevelDescriptions.LevelDescriptions.IsNullOrEmpty())
                return;

            _editingLevelDescription = modLevelManager.modLevelDescriptions.LevelDescriptions[value];
            Populate(_editingLevelDescription);
        }
    }
}
