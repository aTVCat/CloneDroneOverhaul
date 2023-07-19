using Steamworks;

namespace CDOverhaul
{
    internal static class OverhaulPlayerIdentifier
    {
        public const string OnLoginSuccessEventString = "PlayfabLoginSuccessOverhaulMod";

        private static string s_SteamID;
        private static string s_PlayFabID;
        private static bool s_HasInitialized;

        public static void Initialize()
        {
            if (s_HasInitialized)
            {
                DelegateScheduler.Instance.Schedule(scheduledOnLogin, 0.1f);
                return;
            }

            _ = OverhaulEventsController.AddEventListener(GlobalEvents.PlayfabLoginSuccess, onLogin, true);
            s_HasInitialized = true;
        }
        private static void onLogin()
        {
            s_PlayFabID = GetLocalPlayFabID();
            OverhaulEventsController.RemoveEventListener(GlobalEvents.PlayfabLoginSuccess, onLogin, true);
            OverhaulEventsController.DispatchEvent(OnLoginSuccessEventString);
        }
        private static void scheduledOnLogin()
        {
            if (string.IsNullOrEmpty(s_PlayFabID))
                s_PlayFabID = GetLocalPlayFabID();

            OverhaulEventsController.DispatchEvent(OnLoginSuccessEventString);
        }

        public static string GetLocalPlayFabID()
        {
            string result = MultiplayerLoginManager.Instance.GetLocalPlayFabID();
            if (result == null)
                result = string.Empty;

            return result;
        }

        public static string GetLocalSteamID()
        {
            if (!SteamAPI.IsSteamRunning() || !SteamManager.Instance.Initialized)
                return string.Empty;

            if (string.IsNullOrEmpty(s_SteamID))
                s_SteamID = SteamUser.GetSteamID().ToString();

            return s_SteamID;
        }

        public static long GetDiscordID()
        {
            return !OverhaulDiscordController.HasInitialized
                ? 0
                : OverhaulDiscordController.Instance.UserID;
        }

        public static bool HasPlayfabID() => !string.IsNullOrEmpty(GetLocalPlayFabID());
        public static bool IsDeveloper() => HasPlayfabID() && GetLocalPlayFabID() == "883CC7F4CA3155A3";
    }
}
