using CDOverhaul.Gameplay;
using HarmonyLib;
using System.Collections.Generic;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(AISwordsmanController))]
    internal static class AISwordsmanController_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("FixedUpdate")]
        private static void FixedUpdate_Postfix(AISwordsmanController __instance)
        {
            if (!OverhaulMod.IsCoreCreated)
            {
                return;
            }

            List<FirstPersonMoverExtention> list = FirstPersonMoverExtention.GetExtentions(__instance.GetComponent<FirstPersonMover>());
            if (list.IsNullOrEmpty())
            {
                return;
            }
            foreach (FirstPersonMoverExtention ext in list)
            {
                ext.OnAIUpdate(__instance);
            }
        }
    }
}
