using HarmonyLib;
using OverhaulMod.Engine;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(AudioManager))]
    internal static class AudioManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(AudioManager.refreshVolume))]
        private static void refreshVolume_Postfix()
        {
            if (!ModCore.IsActive()) return;

            ModAudioManager.Instance.StopChangingVolume(false);
            ModAudioManager.Instance.RefreshVolume();
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(AudioManager.Update))]
        private static bool Update_Prefix(AudioManager __instance)
        {
            if (!ModCore.IsActive()) return true;

            SceneTransitionManager sceneTransitionManager = SceneTransitionManager.Instance;
            if (sceneTransitionManager && sceneTransitionManager.IsDisconnecting())
            {
                if (__instance._musicFadeOutStartTime > 0f)
                {
                    float musicVolume = SettingsManager.Instance.GetMusicVolume();
                    if (Time.unscaledTime > __instance._musicFadeOutStartTime + __instance._musicFadeOutDuration)
                    {
                        __instance._musicFadeOutStartTime = -1f;
                        __instance.SetMusicVolume(0f);
                    }
                    else
                    {
                        __instance.SetMusicVolume(musicVolume * (1f - (Time.unscaledTime - __instance._musicFadeOutStartTime) / __instance._musicFadeOutDuration));
                    }
                }
                return false;
            }
            return true;
        }
    }
}
