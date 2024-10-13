using HarmonyLib;
using OverhaulMod.Engine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(AudioManager))]
    internal static class AudioManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(AudioManager.refreshVolume))]
        private static void refreshVolume_Postfix()
        {
            ModAudioManager.Instance.StopChangingVolume(false);
            ModAudioManager.Instance.RefreshVolume();
        }
    }
}
