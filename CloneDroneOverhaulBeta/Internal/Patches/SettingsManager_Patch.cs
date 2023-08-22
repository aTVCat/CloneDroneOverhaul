using CDOverhaul.Visuals;
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
            OverhaulRenderManager.RefreshLightsCount();
        }

        [HarmonyPostfix]
        [HarmonyPatch("GetForceRelayConnection")]
        private static void GetForceRelayConnection_Postfix(ref bool __result)
        {
            __result = OverhaulCore.IsRelayConnectionEnabled;
        }
    }
}
