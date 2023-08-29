using CDOverhaul.CustomMultiplayer;
using Steamworks;

namespace CDOverhaul.HUD.Gamemodes
{
    public class SandboxCodeEnterPage : LBSCodeEnterPage
    {
        public override void Initialize(OverhaulGamemodesUIFullscreenWindow fullscreenWindow)
        {
            base.Initialize(fullscreenWindow);

            OverhaulEventsController.AddEventListener(OverhaulMultiplayerManager.LobbyJoinFailEvent, onFailedToJoinLobby);
        }

        protected override void OnDisposed()
        {
            OverhaulEventsController.RemoveEventListener(OverhaulMultiplayerManager.LobbyJoinFailEvent, onFailedToJoinLobby);
        }

        protected override async void TryJoinLobby()
        {
            FullscreenWindow.GamemodesUI.Hide();

            if (!ulong.TryParse(GetInputFieldValue(), out ulong lobbyId))
            {
                SetInputFieldInteractable(true);
                return;
            }
            OverhaulMultiplayerManager.Instance.JoinLobby((CSteamID)lobbyId);
        }

        private void onFailedToJoinLobby()
        {
            SetInputFieldInteractable(true);
        }
    }
}
