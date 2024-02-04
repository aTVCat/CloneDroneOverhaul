using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
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

        private List<LevelDescription> m_selectedLevels;

        public Action<List<LevelDescription>> callback
        {
            get;
            set;
        }

        public bool allowMultiSelection
        {
            get;
            set;
        }

        public override bool hideTitleScreen => true;

        protected override void OnInitialized()
        {
            m_selectedLevels = new List<LevelDescription>();
        }

        public override void Update()
        {
            m_doneButton.interactable = !m_selectedLevels.IsNullOrEmpty();
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

            string levelWord = levels.Count == 1 ? "level" : "levels";
            m_levelsLabel.text = $"{levels.Count} {levelWord} to pick";

            foreach(var levelDescription in levels)
            {
                ModdedObject moddedObject = Instantiate(m_levelDescriptionDisplayPrefab, m_levelDescriptionDisplayContainer);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = levelDescription.LevelID;

                Graphic graphic = moddedObject.GetComponent<Graphic>();
                graphic.color = ModParseUtils.TryParseToColor(DESELECTED_COLOR, Color.gray);

                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    if (!m_selectedLevels.Contains(levelDescription))
                    {
                        if (!allowMultiSelection && m_selectedLevels.Count > 0)
                            return;

                        m_selectedLevels.Add(levelDescription);
                        graphic.color = ModParseUtils.TryParseToColor(SELECTED_COLOR, Color.cyan);
                    }
                    else
                    {
                        if (m_selectedLevels.Remove(levelDescription))
                            graphic.color = ModParseUtils.TryParseToColor(DESELECTED_COLOR, Color.gray);
                    }
                });
            }
        }

        public void OnDoneButtonClicked()
        {
            Hide();
            callback?.Invoke(m_selectedLevels.IsNullOrEmpty() ? null : m_selectedLevels);
            callback = null;

            if (m_selectedLevels != null)
                m_selectedLevels.Clear();
        }

        public void OnCancelButtonClicked()
        {
            Hide();
            callback?.Invoke(null);
            callback = null;

            if (m_selectedLevels != null)
                m_selectedLevels.Clear();
        }
    }
}
