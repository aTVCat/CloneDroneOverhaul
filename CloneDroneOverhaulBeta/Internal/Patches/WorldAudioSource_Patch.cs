using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(WorldAudioSource))]
    internal static class WorldAudioSource_Patch
    {
        [OverhaulSettingAttribute_Old("Audio.Filters.Reverb", true)]
        public static bool EnableReverbFilter;

        public static float DecayTime = 0.375f;
        public static float Diffusion = 100f;
        public static float Density = 100f;

        [HarmonyPostfix]
        [HarmonyPatch("Initialize")]
        private static void Initialize_Postfix(WorldAudioSource __instance)
        {
            if (!OverhaulMod.IsModInitialized || __instance.gameObject.name.Contains("Global"))
                return;

            AudioReverbFilter filter = __instance.GetComponent<AudioReverbFilter>();
            if (!filter)
            {
                filter = __instance.gameObject.AddComponent<AudioReverbFilter>();
            }

            filter.diffusion = Diffusion;
            filter.density = Density;
            filter.decayTime = EnableReverbFilter && Physics.Raycast(__instance.transform.position, Vector3.up, 50f, PhysicsManager.GetEnvironmentLayerMask())
                ? DecayTime
                : 0f;
        }
    }
}
