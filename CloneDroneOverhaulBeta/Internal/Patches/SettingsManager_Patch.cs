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

        [HarmonyPostfix]
        [HarmonyPatch("GetForceRelayConnection")]
        private static void GetForceRelayConnection_Postfix(ref bool __result)
        {
            __result = OverhaulCore.IsRelayConnectionEnabled;
        }
    }
}
