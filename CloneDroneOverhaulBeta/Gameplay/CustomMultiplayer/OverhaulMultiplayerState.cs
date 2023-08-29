namespace CDOverhaul.CustomMultiplayer
{
    public static class OverhaulMultiplayerState
    {
        public static bool IsMultiplayerMode
        {
            get
            {
                return OverhaulMultiplayerManager.Mode != EOverhaulMultiplayerMode.None;
            }
        }

        public static bool IsHost
        {
            get
            {
                if (!IsMultiplayerMode)
                    return true;

                OverhaulMultiplayerLobby lobby = OverhaulMultiplayerManager.Lobby;
                return lobby.OwnerUserID == lobby.LocalUserID;
            }
        }
    }
}
