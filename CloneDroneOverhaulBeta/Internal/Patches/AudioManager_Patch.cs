using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(AudioManager))]
    internal static class AudioManager_Patch
    {
        private static readonly string[] s_KickSounds = new string[]
        {
            "048982865-critical-damage",
            "048982866-jab-damage",
            "048982871-punch-damage"
        };

        private static float m_TimeToAllowKickSounds = 0f;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AudioManager), "PlayClipAtTransform", new System.Type[] { typeof(AudioClipDefinition[]), typeof(Transform), typeof(float), typeof(bool), typeof(float), typeof(float), typeof(float) })]
        private static bool PlayClipAtTransform_Prefix(AudioManager __instance, AudioClipDefinition[] clips, Transform targetTransform, float delay = 0f, bool loop = false, float maxDistanceMultiplier = 1f, float pitchMultiplier = 1f, float startTime = 0f)
        {
            if (!OverhaulMod.IsModInitialized)
                return true;

            if (!clips.IsNullOrEmpty())
            {
                foreach (AudioClipDefinition clip in clips)
                {
                    if (s_KickSounds.Contains(clip.Clip.name))
                    {
                        if (Time.unscaledTime < m_TimeToAllowKickSounds)
                            return false;

                        m_TimeToAllowKickSounds = Time.unscaledTime + 0.2f;
                        break;
                    }
                }
            }
            return true;
        }
    }
}
