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
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnDoneButtonClicked))]
        [UIElement("DoneButton")]
        private readonly Button m_doneButton;

        [UIElementAction(nameof(OnCancelButtonClicked))]
        [UIElement("CancelButton")]
        private readonly Button m_cancelButton;

        [UIElement("LevelDescriptionDisplay", false)]
        private readonly ModdedObject m_levelDescriptionDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform m_levelDescriptionDisplayContainer;

        [UIElement("LevelsLabel")]
        private readonly Text m_levelsLabel;

        [UIElement("DifficultySelectWindow", false)]
        private readonly GameObject m_levelDifficultySelectWindow;

        [UIElementAction(nameof(OnDifficultyCloseButtonClicked))]
        [UIElement("DifficultyCloseButton")]
        private readonly Button m_difficultyExitButton;

        [UIElementAction(nameof(OnDifficultyDoneButtonClicked))]
        [UIElement("DifficultyDoneButton")]
        private readonly Button m_difficultyDoneButton;

        [UIElement("LevelDifficultyDisplay", false)]
        private readonly ModdedObject m_levelDifficultyDisplayPrefab;

        [UIElement("DifficultyContent")]
        private readonly Transform m_levelDifficultyDisplayContainer;

        private LevelDescription m_selectedLevel;

        private Graphic m_prevGraphic;

        public Action<LevelDescription> callback
        {
            get;
            set;
        }

        public override bool hideTitleScreen => true;

        public override void Update()
        {
            m_doneButton.interactable = m_selectedLevel != null;
            m_difficultyDoneButton.interactable = m_selectedLevel != null;
        }

        public void Populate(List<LevelDescription> levels)
        {
            if (m_levelDescriptionDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(m_levelDescriptionDisplayContainer);

            if (levels.IsNullOrEmpty())
            {
                m_levelsLabel.text = "No levels to pick";
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

            m_levelsLabel.text = $"{levelLists.Count} {LocalizationManager.Instance.GetTranslatedString("mmcustomization_text_n_levels_to_pick")}";

            foreach (KeyValuePair<string, List<LevelDescription>> keyValue in levelLists)
            {
                SteamWorkshopItem steamWorkshopItem = keyValue.Value[0].WorkshopItem;
                bool isWorkshop = steamWorkshopItem != null;

                ModdedObject moddedObject = Instantiate(m_levelDescriptionDisplayPrefab, m_levelDescriptionDisplayContainer);
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
            m_selectedLevel = null;
            if (m_levelDifficultyDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(m_levelDifficultyDisplayContainer);

            list = list.OrderBy(f => (int)f.DifficultyTier).ToList();

            Dictionary<DifficultyTier, int> countOfEachDifficultyConfig = new Dictionary<DifficultyTier, int>();

            m_levelDifficultySelectWindow.SetActive(true);
            foreach (LevelDescription level in list)
            {
                if (countOfEachDifficultyConfig.ContainsKey(level.DifficultyTier))
                    countOfEachDifficultyConfig[level.DifficultyTier]++;
                else
                    countOfEachDifficultyConfig.Add(level.DifficultyTier, 1);

                EndlessTierDescription description = EndlessModeManager.Instance.GetTierDescriptionFromTier(level.DifficultyTier);
                int difficultyCount = countOfEachDifficultyConfig[level.DifficultyTier];
                Color difficultyColor = description == null ? Color.white : description.TextColor;

                ModdedObject moddedObject = Instantiate(m_levelDifficultyDisplayPrefab, m_levelDifficultyDisplayContainer);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = $"{level.DifficultyTier.GetTierString()}{(difficultyCount <= 1 ? string.Empty : $" {difficultyCount}")}";
                moddedObject.GetObject<Text>(0).color = difficultyColor;

                Graphic graphic = moddedObject.GetComponent<Graphic>();
                graphic.color = ModParseUtils.TryParseToColor(DESELECTED_COLOR, Color.gray);

                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    if (m_prevGraphic && m_prevGraphic != graphic)
                    {
                        m_prevGraphic.color = ModParseUtils.TryParseToColor(DESELECTED_COLOR, Color.gray);
                    }
                    graphic.color = ModParseUtils.TryParseToColor(SELECTED_COLOR, Color.cyan);
                    m_prevGraphic = graphic;

                    m_selectedLevel = level;
                });
            }
        }

        public void OnDifficultyCloseButtonClicked()
        {
            m_selectedLevel = null;
            m_levelDifficultySelectWindow.SetActive(false);
        }

        public void OnDifficultyDoneButtonClicked()
        {
            m_levelDifficultySelectWindow.SetActive(false);
            OnDoneButtonClicked();
        }

        public void OnDoneButtonClicked()
        {
            Hide();
            callback?.Invoke(m_selectedLevel);
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
