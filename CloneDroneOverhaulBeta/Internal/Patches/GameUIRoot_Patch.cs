using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(GameUIRoot))]
    internal static class GameUIRoot_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("RefreshCursorEnabled")]
        private static void RefreshCursorEnabled_Postfix()
        {
            if (!OverhaulMod.IsCoreCreated)
            {
                return;
            }

            if (EnableCursorController.HasToEnableCursor())
            {
                InputManager.Instance.SetCursorEnabled(true);
            }
        }
    }
}
