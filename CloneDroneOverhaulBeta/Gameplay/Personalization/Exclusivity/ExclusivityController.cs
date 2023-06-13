

namespace CDOverhaul
{
    internal static class ExclusivityController
    {
        [OverhaulSettingAttribute("Player.VanillaAdditions.FavColorOffset", 0, false/*!OverhaulVersion.IsDebugBuild*/)]
        public static int ColorOffset;

        public const string OnLoginSuccessEventString = "PlayfabLoginSuccessOverhaulMod";

        private static string _playfabId;
        private static bool _hasInitialized;

        public static void Initialize()
        {
            if (_hasInitialized)
            {
                DelegateScheduler.Instance.Schedule(scheduledOnLogin, 0.1f);
                return;
            }

            _ = OverhaulEventsController.AddEventListener(GlobalEvents.PlayfabLoginSuccess, onLogin, true);
            _hasInitialized = true;
        }
        private static void onLogin()
        {
            _playfabId = GetLocalPlayFabID();
            OverhaulEventsController.RemoveEventListener(GlobalEvents.PlayfabLoginSuccess, onLogin, true);
            OverhaulEventsController.DispatchEvent(OnLoginSuccessEventString);
            ExclusiveColorsController.OnGotPlayfabID(_playfabId);
        }
        private static void scheduledOnLogin()
        {
            if (string.IsNullOrEmpty(_playfabId))
                _playfabId = GetLocalPlayFabID();

            ExclusiveColorsController.OnGotPlayfabID(_playfabId);
            OverhaulEventsController.DispatchEvent(OnLoginSuccessEventString);
        }

        public static string GetLocalPlayFabID()
        {
            string result = MultiplayerLoginManager.Instance.GetLocalPlayFabID();
            if (result == null)
                result = string.Empty;

            return result;
        }

        public static long GetDiscordID()
        {
            return OverhaulDiscordController.Instance == null || OverhaulDiscordController.Instance.IsDisposedOrDestroyed() || !OverhaulDiscordController.SuccessfulInitialization
                ? 0
                : OverhaulDiscordController.Instance.UserID;
        }

        public static bool HasPlayfabID() => !string.IsNullOrEmpty(GetLocalPlayFabID());
        public static bool IsDeveloper() => HasPlayfabID() && GetLocalPlayFabID() == "883CC7F4CA3155A3";
    }
}
