using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(WorldAudioSource))]
    internal static class WorldAudioSource_Patch
    {
        [OverhaulSetting("Audio.Filters.Reverb", true)]
        public static bool EnableReverbFilter;

        public static float DecayTime = 0.375f;
        public static float Diffusion = 100f;
        public static float Density = 100f;

        [HarmonyPrefix]
        [HarmonyPatch("Initialize")]
        private static void Initialize_Prefix(WorldAudioSource __instance)
        {
            AudioReverbFilter filter = __instance.GetComponent<AudioReverbFilter>();
            if (!OverhaulMod.IsModInitialized || !EnableReverbFilter || __instance.gameObject.name.Contains("Global"))
            {
                if (filter)
                    Object.Destroy(filter);

                return;
            }

            if (!filter)
                filter = __instance.gameObject.AddComponent<AudioReverbFilter>();

            filter.diffusion = Diffusion;
            filter.density = Density;
            filter.decayTime = Physics.Raycast(__instance.transform.position, Vector3.up, 50f, PhysicsManager.GetEnvironmentLayerMask()) ? DecayTime : 0f;
        }
    }
}
