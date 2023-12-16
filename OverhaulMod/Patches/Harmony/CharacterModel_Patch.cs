using HarmonyLib;
using UnityEngine;

namespace OverhaulMod.Patches.Harmony
{
    [HarmonyPatch(typeof(CharacterModel))]
    internal static class CharacterModel_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("OverridePatternColor")]
        private static void OverridePatternColor_Prefix(CharacterModel __instance, ref Color newColor, bool forceMultiplayerHSBReplacement = false)
        {
            FirstPersonMover firstPersonMover = __instance.GetOwner();
            if (!firstPersonMover)
                return;

            ModExclusiveContentManager.Instance.GetOverrideRobotColor(firstPersonMover, newColor, out Color toReplace);
            newColor = toReplace;
        }
    }
}
