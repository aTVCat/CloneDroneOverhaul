using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class CodeEnterPage : FullscreenWindowPageBase
    {
        private Button m_GoButton;
        private Button m_BackButton;
        private InputField m_CodeField;

        public override Vector2 GetWindowSize() => OverhaulGamemodesUIFullscreenWindow.CodeEnterWindowSize;
        public override bool AllowPressingBackspace() => false;

        public override void Initialize(OverhaulGamemodesUIFullscreenWindow fullscreenWindow)
        {
            base.Initialize(fullscreenWindow);

            m_CodeField = MyModdedObject.GetObject<InputField>(0);
            m_CodeField.text = string.Empty;
            m_GoButton = MyModdedObject.GetObject<Button>(1);
            m_GoButton.onClick.AddListener(OnGoClick);
            m_BackButton = MyModdedObject.GetObject<Button>(2);
            m_BackButton.onClick.AddListener(OnBackClick);
        }

        public void OnGoClick()
        {
            m_GoButton.interactable = false;
            tryJoinLobby();
        }

        public void OnBackClick()
        {
            FullscreenWindow.Hide();
        }

        private async void tryJoinLobby()
        {
            GameRequestType gameType = GameRequestType.BattleRoyaleInviteCodeJoin;
            GameRequest gameRequest = new GameRequest
            {
                GameType = gameType,
                InviteCodeToJoin = m_CodeField.text,
                Exclusivity = await MultiplayerMatchmakingManager.GetExclusivePlatformAsync()
            };

            MultiplayerMatchmakingManager.Instance.ResetMatchmakingStartTime();
            Singleton<CustomMatchmakerClientAPI>.Instance.FindMatch(new CustomMatchmakeRequest
            {
                GameRequest = gameRequest
            }, delegate (CustomMatchmakeResult result)
            {
                BoltGlobalEventListenerSingleton<MultiplayerMatchmakingManager>.Instance.ConnectToExternalMatchmakeResult(result, gameRequest);
                FullscreenWindow.GamemodesUI.Hide();
            }, delegate (CustomMatchmakerError error)
            {
                m_GoButton.interactable = true;
                OverhaulDialogues.CreateDialogue("Cannot enter the lobby", error.Type == CustomMatchmakerErrorType.InviteMatchFull ? "The lobby you want to connect to is full." : "No lobby found.", 0f, new Vector2(315, 160), null);
            });
        }
    }
}
