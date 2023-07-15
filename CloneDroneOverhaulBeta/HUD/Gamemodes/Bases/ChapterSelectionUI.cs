using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class ChapterSelectionUI : OverhaulGamemodeUIBase
    {
        private Button m_GoBackButton;
        private Text m_ChapterText;
        private Text m_ProgressText;

        private Button m_ContinueButton;
        private Text m_ContinueButtonText;
        private Button m_RestartButton;

        private Button m_NextChapterButton;
        private Button m_PrevChapterButton;

        private int m_SelectedChapter;

        protected override void OnInitialize()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            m_ChapterText = moddedObject.GetObject<Text>(0);
            m_ProgressText = moddedObject.GetObject<Text>(6);
            m_GoBackButton = moddedObject.GetObject<Button>(1);
            m_GoBackButton.onClick.AddListener(goBackToGamemodeSelection);
            m_ContinueButton = moddedObject.GetObject<Button>(2);
            m_ContinueButton.onClick.AddListener(OnContinueClicked);
            m_ContinueButtonText = moddedObject.GetObject<Text>(2);
            m_RestartButton = moddedObject.GetObject<Button>(3);
            m_RestartButton.onClick.AddListener(OnRestartClicked);

            m_NextChapterButton = moddedObject.GetObject<Button>(4);
            m_NextChapterButton.onClick.AddListener(OnNextChapterClicked);
            m_PrevChapterButton = moddedObject.GetObject<Button>(5);
            m_PrevChapterButton.onClick.AddListener(OnPrevChapterClicked);
        }

        protected override void OnShow()
        {
            resetView();

            int num = MetagameProgressManager.Instance.CurrentProgressHasReached(MetagameProgress.P2_FirstHumanEscaped) ? 2 : 1;
            int num2 = GameDataManager.Instance.GetNumberOfStoryLevelsWon() + 1;
            if (num == 1 && num2 == 1)
            {
                m_ProgressText.gameObject.SetActive(false);
                m_ContinueButtonText.text = LocalizationManager.Instance.GetTranslatedString("New Game");
            }
            else
            {
                m_ProgressText.gameObject.SetActive(true);
                m_ContinueButtonText.text = LocalizationManager.Instance.GetTranslatedString("Continue");
            }
            m_ProgressText.text = string.Concat(new object[]
            {
                 LocalizationManager.Instance.GetTranslatedString("Chapter", -1),
                 " ",
                 num,
                 ", ",
                 Singleton<LocalizationManager>.Instance.GetTranslatedString("Level", -1),
                 " ",
                 num2
            });
        }

        private void resetView()
        {
            SelectChapter(1);
        }

        private void goBackToGamemodeSelection()
        {
            Hide();
            GameUIRoot.Instance.TitleScreenUI.SetSinglePlayerModeSelectButtonsVisibile(true);
        }

        public void SwitchChapter(bool right)
        {
            SelectChapter(right ? m_SelectedChapter + 1 : m_SelectedChapter - 1);
        }

        public void SelectChapter(int chapterIndex)
        {
            m_NextChapterButton.interactable = true;
            m_PrevChapterButton.interactable = true;

            m_SelectedChapter = chapterIndex;
            if (chapterIndex < 1)
                m_SelectedChapter = 1;

            if (chapterIndex > 5)
                m_SelectedChapter = 5;

            if (chapterIndex < 2)
                m_PrevChapterButton.interactable = false;

            if (chapterIndex > 4)
                m_NextChapterButton.interactable = false;

            m_ChapterText.text = LocalizationManager.Instance.GetTranslatedString("Chapter") + " " + m_SelectedChapter;
            GamemodesUI.ChangeBackgroundTexture(OverhaulMod.Core.ModDirectory + "Assets/Previews/chapter" + m_SelectedChapter + "Preview.jpeg");
        }

        public void OnContinueClicked()
        {
            GamemodesUI.FullscreenWindow.Show(delegate
            {
                GameFlowManager.Instance.StartStoryModeGame(false);
                Hide();
            }, 0);
        }

        public void OnRestartClicked()
        {
            GamemodesUI.FullscreenWindow.Show(delegate
            {
                if (m_SelectedChapter == 1)
                    MetagameProgressManager.Instance.ResetToChapter1();
                else if (m_SelectedChapter == 2)
                    MetagameProgressManager.Instance.ResetToChapter2();
                else if (m_SelectedChapter == 3)
                    MetagameProgressManager.Instance.ResetToChapter3();
                else if (m_SelectedChapter == 4)
                    MetagameProgressManager.Instance.ResetToChapter4();
                else if (m_SelectedChapter == 5)
                    MetagameProgressManager.Instance.ResetToChapter5();

                GameFlowManager.Instance.StartStoryModeGame(true);
                Hide();
            }, 0);
        }

        public void OnNextChapterClicked()
        {
            if (m_SelectedChapter == 5)
                return;

            SwitchChapter(true);
        }

        public void OnPrevChapterClicked()
        {
            if (m_SelectedChapter == 1)
                return;

            SwitchChapter(false);
        }

        public override void Update()
        {
            base.Update();

            if (GamemodesUI.FullscreenWindow.IsActive)
                return;

            if (Input.GetKeyDown(KeyCode.Backspace))
                goBackToGamemodeSelection();

            if (Input.GetKeyDown(KeyCode.E))
                OnNextChapterClicked();

            if (Input.GetKeyDown(KeyCode.Q))
                OnPrevChapterClicked();

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                OnContinueClicked();
        }
    }
}
