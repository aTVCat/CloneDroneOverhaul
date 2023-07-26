using CDOverhaul.CustomMultiplayer;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulGamemodeManager
    {
        private const string GunModID = "ee32ba1b-8c92-4f50-bdf4-400a14da829e";

        public static bool IsMultiplayerSandbox() => CustomMultiplayerController.Instance && CustomMultiplayerController.FullInitialization;

        public static bool SupportsPersonalization() => true;
        public static bool SupportsOutfits() => !OverhaulVersion.IsVersion2 && SupportsPersonalization();
        public static bool SupportsBowSkins() => !OverhaulMod.IsModEnabled(GunModID) || GameModeManager.IsMultiplayer();

        public static bool ShouldShowRoomCodePanel() => MultiplayerMatchmakingManager.Instance != null && MultiplayerMatchmakingManager.Instance.IsLocalPlayerHostOfCustomMatch();
        public static string GetPrivateRoomCode() => !ShouldShowRoomCodePanel() ? string.Empty : MultiplayerMatchmakingManager.Instance.GetLastInviteCode();
        public static void CopyPrivateRoomCode()
        {
            if (!ShouldShowRoomCodePanel())
                return;

            GetPrivateRoomCode().CopyToClipboard();
        }
    }
}
