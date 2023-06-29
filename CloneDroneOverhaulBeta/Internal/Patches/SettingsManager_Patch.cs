using CDOverhaul.Graphics;
using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(SettingsManager))]
    internal static class SettingsManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("SetQuality")]
        private static void SetQuality_Postfix()
        {
            OverhaulGraphicsController.RefreshLightsCount();
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetVsyncOn")]
        private static void SetVsyncOn_Prefix(bool value)
        {
            if (OverhaulGraphicsController.DisallowChangeFPSLimit)
                return;

            if (value)
            {
                SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Settings.Target framerate", true), 2, false);
            }
            else
            {
                SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Settings.Target framerate", true), 0, false);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("GetForceRelayConnection")]
        private static void GetForceRelayConnection_Postfix(ref bool __result)
        {
            __result = OverhaulCore.IsRelayConnectionEnabled;
        }
    }
}
