using Steamworks;

namespace OverhaulMod.Content
{
    public class ExclusiveContentInfo
    {
        public ExclusiveContentType ContentType;

        public ulong SteamID;

        public string PlayFabID;

        public ExclusiveContentBase Content;

        public string Name;

        public void VerifyFields()
        {
            if (Content != null && Content.InfoReference == null)
            {
                Content.InfoReference = this;
            }
        }

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
            return (Content != null && Content.ForceUnlock)
|| (HasSteamID() && SteamManager.Instance.Initialized && SteamID == (ulong)SteamUser.GetSteamID()) || (HasPlayFabID() && MultiplayerLoginManager.Instance.GetLocalPlayFabID() == PlayFabID);
        }
    }
}
