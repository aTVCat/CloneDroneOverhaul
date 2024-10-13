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
            UseKeyTriggerManager.PatchKeyboardHint(__instance.UpgradePressEPrefab.transform);
        }
    }
}
