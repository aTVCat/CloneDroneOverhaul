using OverhaulMod.Gameplay;
using OverhaulMod.Utils;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIChapterSelectMenuRework : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnRestartChapter1ButtonClicked))]
        [UIElement("RestartC1")]
        private readonly Button _chapter1Button;

        [UIElementAction(nameof(OnRestartChapter2ButtonClicked))]
        [UIElement("RestartC2")]
        private readonly Button _chapter2Button;

        [UIElementAction(nameof(OnRestartChapter3ButtonClicked))]
        [UIElement("RestartC3")]
        private readonly Button _chapter3Button;

        [UIElementAction(nameof(OnRestartChapter4ButtonClicked))]
        [UIElement("RestartC4")]
        private readonly Button _chapter4Button;

        [UIElementAction(nameof(OnRestartChapter5ButtonClicked))]
        [UIElement("RestartC5")]
        private readonly Button _chapter5Button;

        [UIElementAction(nameof(OnContinueButtonClicked))]
        [UIElement("ContinueButton")]
        private readonly Button _continueButton;

        [UIElementAction(nameof(OnLegacyUIButtonClicked))]
        [UIElement("OldUIButton")]
        private readonly Button _legacyUIButton;

        [UIElementAction(nameof(OnDifficultyDropdownEdit))]
        [UIElement("DifficultyDropdown")]
        private readonly Dropdown _difficultyDropdown;

        [UIElementAction(nameof(OnEnableGreatSwordsToggleEdit))]
        [UIElement("EnableGreatswordsToggle")]
        private readonly Toggle _enableGreatSwordsToggle;

        [UIElement("ProgressText")]
        private readonly Text _progressText;

        [UIElement("ContinueText")]
        private readonly Text _continueButtonText;

        public override bool RefreshOnlyCursor => true;

        public override void Show()
        {
            base.Show();
            ModCache.TitleScreenUI.SetSinglePlayerModeSelectButtonsVisibile(false);
            _difficultyDropdown.options = SettingsManager.Instance.GetDifficultyOptions();
            _difficultyDropdown.value = SettingsManager.Instance.GetStoryDifficultyIndex();
            _enableGreatSwordsToggle.isOn = ModGameModifiersManager.Instance.ForceEnableGreatSwords;
            RefreshProgressText();
        }

        public override void Hide()
        {
            base.Hide();
            if (!ModCache.TitleScreenUI.ChapterSelectUI.gameObject.activeInHierarchy)
                ModCache.TitleScreenUI.SetSinglePlayerModeSelectButtonsVisibile(true);
        }

        public void RefreshProgressText()
        {
            _progressText.text = string.Empty;

            int currentChapter = MetagameProgressManager.Instance.CurrentProgressHasReached(MetagameProgress.P2_FirstHumanEscaped) ? 2 : 1;
            int numLevels = GameDataManager.Instance.GetNumberOfStoryLevelsWon() + 1;
            if (currentChapter <= 1 && numLevels <= 1)
                _continueButtonText.text = LocalizationManager.Instance.GetTranslatedString("New Game");
            else
            {
                string chapterText = LocalizationManager.Instance.GetTranslatedString("Chapter");
                string levelText = LocalizationManager.Instance.GetTranslatedString("Level");

                _continueButtonText.text = LocalizationManager.Instance.GetTranslatedString("Continue");
                _progressText.text = Singleton<MetagameProgressManager>.Instance.CurrentProgressHasReached(MetagameProgress.P10_ConqueredBattlecruiser)
                    ? $"{chapterText} 5"
                    : Singleton<MetagameProgressManager>.Instance.CurrentProgressHasReached(MetagameProgress.P7_CompletedTowerAssault)
                    ? $"{chapterText} 4"
                    : Singleton<MetagameProgressManager>.Instance.CurrentProgressHasReached(MetagameProgress.P5_DestroyedAlphaCentauri)
                    ? $"{chapterText} 3"
                    : $"{chapterText} {currentChapter}, {levelText} {numLevels}";
            }

            _chapter1Button.gameObject.SetActive(true);
            _chapter2Button.gameObject.SetActive(MetagameProgressManager.Instance.HasBeatChapter1());
            _chapter3Button.gameObject.SetActive(MetagameProgressManager.Instance.HasBeatChapter2());
            _chapter4Button.gameObject.SetActive(MetagameProgressManager.Instance.HasBeatChapter3());
            _chapter5Button.gameObject.SetActive(MetagameProgressManager.Instance.HasBeatChapter4());
        }

        public void ShowChapterLevelSelectionMenu(int chapterIndex)
        {
            _ = ModUIConstants.ShowChapterLevelSelectMenu(base.transform, chapterIndex);
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

        public void OnContinueButtonClicked()
        {
            Hide();
            GameFlowManager.Instance.StartStoryModeGame();
        }

        public void OnDifficultyDropdownEdit(int index)
        {
            SettingsManager.Instance.SetStoryDifficultyIndex(index);
        }

        public void OnEnableGreatSwordsToggleEdit(bool value)
        {
            ModGameModifiersManager.Instance.ForceEnableGreatSwords = value;
        }

        public void OnLegacyUIButtonClicked()
        {
            TitleScreenUI titleScreenUI = ModCache.TitleScreenUI;
            if (titleScreenUI)
            {
                titleScreenUI.ChapterSelectUI.Show();
                Hide();
            }
        }
    }
}
