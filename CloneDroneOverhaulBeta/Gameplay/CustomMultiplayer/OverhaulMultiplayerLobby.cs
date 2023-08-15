using Steamworks;

namespace CDOverhaul.CustomMultiplayer
{
    public class OverhaulMultiplayerLobby
    {
        public bool IsDirty;

        public CSteamID LobbyID
        {
            get;
            set;
        }

        private CSteamID m_OwnerUserID = CSteamID.Nil;
        /// <summary>
        /// The SteamID of lobby owner
        /// </summary>
        public CSteamID OwnerUserID
        {
            get
            {
                if (IsDirty)
                    Refresh();

                if (m_OwnerUserID == CSteamID.Nil)
                    m_OwnerUserID = SteamMatchmaking.GetLobbyOwner(LobbyID);

                return m_OwnerUserID;
            }
        }

        private CSteamID m_LocalUserID = CSteamID.Nil;
        /// <summary>
        /// The SteamID of local user
        /// </summary>
        public CSteamID LocalUserID
        {
            get
            {
                if (IsDirty)
                    Refresh();

                if (m_LocalUserID == CSteamID.Nil)
                    m_LocalUserID = SteamUser.GetSteamID();

                return m_LocalUserID;
            }
        }

        private int m_MemberCount;
        public int MemberCount
        {
            get
            {
                if (IsDirty)
                    Refresh();

                if (m_MemberCount == 0)
                    m_MemberCount = SteamMatchmaking.GetNumLobbyMembers(LobbyID);

                return m_MemberCount;
            }
        }

        private CSteamID[] m_Members;
        public CSteamID[] Members
        {
            get
            {
                if (IsDirty)
                    Refresh();

                if (m_Members == null)
                    m_Members = getMembers();

                return m_Members;
            }
        }

        public OverhaulMultiplayerLobby(CSteamID lobbyID)
        {
            LobbyID = lobbyID;
        }

        public void Refresh()
        {
            m_OwnerUserID = SteamMatchmaking.GetLobbyOwner(LobbyID);
            m_MemberCount = SteamMatchmaking.GetNumLobbyMembers(LobbyID);
            m_LocalUserID = SteamUser.GetSteamID();
            m_Members = getMembers();
            IsDirty = false;
        }

        private CSteamID[] getMembers()
        {
            CSteamID[] result = new CSteamID[MemberCount];
            int index = 0;
            do
            {
                result[index] = SteamMatchmaking.GetLobbyMemberByIndex(LobbyID, index);
                index++;
            } while (index < result.Length);
            return result;
        }
    }
}
