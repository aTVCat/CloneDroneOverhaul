using OverhaulMod.Utils;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIChapterSelectMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnRestartChapter1ButtonClicked))]
        [UIElement("RestartC1")]
        private readonly Button m_chapter1Button;

        [UIElementAction(nameof(OnRestartChapter2ButtonClicked))]
        [UIElement("RestartC2")]
        private readonly Button m_chapter2Button;

        [UIElementAction(nameof(OnRestartChapter3ButtonClicked))]
        [UIElement("RestartC3")]
        private readonly Button m_chapter3Button;

        [UIElementAction(nameof(OnRestartChapter4ButtonClicked))]
        [UIElement("RestartC4")]
        private readonly Button m_chapter4Button;

        [UIElementAction(nameof(OnRestartChapter5ButtonClicked))]
        [UIElement("RestartC5")]
        private readonly Button m_chapter5Button;

        public override void Show()
        {
            base.Show();
            ModCache.titleScreenUI.SetSinglePlayerModeSelectButtonsVisibile(false);
        }

        public override void Hide()
        {
            base.Hide();
            ModCache.titleScreenUI.SetSinglePlayerModeSelectButtonsVisibile(true);
        }

        public void ShowChapterLevelSelectionMenu(int chapterIndex)
        {
            ModUIConstants.ShowChapterLevelSelectMenu(base.transform, chapterIndex);
        }

        public void OnRestartChapter1ButtonClicked()
        {
            ShowChapterLevelSelectionMenu(1);
        }

        public void OnRestartChapter2ButtonClicked()
        {
            ShowChapterLevelSelectionMenu(2);
        }

        public void OnRestartChapter3ButtonClicked()
        {
            ShowChapterLevelSelectionMenu(3);
        }

        public void OnRestartChapter4ButtonClicked()
        {
            ShowChapterLevelSelectionMenu(4);
        }

        public void OnRestartChapter5ButtonClicked()
        {
            ShowChapterLevelSelectionMenu(5);
        }
    }
}
