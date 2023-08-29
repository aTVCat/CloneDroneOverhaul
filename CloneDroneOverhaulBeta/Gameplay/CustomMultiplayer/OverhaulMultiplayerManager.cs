using Steamworks;
using UnityEngine;

namespace CDOverhaul.CustomMultiplayer
{
    public class OverhaulMultiplayerManager : OverhaulManager<OverhaulMultiplayerManager>
    {
        public const string LobbyJoinFailEvent = "OverhaulMultiplayer.LobbyJoinFail";

        public static OverhaulMultiplayerManager Instance;

        public static OverhaulMultiplayerLobby Lobby
        {
            get;
            private set;
        }

        public static EOverhaulMultiplayerMode Mode
        {
            get;
            private set;
        }

        public static bool IsStartingMultiplayer
        {
            get;
            private set;
        }

        public static bool FullInitialization => Instance && Mode != EOverhaulMultiplayerMode.None && Lobby != null;

        public override void Initialize()
        {
            Instance = this;
        }

        private void LateUpdate()
        {
            if (!FullInitialization)
                return;

            if (Input.GetKeyDown(KeyCode.I))
                Lobby.LobbyID.ToString().CopyToClipboard();
        }

        /// <summary>
        /// Create a lobby
        /// </summary>
        public void StartMultiplayer(EOverhaulMultiplayerMode mode, bool isPublic = false, int maxPlayers = 10)
        {
            IsStartingMultiplayer = true;
            SteamAPICall_t apiCall = SteamMatchmaking.CreateLobby(isPublic ? ELobbyType.k_ELobbyTypePublic : ELobbyType.k_ELobbyTypeFriendsOnly, maxPlayers);
            CallResult<LobbyCreated_t> apiCallResult = CallResult<LobbyCreated_t>.Create(delegate (LobbyCreated_t t, bool a)
            {
                IsStartingMultiplayer = false;
                if (t.m_eResult != EResult.k_EResultOK)
                {
                    Debug.LogWarning("[CDO_MS] Cannot create lobby!");
                    OverhaulEventsController.DispatchEvent(LobbyJoinFailEvent);
                    return;
                }

                InitializeLobbyData((CSteamID)t.m_ulSteamIDLobby);
                OverhaulFullscreenDialogueWindow.ShowOkWindow("Multiplayer server started!", "Now your friends can join the game!", 300, 150, OverhaulFullscreenDialogueWindow.IconType.None);
            });
            apiCallResult.Set(apiCall);
        }

        /// <summary>
        /// Join specific lobby
        /// </summary>
        /// <param name="lobbyId"></param>
        public void JoinLobby(CSteamID lobbyId)
        {
            IsStartingMultiplayer = true;
            SteamAPICall_t apiCall = SteamMatchmaking.JoinLobby(lobbyId);
            CallResult<LobbyEnter_t> apiCallResult = CallResult<LobbyEnter_t>.Create(delegate (LobbyEnter_t t, bool a)
            {
                IsStartingMultiplayer = false;
                if ((EChatRoomEnterResponse)t.m_EChatRoomEnterResponse != EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
                {
                    Debug.LogWarning("[CDO_MS] Cannot join the lobby!");
                    OverhaulEventsController.DispatchEvent(LobbyJoinFailEvent);
                    return;
                }

                // Game version check
                string version = SteamMatchmaking.GetLobbyData((CSteamID)t.m_ulSteamIDLobby, "modVer");
                if (version != OverhaulVersion.modVersion.ToString())
                {
                    LeaveLobby((CSteamID)t.m_ulSteamIDLobby);
                    OverhaulFullscreenDialogueWindow.ShowOkWindow("Version mismatch!", string.Format("The server uses different version of the mod (Server: {0}, You: {1})", version, OverhaulVersion.modVersion.ToString()), 300, 175, OverhaulFullscreenDialogueWindow.IconType.Warn);
                    return;
                }

                InitializeLobbyData((CSteamID)t.m_ulSteamIDLobby);
                OverhaulFullscreenDialogueWindow.ShowOkWindow("You've joined the lobby!", "", 300, 150, OverhaulFullscreenDialogueWindow.IconType.None);
            });
            apiCallResult.Set(apiCall);
        }

        public void LeaveLobby(CSteamID lobbyId)
        {
            Lobby = null;
            Mode = EOverhaulMultiplayerMode.None;
            SteamMatchmaking.LeaveLobby(lobbyId);
        }

        public void InitializeLobbyData(CSteamID lobbyId)
        {
            if (Lobby != null)
                return;

            Lobby = new OverhaulMultiplayerLobby(lobbyId);
            if (OverhaulMultiplayerState.IsHost)
            {
                _ = SteamMatchmaking.SetLobbyData(Lobby.LobbyID, "modVer", OverhaulVersion.modVersion.ToString());
                _ = SteamMatchmaking.SetLobbyData(Lobby.LobbyID, "mode", Mode.ToString());
            }
        }
    }
}
