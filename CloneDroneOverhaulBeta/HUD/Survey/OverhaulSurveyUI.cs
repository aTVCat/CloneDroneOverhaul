using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulSurveyUI : OverhaulUI
    {
        public static bool HasSentFeedbackThisSession;

        private Button m_GoBack;
        private Button m_Send;
        private Button m_ExitGame;

        private Transform m_SendingLabelTransform;

        private Button[] m_RankButtons;
        private InputField m_ImproveText;
        private InputField m_LikedText;

        private Toggle m_IncludeGameLogs;
        private Toggle m_IncludeDeviceLogs;

        public int SelectedRankIndex;

        public override void Initialize()
        {
            base.gameObject.SetActive(false);

            m_GoBack = MyModdedObject.GetObject<Button>(0);
            m_GoBack.onClick.AddListener(OnBackClick);
            m_Send = MyModdedObject.GetObject<Button>(10);
            m_Send.onClick.AddListener(OnSendClick);
            m_ExitGame = MyModdedObject.GetObject<Button>(11);
            m_ExitGame.onClick.AddListener(OnExitGameClick);

            m_SendingLabelTransform = MyModdedObject.GetObject<Transform>(12);

            m_RankButtons = new Button[5];
            m_RankButtons[0] = MyModdedObject.GetObject<Button>(1);
            m_RankButtons[0].onClick.AddListener(OnRank1Click);
            m_RankButtons[1] = MyModdedObject.GetObject<Button>(2);
            m_RankButtons[1].onClick.AddListener(OnRank2Click);
            m_RankButtons[2] = MyModdedObject.GetObject<Button>(3);
            m_RankButtons[2].onClick.AddListener(OnRank3Click);
            m_RankButtons[3] = MyModdedObject.GetObject<Button>(4);
            m_RankButtons[3].onClick.AddListener(OnRank4Click);
            m_RankButtons[4] = MyModdedObject.GetObject<Button>(5);
            m_RankButtons[4].onClick.AddListener(OnRank5Click);
            m_ImproveText = MyModdedObject.GetObject<InputField>(6);
            m_ImproveText.text = string.Empty;
            m_LikedText = MyModdedObject.GetObject<InputField>(7);
            m_LikedText.text = string.Empty;

            m_IncludeGameLogs = MyModdedObject.GetObject<Toggle>(8);
            m_IncludeDeviceLogs = MyModdedObject.GetObject<Toggle>(9);

            SelectedRankIndex = -1;
        }

        public void Show()
        {
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);
            base.gameObject.SetActive(true);

            m_SendingLabelTransform.gameObject.SetActive(false);
            SetAllInteractable(!HasSentFeedbackThisSession);
        }

        public void Hide()
        {
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);
            base.gameObject.SetActive(false);
        }

        public void OnBackClick()
        {
            Hide();
        }

        public void OnExitGameClick()
        {
            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsNewTransitionScreenEnabled)
            {
                Application.Quit();
                return;
            }

            OverhaulTransitionController.DoTransitionWithAction(delegate
            {
                Application.Quit();
            });
        }

        public void OnSendClick()
        {
            if (!checkFields() || HasSentFeedbackThisSession)
                return;

            executeFeedbackWebHook();
        }

        public void OnRank1Click() => SelectedRankIndex = 1;
        public void OnRank2Click() => SelectedRankIndex = 2;
        public void OnRank3Click() => SelectedRankIndex = 3;
        public void OnRank4Click() => SelectedRankIndex = 4;
        public void OnRank5Click() => SelectedRankIndex = 5;

        public void SetAllInteractable(bool value)
        {
            foreach (Button b in m_RankButtons)
                if (b)
                    b.interactable = value;

            m_ImproveText.interactable = value;
            m_LikedText.interactable = value;
            m_IncludeDeviceLogs.interactable = value;
            m_IncludeGameLogs.interactable = value;
            m_Send.interactable = value;
        }

        private bool checkFields()
        {
            if (SelectedRankIndex == -1)
            {
                OverhaulDialogues.CreateDialogue("Error", "You forgot to rate the modded experience...", 0f, new Vector2(315, 160), null);
                return false;
            }
            if (string.IsNullOrEmpty(m_ImproveText.text) && string.IsNullOrEmpty(m_LikedText.text))
            {
                OverhaulDialogues.CreateDialogue("Error", "You didn't write anything..", 0f, new Vector2(315, 160), null);
                return false;
            }
            return true;
        }

        private void executeFeedbackWebHook()
        {
            HasSentFeedbackThisSession = true;

            SetAllInteractable(false);
            m_ExitGame.interactable = false;
            m_SendingLabelTransform.gameObject.SetActive(true);
            _ = StaticCoroutineRunner.StartStaticCoroutine(executeFeedbackWebhookCoroutine());
        }

        private IEnumerator executeFeedbackWebhookCoroutine()
        {
            OverhaulWebhooksController.ExecuteSurveysWebhook(SelectedRankIndex, m_ImproveText.text, m_LikedText.text);
            yield return new WaitForSecondsRealtime(2f);
            m_SendingLabelTransform.gameObject.SetActive(false);
            m_ExitGame.interactable = true;
            OverhaulDialogues.CreateDialogue("Feedback sent!", "", 0f, new Vector2(315, 40), null);
            yield break;
        }

    }
}
