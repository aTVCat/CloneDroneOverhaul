

namespace CDOverhaul
{
    internal class ExclusivityController
    {
        public const string OnLoginSuccessEventString = "PlayfabLoginSuccessOverhaulMod";

        private static string _playfabId;
        private static bool _hasInitialized;

        public static void Initialize()
        {
            if (_hasInitialized)
            {
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
