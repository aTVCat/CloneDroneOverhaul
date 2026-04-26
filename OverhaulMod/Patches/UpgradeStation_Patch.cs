using HarmonyLib;
using OverhaulMod.Engine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(UpgradeStation))]
    internal static class UpgradeStation_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(UpgradeStation.Start))]
        private static void Start_Prefix(UpgradeStation __instance)
        {
            if (!ModCore.IsActive()) return;

            UseKeyTriggerManager.PatchKeyboardHint(__instance.UpgradePressEPrefab.transform);
        }
    }
}
