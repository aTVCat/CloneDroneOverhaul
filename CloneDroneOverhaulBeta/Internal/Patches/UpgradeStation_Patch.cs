using CDOverhaul.Gameplay.Overmodes;
using CDOverhaul.Gameplay.QualityOfLife;
using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(UpgradeStation))]
    internal static class UpgradeStation_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("notActivatable")]
        private static void notActivatable_Postfix(UpgradeStation __instance, ref bool __result)
        {
            if (OvermodesController.IsOvermode() && __result)
            {
                __result = !OvermodesController.GetOvermode().AllowUpgradeBots();
            }
        }
    }
}