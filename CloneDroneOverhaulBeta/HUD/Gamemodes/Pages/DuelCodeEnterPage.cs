using CDOverhaul.CustomMultiplayer;
using UnityEngine;

namespace CDOverhaul.HUD.Gamemodes
{
    public class DuelCodeEnterPage : LBSCodeEnterPage
    {
        public override void Initialize(OverhaulGamemodesUIFullscreenWindow fullscreenWindow)
        {
            base.Initialize(fullscreenWindow);

            _ = OverhaulEventsController.AddEventListener(CustomMultiplayerController.LobbyJoinFailEvent, onFailedToJoinLobby);
        }

        protected override void OnDisposed()
        {
            OverhaulEventsController.RemoveEventListener(CustomMultiplayerController.LobbyJoinFailEvent, onFailedToJoinLobby);
        }

        protected override async void TryJoinLobby()
        {
            GameRequestType gameType = GameRequestType.DuelInviteCodeJoin;
            GameRequest gameRequest = new GameRequest
            {
                GameType = gameType,
                InviteCodeToJoin = GetInputFieldValue(),
                Exclusivity = await MultiplayerMatchmakingManager.GetExclusivePlatformAsync()
            };

            MultiplayerMatchmakingManager.Instance.ResetMatchmakingStartTime();
            CustomMatchmakerClientAPI.Instance.FindMatch(new CustomMatchmakeRequest
            {
                GameRequest = gameRequest
            }, delegate (CustomMatchmakeResult result)
            {
                MultiplayerMatchmakingManager.Instance.ConnectToExternalMatchmakeResult(result, gameRequest);
                FullscreenWindow.GamemodesUI.Hide();
            }, delegate (CustomMatchmakerError error)
            {
                GoButton.interactable = true;
                OverhaulDialogues.CreateDialogue("Cannot enter the lobby", error.Type == CustomMatchmakerErrorType.InviteMatchFull ? "The lobby you want to connect to is full." : "No lobby found.", 0f, new Vector2(315, 160), null);
            });
        }

        private void onFailedToJoinLobby()
        {
            SetInputFieldInteractable(true);
        }
    }
}
