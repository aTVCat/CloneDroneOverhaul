using HarmonyLib;

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

            OverhaulCharacterExpansion[] expansionBases = __instance.GetComponents<OverhaulCharacterExpansion>();
            foreach (OverhaulCharacterExpansion b in expansionBases)
            {
                b.OnPreAIUpdate(__instance, out bool continueEx);
                if (!continueEx)
                {
                    expansionBases = null;
                    return false;
                }
            }

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

            OverhaulCharacterExpansion[] expansionBases = __instance.GetComponents<OverhaulCharacterExpansion>();
            foreach (OverhaulCharacterExpansion b in expansionBases)
            {
                b.OnPostAIUpdate(__instance);
            }
        }
    }
}
