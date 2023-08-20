using CDOverhaul.Graphics;
using HarmonyLib;
using ModLibrary;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(AmplifyColorSwapper))]
    internal static class AmplifyColorSwapper_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("refreshLutValues")]
        private static void refreshLutValues_Postfix(AmplifyColorSwapper __instance)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            OverhaulGraphicsManager graphicsManager = OverhaulGraphicsManager.GetController<OverhaulGraphicsManager>();
            if (!graphicsManager || !graphicsManager.amplifyColorOverhaul || !__instance._amplifyColorBase)
                return;

            graphicsManager.amplifyColorOverhaul.PatchCamera(__instance._amplifyColorBase.GetComponent<Camera>());
        }
    }
}
