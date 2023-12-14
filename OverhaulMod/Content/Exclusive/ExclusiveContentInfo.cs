using Steamworks;

namespace OverhaulMod.Content
{
    public class ExclusiveContentInfo
    {
        public ExclusiveContentType ContentType;

        public ulong SteamID;

        public string PlayFabID;

        public ExclusiveContentBase Content;

        public bool HasSteamID()
        {
            return SteamID != default;
        }

        public bool HasPlayFabID()
        {
            return !string.IsNullOrEmpty(PlayFabID);
        }

        public bool IsAvailableToLocalUser()
        {
            return (HasSteamID() && SteamManager.Instance.Initialized && SteamID == (ulong)SteamUser.GetSteamID())
|| (HasPlayFabID() && MultiplayerLoginManager.Instance.GetLocalPlayFabID() == PlayFabID);
        }
    }
}
