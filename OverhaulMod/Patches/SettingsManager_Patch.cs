using HarmonyLib;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(SettingsManager))]
    internal static class SettingsManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(SettingsManager.GetForceRelayConnection))]
        private static void GetForceRelayConnection_Postfix(SettingsManager __instance, ref bool __result)
        {
            if (!ModCore.IsActive()) return;

            __result = __instance._data.ForceRelayConnection;
        }
    }
}