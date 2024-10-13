using HarmonyLib;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(LevelEditorCinematicCamera))]
    internal static class LevelEditorCinematicCamera_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(LevelEditorCinematicCamera.TurnOff))]
        private static void TurnOff_Prefix(LevelEditorCinematicCamera __instance)
        {
            if (__instance._hasTakenOverPlayerCamera)
            {
                GlobalEventManager.Instance.Dispatch(Engine.CameraManager.CINEMATIC_CAMERA_TURNED_OFF_EVENT);
            }
        }
    }
}
