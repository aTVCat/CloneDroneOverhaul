using HarmonyLib;
using OverhaulMod.Engine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(SceneTransitionManager))]
    internal static class SceneTransitionManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(SceneTransitionManager.DisconnectAndExitToMainMenu))]
        private static bool DisconnectAndExitToMainMenu_Prefix(SceneTransitionManager __instance)
        {
            if (!ModCore.IsActive()) return true;

            if (!TransitionManager.OverhaulSceneTransitions)
                return true;

            TransitionManager.Instance.DoSceneTransition();
            return false;
        }
    }
}