

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
                ExclusiveRolesController.OnGotPlayfabID(_playfabId);
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

        public static string GetLocalPlayfabID()
        {
            return MultiplayerLoginManager.Instance.GetLocalPlayFabID();
        }

        public static bool HasPlayfabID()
        {
            return !string.IsNullOrEmpty(GetLocalPlayfabID());
        }
    }
}
