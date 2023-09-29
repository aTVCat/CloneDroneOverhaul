using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class DuelPlayUI : OverhaulGamemodeUIBase
    {
        [UIElementReferenceAttribute("BackButton")]
        private readonly Button m_GoBack;

        [UIElementReferenceAttribute("PlayButton")]
        private readonly Button m_Play;
        [UIElementReferenceAttribute("PlayPrivateButton")]
        private readonly Button m_PlayPrivate;

        [UIElementActionReference(nameof(OnPrevMatchesClick))]
        [UIElementReferenceAttribute("PrevMatchesButton")]
        private readonly Button m_PrevMatches;

        [UIElementReferenceAttribute("RelayConnectionToggle")]
        private readonly Toggle m_Relay;

        protected override void OnInitialize()
        {
            UIController.AssignVariables(this);

            m_GoBack.onClick.AddListener(goBackToGamemodeSelection);
            m_Play.onClick.AddListener(OnPlayClicked);
            m_PlayPrivate.onClick.AddListener(OnPlayPrivateClick);
            m_Relay.onValueChanged.AddListener(OnRelayToggleClick);
        }

        protected override void OnShow()
        {
            GamemodesUI.ChangeBackgroundTexture(OverhaulMod.Core.ModDirectory + "Assets/Previews/DuelBG_" + UnityEngine.Random.Range(1, 4) + ".jpg");
            m_Relay.isOn = OverhaulCore.IsRelayConnectionEnabled;
            GameUIRoot.Instance.TitleScreenUI.SetMultiplayerPlayerModeSelectButtonsVisibile(false);
        }

        private void goBackToGamemodeSelection()
        {
            Hide();
            GameUIRoot.Instance.TitleScreenUI.SetMultiplayerPlayerModeSelectButtonsVisibile(true);
        }

        public void OnPlayClicked()
        {
            if (ExperimentalBranchManager.Instance.UseGameye)
                MultiplayerMatchmakingManager.Instance.Matchmaking20Private.StopSearching(false);

            MultiplayerMatchmakingManager.Instance.FindAndJoinMatch(new GameRequest
            {
                GameType = GameRequestType.RandomDuel
            });
            Hide();
        }

        public void OnPlayPrivateClick()
        {
            GamemodesUI.FullscreenWindow.Show(null, 6);
        }

        public void OnPrevMatchesClick()
        {
            OverhaulFullscreenDialogueWindow.ShowUnfinishedFeatureWindow();
        }

        public void OnRelayToggleClick(bool newValue)
        {
            OverhaulSettingInfo_Old.SavePref(OverhaulSettingsManager_Old.GetSetting("Gameplay.Multiplayer.Relay Connection", true), newValue);
        }

        private void Update()
        {
            base.Update();

            if (GamemodesUI.FullscreenWindow.IsActive)
                return;

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                OnPlayClicked();

            if (Input.GetKeyDown(KeyCode.P))
                OnPlayPrivateClick();

            if (Input.GetKeyDown(KeyCode.Backspace))
                goBackToGamemodeSelection();
        }
    }
}
