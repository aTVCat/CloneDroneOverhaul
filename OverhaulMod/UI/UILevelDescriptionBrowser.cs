using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UILevelDescriptionBrowser : OverhaulUIBehaviour
    {
        public const string SELECTED_COLOR = "#305EE0";
        public const string DESELECTED_COLOR = "#4C4C4C";

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnDoneButtonClicked))]
        [UIElement("DoneButton")]
        private readonly Button _doneButton;

        [UIElementAction(nameof(OnCancelButtonClicked))]
        [UIElement("CancelButton")]
        private readonly Button _cancelButton;

        [UIElement("LevelDescriptionDisplay", false)]
        private readonly ModdedObject _levelDescriptionDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform _levelDescriptionDisplayContainer;

        [UIElement("LevelsLabel")]
        private readonly Text _levelsLabel;

        [UIElement("DifficultySelectWindow", false)]
        private readonly GameObject _levelDifficultySelectWindow;

        [UIElementAction(nameof(OnDifficultyCloseButtonClicked))]
        [UIElement("DifficultyCloseButton")]
        private readonly Button _difficultyExitButton;

        [UIElementAction(nameof(OnDifficultyDoneButtonClicked))]
        [UIElement("DifficultyDoneButton")]
        private readonly Button _difficultyDoneButton;

        [UIElement("LevelDifficultyDisplay", false)]
        private readonly ModdedObject _levelDifficultyDisplayPrefab;

        [UIElement("DifficultyContent")]
        private readonly Transform _levelDifficultyDisplayContainer;

        private LevelDescription _selectedLevel;

        private Graphic _prevGraphic;

        public Action<LevelDescription> callback
        {
            get;
            set;
        }

        public override bool hideTitleScreen => true;

        public override void Update()
        {
            _doneButton.interactable = _selectedLevel != null;
            _difficultyDoneButton.interactable = _selectedLevel != null;
        }

        public void Populate(List<LevelDescription> levels)
        {
            if (_levelDescriptionDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_levelDescriptionDisplayContainer);

            if (levels.IsNullOrEmpty())
            {
                _levelsLabel.text = "No levels to pick";
                return;
            }

            Dictionary<string, List<LevelDescription>> levelLists = new Dictionary<string, List<LevelDescription>>();
            foreach (LevelDescription levelDescription in levels)
            {
                string source = levelDescription.GetLevelSource();

                if (!levelLists.ContainsKey(source))
                {
                    levelLists.Add(source, new List<LevelDescription>() { levelDescription });
                }
                else
                {
                    levelLists[source].Add(levelDescription);
                }
            }

            _levelsLabel.text = $"{levelLists.Count} {LocalizationManager.Instance.GetTranslatedString("mmcustomization_text_n_levels_to_pick")}";

            foreach (KeyValuePair<string, List<LevelDescription>> keyValue in levelLists)
            {
                SteamWorkshopItem steamWorkshopItem = keyValue.Value[0].WorkshopItem;
                bool isWorkshop = steamWorkshopItem != null;

                ModdedObject moddedObject = Instantiate(_levelDescriptionDisplayPrefab, _levelDescriptionDisplayContainer);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = isWorkshop ? steamWorkshopItem.Title : StringUtils.AddSpacesToCamelCasedString(keyValue.Key.Substring(keyValue.Key.LastIndexOf("/") + 1).Replace(".json", string.Empty));

                Graphic graphic = moddedObject.GetComponent<Graphic>();
                graphic.color = ModParseUtils.TryParseToColor(DESELECTED_COLOR, Color.gray);

                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    ShowDifficultySelection(keyValue.Value);
                });
            }
        }

        public void ShowDifficultySelection(List<LevelDescription> list)
        {
            _selectedLevel = null;
            if (_levelDifficultyDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_levelDifficultyDisplayContainer);

            list = list.OrderBy(f => (int)f.DifficultyTier).ToList();

            Dictionary<DifficultyTier, int> countOfEachDifficultyConfig = new Dictionary<DifficultyTier, int>();

            _levelDifficultySelectWindow.SetActive(true);
            foreach (LevelDescription level in list)
            {
                if (countOfEachDifficultyConfig.ContainsKey(level.DifficultyTier))
                    countOfEachDifficultyConfig[level.DifficultyTier]++;
                else
                    countOfEachDifficultyConfig.Add(level.DifficultyTier, 1);

                EndlessTierDescription description = EndlessModeManager.Instance.GetTierDescriptionFromTier(level.DifficultyTier);
                int difficultyCount = countOfEachDifficultyConfig[level.DifficultyTier];
                Color difficultyColor = description == null ? Color.white : description.TextColor;

                ModdedObject moddedObject = Instantiate(_levelDifficultyDisplayPrefab, _levelDifficultyDisplayContainer);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = $"{level.DifficultyTier.GetTierString()}{(difficultyCount <= 1 ? string.Empty : $" {difficultyCount}")}";
                moddedObject.GetObject<Text>(0).color = difficultyColor;

                Graphic graphic = moddedObject.GetComponent<Graphic>();
                graphic.color = ModParseUtils.TryParseToColor(DESELECTED_COLOR, Color.gray);

                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    if (_prevGraphic && _prevGraphic != graphic)
                    {
                        _prevGraphic.color = ModParseUtils.TryParseToColor(DESELECTED_COLOR, Color.gray);
                    }
                    graphic.color = ModParseUtils.TryParseToColor(SELECTED_COLOR, Color.cyan);
                    _prevGraphic = graphic;

                    _selectedLevel = level;
                });
            }
        }

        public void OnDifficultyCloseButtonClicked()
        {
            _selectedLevel = null;
            _levelDifficultySelectWindow.SetActive(false);
        }

        public void OnDifficultyDoneButtonClicked()
        {
            _levelDifficultySelectWindow.SetActive(false);
            OnDoneButtonClicked();
        }

        public void OnDoneButtonClicked()
        {
            Hide();
            callback?.Invoke(_selectedLevel);
            callback = null;
        }

        public void OnCancelButtonClicked()
        {
            Hide();
            callback?.Invoke(null);
            callback = null;
        }
    }
}
