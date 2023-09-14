using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UIFeedbackMenu : UIController
    {
        public static bool HasSentFeedbackThisSession;

        [UIElementActionReference(nameof(OnRank1Click))]
        [UIElementReference("BadRank")]
        private Button m_Rank1Button;

        [UIElementActionReference(nameof(OnRank2Click))]
        [UIElementReference("MehRank")]
        private Button m_Rank2Button;

        [UIElementActionReference(nameof(OnRank3Click))]
        [UIElementReference("NeutralRank")]
        private Button m_Rank3Button;

        [UIElementActionReference(nameof(OnRank4Click))]
        [UIElementReference("GoodRank")]
        private Button m_Rank4Button;

        [UIElementActionReference(nameof(OnRank5Click))]
        [UIElementReference("SatisfiedRank")]
        private Button m_Rank5Button;

        [UIElementReference("improve")]
        private InputField m_ImproveText;

        [UIElementReference("like")]
        private InputField m_LikedText;

        [UIElementReference("IncludeUsername")]
        private Toggle m_IncludeGameLogs;

        [UIElementReference("IncludeDeviceInfo")]
        private Toggle m_IncludeDeviceLogs;

        [UIElementDefaultVisibilityState(false)]
        [UIElementReference("StatusLabel")]
        private Transform m_SendingLabelTransform;

        [UIElementActionReference(nameof(OnBackClicked))]
        [UIElementReference("BackButton")]
        private Button m_GoBackButton;

        [UIElementActionReference(nameof(OnSendClicked))]
        [UIElementReference("SendFeedback")]
        private Button m_SendButton;

        [UIElementActionReference(nameof(OnExitGameClicked))]
        [UIElementReference("CloseGameButton")]
        private Button m_ExitGameButton;

        private int m_SelectedRank;
        public int SelectedRank
        {
            get => m_SelectedRank;
            set
            {
                m_SelectedRank = value;
                m_Rank1Button.interactable = value != 1;
                m_Rank2Button.interactable = value != 2;
                m_Rank3Button.interactable = value != 3;
                m_Rank4Button.interactable = value != 4;
                m_Rank5Button.interactable = value != 5;
            }
        }

        protected override bool HideTitleScreen() => true;
        protected override bool WaitForEscapeKeyToHide() => true;

        public override void Initialize()
        {
            base.Initialize();
            m_ImproveText.text = string.Empty;
            m_LikedText.text = string.Empty;
            SelectedRank = -1;
        }

        public override void Show()
        {
            base.Show();
            SetAllInteractable(!HasSentFeedbackThisSession);
        }

        public void OnBackClicked()
        {
            Hide();
        }

        public void OnExitGameClicked()
        {
            OverhaulTransitionManager.reference.DoTransition(delegate
            {
                Application.Quit();
            });
        }

        public void OnSendClicked()
        {
            if (!checkFields() || HasSentFeedbackThisSession)
                return;

            executeFeedbackWebHook();
        }

        public void OnRank1Click() => SelectedRank = 1;
        public void OnRank2Click() => SelectedRank = 2;
        public void OnRank3Click() => SelectedRank = 3;
        public void OnRank4Click() => SelectedRank = 4;
        public void OnRank5Click() => SelectedRank = 5;

        public void SetAllInteractable(bool value)
        {
            m_Rank1Button.interactable = value;
            m_Rank2Button.interactable = value;
            m_Rank3Button.interactable = value;
            m_Rank4Button.interactable = value;
            m_Rank5Button.interactable = value;

            m_ImproveText.interactable = value;
            m_LikedText.interactable = value;
            m_IncludeDeviceLogs.interactable = value;
            m_IncludeGameLogs.interactable = value;
            m_SendButton.interactable = value;

            if (value)
                SelectedRank = SelectedRank;
        }

        private bool checkFields()
        {
            if (SelectedRank == -1)
            {
                OverhaulDialogues.CreateDialogue("Error", "You forgot to rate the modded experience...", 0f, new Vector2(315, 160), null);
                return false;
            }
            if (string.IsNullOrEmpty(m_ImproveText.text) && SelectedRank < 3)
            {
                OverhaulDialogues.CreateDialogue("Error", "You didn't enjoy the mod and didn't tell ME what thing(s) should to be improved?\nHuh..", 0f, new Vector2(315, 160), null);
                return false;
            }
            if (string.IsNullOrEmpty(m_LikedText.text) && SelectedRank > 3)
            {
                OverhaulDialogues.CreateDialogue("Error", "It doesn't seem that you don't have any favorite thing in the mod...", 0f, new Vector2(315, 160), null);
                return false;
            }
            return true;
        }

        private void executeFeedbackWebHook()
        {
            HasSentFeedbackThisSession = true;

            SetAllInteractable(false);
            m_ExitGameButton.interactable = false;
            m_SendingLabelTransform.gameObject.SetActive(true);
            _ = StaticCoroutineRunner.StartStaticCoroutine(executeFeedbackWebhookCoroutine());
        }

        private IEnumerator executeFeedbackWebhookCoroutine()
        {
            OverhaulWebhooks.ExecuteSurveysWebhook(SelectedRank, m_ImproveText.text, m_LikedText.text, m_IncludeGameLogs.isOn, true);
            yield return new WaitForSecondsRealtime(2f);
            m_SendingLabelTransform.gameObject.SetActive(false);
            m_ExitGameButton.interactable = true;
            OverhaulDialogues.CreateDialogue("Feedback sent!", "", 0f, new Vector2(315, 40), null);
            yield break;
        }

    }
}
