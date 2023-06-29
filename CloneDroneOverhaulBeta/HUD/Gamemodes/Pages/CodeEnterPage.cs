using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class CodeEnterPage : FullscreenWindowPageBase
    {
        private Button m_GoButton;
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
        }

        public void OnGoClick()
        {
            tryJoinLobby();
        }

        private async void tryJoinLobby()
        {
            GameRequestType gameType = GameRequestType.BattleRoyaleInviteCodeJoin;
            GameRequest gameRequest = new GameRequest();
            gameRequest.GameType = gameType;
            gameRequest.InviteCodeToJoin = m_CodeField.text;
            gameRequest.Exclusivity = await MultiplayerMatchmakingManager.GetExclusivePlatformAsync();

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
            });
        }
    }
}
