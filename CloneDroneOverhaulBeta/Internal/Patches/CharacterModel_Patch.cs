using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(CharacterModel))]
    internal static class CharacterModel_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("OverridePatternColor")]
        private static void OverridePatternColor_Prefix(CharacterModel __instance, ref Color newColor, bool forceMultiplayerHSBReplacement = false)
        {
            if (!OverhaulMod.IsCoreCreated || !GameModeManager.IsMultiplayer())
            {
                return;
            }

            ExclusiveRolesController.TryApplyExclusivityOnRobot(__instance.GetOwner(), newColor, out Color toReplace);
            newColor = toReplace;
        }
    }
}
