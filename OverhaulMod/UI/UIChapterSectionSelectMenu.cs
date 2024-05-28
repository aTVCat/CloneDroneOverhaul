using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIChapterSectionSelectMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnStartFromBeginningButtonClicked))]
        [UIElement("StartFromBeginningButton")]
        private readonly Button m_restartButton;

        [UIElement("LevelDisplay", false)]
        private readonly ModdedObject m_sectionDisplayPrefab;

        [UIElement("SectionHeader", false)]
        private readonly Text m_levelHeaderPrefab;

        [UIElement("Content")]
        private readonly Transform m_sectionsContainer;

        private Dictionary<string, int> m_levelIdToSiblingIndex;

        public int chapterIndex
        {
            get;
            private set;
        }

        public override bool refreshOnlyCursor => true;

        protected override void OnInitialized()
        {
            m_levelIdToSiblingIndex = new Dictionary<string, int>();
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
            m_levelIdToSiblingIndex.Clear();
            if (m_sectionsContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(m_sectionsContainer);

            this.chapterIndex = chapterIndex;

            ModLevelSectionInfo[] sections = ModGameUtils.GetChapterSections(ModLevelManager.Instance.chapterSectionsFolder + "story", chapterIndex);
            if (sections == null || sections.Length == 0)
                return;

            foreach (ModLevelSectionInfo chapterSection in sections)
            {
                if (chapterSection.DeserializationError)
                    continue;

                ModdedObject moddedObject = Instantiate(m_sectionDisplayPrefab, m_sectionsContainer);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = chapterSection.DisplayName;
                moddedObject.GetObject<Text>(1).text = $"{chapterSection.Order + 1}.";
                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    ModUIManager manager = ModUIManager.Instance;
                    if (manager)
                    {
                        Hide();
                        _ = manager.Hide(Utils.AssetBundleConstants.UI, Utils.ModUIConstants.UI_CHAPTER_SELECT_MENU);
                    }

                    _ = ModLevelManager.Instance.SetStoryModeLevelProgress(chapterSection);
                    GameFlowManager.Instance.StartStoryModeGame(false);
                    GameDataManager.Instance.SetCurrentLevelID(chapterSection.LevelID);
                });
            }
        }

        public void OnStartFromBeginningButtonClicked()
        {
            StoryModeChapterSelect legacyUI = ModCache.titleScreenUI.ChapterSelectUI;
            if (!legacyUI)
            {
                ModUIUtils.MessagePopupOK("Error", "Legacy UI reference is NULL");
                return;
            }

            string methodName = string.Format("OnChapter{0}Clicked", chapterIndex);
            MethodInfo methodInfo = typeof(StoryModeChapterSelect).GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            if (methodInfo != null)
            {
                ModUIManager manager = ModUIManager.Instance;
                if (manager)
                {
                    Hide();
                    _ = manager.Hide(Utils.AssetBundleConstants.UI, Utils.ModUIConstants.UI_CHAPTER_SELECT_MENU);
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
