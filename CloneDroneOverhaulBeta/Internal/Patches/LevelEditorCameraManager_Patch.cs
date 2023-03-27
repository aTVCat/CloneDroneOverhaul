#if DEBUG
using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(LevelEditorCameraManager))]
    internal static class LevelEditorCameraManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnLevelEditorCameraRotationChanged")]
        private static bool OnLevelEditorCameraRotationChanged_Prefix()
        {
            return false;
        }
    }
}
#endif
