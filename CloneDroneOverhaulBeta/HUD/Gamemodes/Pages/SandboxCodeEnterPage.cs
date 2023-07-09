using CDOverhaul.CustomMultiplayer;
using Steamworks;

namespace CDOverhaul.HUD.Gamemodes
{
    public class SandboxCodeEnterPage : CodeEnterPage
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
            FullscreenWindow.GamemodesUI.Hide();

            if (!ulong.TryParse(GetInputFieldValue(), out ulong lobbyId))
            {
                SetInputFieldInteractable(true);
                return;
            }
            CustomMultiplayerController.Instance.JoinLobby((CSteamID)lobbyId);
        }

        private void onFailedToJoinLobby()
        {
            SetInputFieldInteractable(true);
        }
    }
}
