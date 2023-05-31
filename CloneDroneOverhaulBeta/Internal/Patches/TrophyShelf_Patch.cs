using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(TrophyShelf))]
    internal static class TrophyShelf_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("refreshTrophies")]
        private static void refreshTrophies_Postfix(TrophyShelf __instance)
        {
            if (!OverhaulMod.IsModInitialized || __instance.TrophyHolder == null || __instance.TrophyHolder.childCount == 0)
                return;

            Light[] l = __instance.TrophyHolder.GetComponentsInChildren<Light>();
            if (!l.IsNullOrEmpty())
                foreach (Light l2 in l)
                    l2.gameObject.SetActive(false);
        }
    }
}
