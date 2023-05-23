using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulGamemodeManager
    {
        private const string GunModID = "ee32ba1b-8c92-4f50-bdf4-400a14da829e";

        /// <summary>
        /// Check if we can see accessories on robots
        /// </summary>
        /// <returns></returns>
        public static bool SupportsPersonalization()
        {
            return true;
        }

        public static bool SupportsOutfits()
        {
            return !OverhaulVersion.IsUpdate2Hotfix && SupportsPersonalization();
        }

        public static bool SupportsBowSkins()
        {
            bool isEnabled = OverhaulMod.IsModEnabled(GunModID);
            return !isEnabled || (isEnabled && GameModeManager.IsMultiplayer());
        }

        public static bool ShouldShowRoomCodePanel()
        {
            return MultiplayerMatchmakingManager.Instance != null && MultiplayerMatchmakingManager.Instance.IsLocalPlayerHostOfCustomMatch();
        }

        public static string GetPrivateRoomCode()
        {
            return !ShouldShowRoomCodePanel() ? string.Empty : MultiplayerMatchmakingManager.Instance.GetLastInviteCode();
        }

        public static void CopyPrivateRoomCode()
        {
            if (!ShouldShowRoomCodePanel())
            {
                return;
            }

            TextEditor edit = new TextEditor
            {
                text = GetPrivateRoomCode()
            };
            edit.SelectAll();
            edit.Copy();
        }
    }
}
