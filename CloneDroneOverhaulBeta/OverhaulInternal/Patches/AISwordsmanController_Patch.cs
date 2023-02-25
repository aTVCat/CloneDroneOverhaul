using Bolt;
using CDOverhaul.Gameplay;
using HarmonyLib;
using System.Collections.Generic;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(AISwordsmanController))]
    internal static class AISwordsmanController_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("FixedUpdate")]
        private static bool FixedUpdate_Prefix(AISwordsmanController __instance)
        {
            if (!OverhaulMod.IsCoreCreated)
            {
                return true;
            }

            FirstPersonMoverExpansionBase[] expansionBases = __instance.GetComponents<FirstPersonMoverExpansionBase>();
            foreach (FirstPersonMoverExpansionBase b in expansionBases)
            {
                b.OnPreAIUpdate(__instance, out bool continueEx);
                if (!continueEx)
                {
                    expansionBases = null;
                    return false;
                }
            }
            expansionBases = null;

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("FixedUpdate")]
        private static void FixedUpdate_Postfix(AISwordsmanController __instance)
        {
            if (!OverhaulMod.IsCoreCreated)
            {
                return;
            }

            FirstPersonMoverExpansionBase[] expansionBases = __instance.GetComponents<FirstPersonMoverExpansionBase>();
            foreach (FirstPersonMoverExpansionBase b in expansionBases)
            {
                b.OnPostAIUpdate(__instance);
            }
            expansionBases = null;

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
