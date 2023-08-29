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
            if (!OverhaulMod.IsModInitialized)
                return;

            ExclusiveColorsSystem.FindAndApplyExclusiveColor(__instance.GetOwner(), newColor, out Color toReplace);
            newColor = toReplace;
        }

        [HarmonyPrefix]
        [HarmonyPatch("OnFootDown")]
        private static bool OnFootDown_Prefix(CharacterModel __instance)
        {
            if (!OverhaulMod.IsModInitialized)
                return true;

            if (!AudioManager.Instance.ShouldPlayLowPrioritySound() || !AudioManager.Instance.ShouldPlaySound(__instance.transform.position, false))
                return false;

            bool isHeavy = __instance.IsHeavyRobot(out bool lowPitch, out _, out bool dontPlaySound);
            if (dontPlaySound)
                return false;

            if (isHeavy && OverhaulAudioLibrary.HasLoadedSounds)
            {
                _ = AudioManager.Instance.PlayClipAtPosition(OverhaulAudioLibrary.HeavyRobotFootsteps, __instance.transform.position, 0f, false, lowPitch ? 0.35f : 0.16f, lowPitch ? 0.85f : 0.98f, 0f);
                return false;
            }

            return true;
        }
    }
}
