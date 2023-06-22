using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class ChapterSelectionUI : OverhaulGamemodeUIBase
    {
        private Button m_GoBackButton;
        private Text m_ChapterText;

        private Button m_ContinueButton;
        private Button m_RestartButton;

        private int m_SelectedChapter;

        protected override void OnInitialize()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            m_ChapterText = moddedObject.GetObject<Text>(0);
            m_GoBackButton = moddedObject.GetObject<Button>(1);
            m_GoBackButton.onClick.AddListener(goBackToGamemodeSelection);
            m_ContinueButton = moddedObject.GetObject<Button>(2);
            m_ContinueButton.onClick.AddListener(OnContinueClicked);
            m_RestartButton = moddedObject.GetObject<Button>(3);
            m_RestartButton.onClick.AddListener(OnRestartClicked);
        }

        protected override void OnShow()
        {
            resetView();
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
            if (!GamemodesUI.AllowSwitching)
                return;

            m_SelectedChapter = chapterIndex;
            if (chapterIndex < 1)
                m_SelectedChapter = 5;

            if (chapterIndex > 5)
                m_SelectedChapter = 1;

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
                MethodInfo info = MetagameProgressManager.Instance.GetType().GetMethod("ResetToChapter" + m_SelectedChapter, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                _ = info.Invoke(MetagameProgressManager.Instance, null);
                Singleton<GameFlowManager>.Instance.StartStoryModeGame(true);
                Hide();
            }, 0);
        }

        public override void Update()
        {
            base.Update();

            if (!GamemodesUI.AllowSwitching || GamemodesUI.FullscreenWindow.IsActive)
                return;

            if (Input.GetKeyDown(KeyCode.Backspace))
                goBackToGamemodeSelection();

            if (Input.GetKeyDown(KeyCode.E))
                SwitchChapter(true);

            if (Input.GetKeyDown(KeyCode.Q))
                SwitchChapter(false);

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                OnContinueClicked();
        }
    }
}
