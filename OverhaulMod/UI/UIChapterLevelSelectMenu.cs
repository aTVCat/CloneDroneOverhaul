using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIChapterLevelSelectMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnStartFromBeginningButtonClicked))]
        [UIElement("StartFromBeginningButton")]
        private readonly Button m_restartButton;

        public int chapterIndex
        {
            get;
            private set;
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
            this.chapterIndex = chapterIndex;
            bool hasOnlyLevels = chapterIndex == 1 || chapterIndex == 2;
            bool hasOnlySection = chapterIndex == 3;
            bool hasBoth = chapterIndex == 4 || chapterIndex == 5;

            Debug.LogFormat("Only levels: {0}, Only sections: {1}, Both: {2}", new object[] { hasOnlyLevels, hasOnlySection, hasBoth });
        }

        public void OnStartFromBeginningButtonClicked()
        {
            StoryModeChapterSelect legacyUI = ModCache.titleScreenUI.ChapterSelectUI;
            if (!legacyUI)
                return;

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
                Debug.LogWarningFormat("[ChaptersMenu} Could not find method called {0}", methodName);
            }
        }
    }
}
