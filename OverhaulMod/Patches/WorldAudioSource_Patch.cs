using HarmonyLib;
using OverhaulMod.Engine;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(WorldAudioSource))]
    internal static class WorldAudioSource_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Initialize")]
        private static void Initialize_Postfix(WorldAudioSource __instance)
        {
            if (__instance.gameObject.name.Contains("Global"))
                return;

            AudioReverbFilter filter = __instance.GetComponent<AudioReverbFilter>();
            if (!filter)
            {
                filter = __instance.gameObject.AddComponent<AudioReverbFilter>();
            }

            filter.diffusion = 100f;
            filter.density = 100f;
            filter.decayTime = ModAudioManager.EnableReverbFilter && Physics.Raycast(__instance.transform.position, Vector3.up, 75f, PhysicsManager.GetEnvironmentLayerMask())
                ? 0.375f * ModAudioManager.ReverbIntensity
                : 0f;
        }
    }
}
