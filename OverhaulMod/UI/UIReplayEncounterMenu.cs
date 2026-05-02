using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIReplayEncounterMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnStartFromBeginningButtonClicked))]
        [UIElement("StartFromBeginningButton")]
        private readonly Button _restartButton;

        [UIElement("LevelDisplay", false)]
        private readonly ModdedObject _sectionDisplayPrefab;

        [UIElement("SectionHeader", false)]
        private readonly Text _levelHeaderPrefab;

        [UIElement("Content")]
        private readonly Transform _sectionsContainer;

        private Dictionary<string, int> _levelIdToSiblingIndex;

        public int ChapterIndex;

        public override bool RefreshOnlyCursor => true;

        protected override void OnInitialized()
        {
            _levelIdToSiblingIndex = new Dictionary<string, int>();
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void PopulateChapter(int chapterIndex)
        {
            _levelIdToSiblingIndex.Clear();
            if (_sectionsContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_sectionsContainer);

            ChapterIndex = chapterIndex;

            ModLevelSectionInfo[] sections = ModGameUtils.GetChapterSections(Path.Combine(ModLevelManager.Instance.chapterSectionsFolder, "story"), chapterIndex);
            if (sections == null || sections.Length == 0)
                return;

            LocalizationManager localizationManager = LocalizationManager.Instance;
            foreach (ModLevelSectionInfo chapterSection in sections)
            {
                if (chapterSection.DeserializationError)
                    continue;

                ModdedObject moddedObject = Instantiate(_sectionDisplayPrefab, _sectionsContainer);
                moddedObject.gameObject.SetActive(true);

                if (chapterSection.ChapterIndex < 3)
                {
                    moddedObject.GetObject<Text>(0).text = $"{localizationManager.GetTranslatedString("story_section_level")} {chapterSection.Order + 1}";
                }
                else
                {
                    string translation = $"story_section_c{chapterSection.ChapterIndex}_{chapterSection.Order + 1}";
                    if (localizationManager.HasTranslatedString(translation))
                        moddedObject.GetObject<Text>(0).text = localizationManager.GetTranslatedString(translation);
                    else
                        moddedObject.GetObject<Text>(0).text = chapterSection.DisplayName;
                }

                moddedObject.GetObject<Text>(1).text = $"{chapterSection.Order + 1}.";
                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    ModUIManager manager = ModUIManager.Instance;
                    if (manager)
                    {
                        Hide();
                        _ = manager.Hide(Utils.ModAssetBundles.UI, Utils.ModUIs.UI_CHAPTER_SELECT_MENU);
                    }

                    _ = ModLevelManager.Instance.SetStoryModeLevelProgress(chapterSection);

                    GameFlowManager gameFlowManager = GameFlowManager.Instance;
                    gameFlowManager._gameMode = GameMode.Story;
                    gameFlowManager.startSingplayerGameFromTitleScreen();
                });
            }
        }

        public void OnStartFromBeginningButtonClicked()
        {
            StoryModeChapterSelect legacyUI = ModCache.TitleScreenUI.ChapterSelectUI;
            if (!legacyUI)
            {
                ModUIUtils.MessagePopupOK("Error", "Legacy UI reference is NULL");
                return;
            }

            string methodName = $"OnChapter{ChapterIndex}Clicked";
            MethodInfo methodInfo = typeof(StoryModeChapterSelect).GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            if (methodInfo != null)
            {
                ModUIManager manager = ModUIManager.Instance;
                if (manager)
                {
                    Hide();
                    _ = manager.Hide(ModAssetBundles.UI, ModUIs.UI_CHAPTER_SELECT_MENU);
                }
                _ = methodInfo.Invoke(legacyUI, null);
            }
            else
            {
                ModUIUtils.MessagePopupOK("Error", $"Could not find method called {methodName}");
            }
        }
    }
}
