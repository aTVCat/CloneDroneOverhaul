

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

            _ = OverhaulEventManager.AddEventListener(GlobalEvents.PlayfabLoginSuccess, onLogin, true);
            _hasInitialized = true;
        }
        private static void onLogin()
        {
            _playfabId = GetLocalPlayfabID();
            OverhaulEventManager.RemoveEventListener(GlobalEvents.PlayfabLoginSuccess, onLogin, true);
            OverhaulEventManager.DispatchEvent(OnLoginSuccessEventString);
            ExclusiveRolesController.OnGotPlayfabID(_playfabId);
        }
        private static void scheduledOnLogin()
        {
            if (string.IsNullOrEmpty(_playfabId))
            {
                _playfabId = GetLocalPlayfabID();
            }

            ExclusiveRolesController.OnGotPlayfabID(_playfabId);
            OverhaulEventManager.DispatchEvent(OnLoginSuccessEventString);
        }

        public static string GetLocalPlayfabID()
        {
            string result = MultiplayerLoginManager.Instance.GetLocalPlayFabID();
            if(result == null)
            {
                result = string.Empty;
            }
            return result;
        }

        public static bool HasPlayfabID()
        {
            return !string.IsNullOrEmpty(GetLocalPlayfabID());
        }
    }
}
