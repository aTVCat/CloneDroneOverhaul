using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulSurveyUI : OverhaulUI
    {
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
            m_LikedText = MyModdedObject.GetObject<InputField>(7);

            m_IncludeGameLogs = MyModdedObject.GetObject<Toggle>(8);
            m_IncludeDeviceLogs = MyModdedObject.GetObject<Toggle>(9);

            SelectedRankIndex = -1;
        }

        public void Show()
        {
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);
            base.gameObject.SetActive(true);
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
            OverhaulTransitionController.DoTransitionWithAction(delegate
            {
                Application.Quit();
            });
        }

        public void OnSendClick()
        {
            executeFeedbackWebHook();
        }

        public void OnRank1Click() => SelectedRankIndex = 1;
        public void OnRank2Click() => SelectedRankIndex = 2;
        public void OnRank3Click() => SelectedRankIndex = 3;
        public void OnRank4Click() => SelectedRankIndex = 4;
        public void OnRank5Click() => SelectedRankIndex = 5;

        private void executeFeedbackWebHook()
        {
            OverhaulWebhooksController.ExecuteSurveysWebhook(SelectedRankIndex, m_ImproveText.text, m_LikedText.text, m_IncludeGameLogs.isOn, m_IncludeDeviceLogs.isOn);
        }

    }
}
