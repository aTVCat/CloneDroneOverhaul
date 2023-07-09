using Steamworks;
using UnityEngine;

namespace CDOverhaul.CustomMultiplayer
{
    public class CustomMultiplayerController : OverhaulController
    {
        public const string LobbyJoinFailEvent = "OverhaulMultiplayer.LobbyJoinFail";

        public static CustomMultiplayerController Instance
        {
            get;
            private set;
        }

        public static CustomMultiplayerLobby Lobby
        {
            get;
            private set;
        }

        public static  CustomMultiplayerWorld World
        {
            get;
            private set;
        }

        public static bool FullInitialization => Instance && Lobby != null && World != null;

        public override void Initialize()
        {
            Instance = this;
        }

        private void FixedUpdate()
        {
            CustomMultiplayerPacketManagement.FixedFrames++;
        }

        private void LateUpdate()
        {
            if (!FullInitialization)
                return;

            CustomMultiplayerPacketManagement.HandleIncomingPackets();

            if (Input.GetKeyDown(KeyCode.I))
                Lobby.LobbyID.ToString().CopyToClipboard();
        }

        /// <summary>
        /// Create a lobby
        /// </summary>
        public void CreateLobby(bool isPublic = false, int maxPlayers = 10)
        {
            SteamAPICall_t apiCall = SteamMatchmaking.CreateLobby(isPublic ? ELobbyType.k_ELobbyTypePublic : ELobbyType.k_ELobbyTypeFriendsOnly, maxPlayers);
            CallResult<LobbyCreated_t> apiCallResult = CallResult<LobbyCreated_t>.Create(delegate (LobbyCreated_t t, bool a)
            {
                if (t.m_eResult != EResult.k_EResultOK)
                {
                    Debug.LogWarning("[CDO_MS] Cannot create lobby!");
                    OverhaulEventsController.DispatchEvent(LobbyJoinFailEvent);
                    return;
                }

                InitializeLobbyData((CSteamID)t.m_ulSteamIDLobby);
                InitializeWorld();
                Debug.Log("[CDO_MS] Created new lobby!");
            });
            apiCallResult.Set(apiCall);
        }

        /// <summary>
        /// Join specific lobby
        /// </summary>
        /// <param name="lobbyId"></param>
        public void JoinLobby(CSteamID lobbyId)
        {
            SteamAPICall_t apiCall = SteamMatchmaking.JoinLobby(lobbyId);
            CallResult<LobbyEnter_t> apiCallResult = CallResult<LobbyEnter_t>.Create(delegate (LobbyEnter_t t, bool a)
            {
                if (t.m_ulSteamIDLobby == (ulong)CSteamID.Nil || (EChatRoomEnterResponse)t.m_EChatRoomEnterResponse != EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
                {
                    Debug.LogWarning("[CDO_MS] Cannot join the lobby!");
                    OverhaulEventsController.DispatchEvent(LobbyJoinFailEvent);
                    return;
                }

                string version = SteamMatchmaking.GetLobbyData((CSteamID)t.m_ulSteamIDLobby, "modVer");
                if (version != OverhaulVersion.ModVersion.ToString())
                {
                    LeaveLobby((CSteamID)t.m_ulSteamIDLobby);
                    OverhaulFullscreenDialogueWindow.ShowOkWindow("Version mismatch!", string.Format("The server is on {0} version of the mod (You're on {1})", version, OverhaulVersion.ModVersion.ToString()), 300, 175, OverhaulFullscreenDialogueWindow.IconType.Warn);
                    return;
                }

                InitializeLobbyData((CSteamID)t.m_ulSteamIDLobby);
                InitializeWorld();
                Debug.Log("[CDO_MS] Joined the lobby!");
            });
            apiCallResult.Set(apiCall);
        }

        public void LeaveLobby(CSteamID lobbyId)
        {
            SteamMatchmaking.LeaveLobby(lobbyId);
            ResetAll();
        }

        public void InitializeLobbyData(CSteamID lobbyId)
        {
            if (Lobby != null)
                return;

            Lobby = new CustomMultiplayerLobby
            {
                LobbyID = lobbyId
            };

            _ = SteamMatchmaking.SetLobbyData(Lobby.LobbyID, "modVer", OverhaulVersion.ModVersion.ToString());
        }

        public void ResetLobby()
        {
            Lobby = null;
        }

        public void InitializeWorld()
        {
            World = new CustomMultiplayerWorld();
            World.StartGameInUsualWorld();
        }

        public void ResetWorld()
        {
            World = null;
        }

        public void ResetAll()
        {
            ResetLobby();
            ResetWorld();
        }
    }
}
