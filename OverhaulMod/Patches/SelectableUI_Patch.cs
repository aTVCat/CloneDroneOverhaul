using HarmonyLib;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(SelectableUI))]
    internal static class SelectableUI_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(SelectableUI.Start))]
        private static void Start_Prefix(SelectableUI __instance)
        {
            if (__instance.GameThemeData) return;

            GameUIThemeData gameUIThemeData = ModCache.gameUIThemeData;
            if (gameUIThemeData) __instance.GameThemeData = gameUIThemeData;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(SelectableUI.onStateEnter))]
        private static bool onStateEnter_Prefix(SelectableUI __instance, UISelectionState stateEntering)
        {
            __instance.updateColorsToState(stateEntering);
            switch (stateEntering)
            {
                case UISelectionState.Selected:
                    if (!__instance.SkipSelectionArrows && __instance.GameThemeData && __instance.GameThemeData.SelectionCornerPrefab)
                    {
                        Animator animator = __instance.getEnabledCornersAnimator();
                        if (animator)
                        {
                            animator.Play("ButtonSelected");
                        }
                    }
                    break;
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(SelectableUI.onStateExit))]
        private static bool onStateExit_Prefix(SelectableUI __instance, UISelectionState stateExiting)
        {
            switch (stateExiting)
            {
                case UISelectionState.Selected:
                    if (!__instance.SkipSelectionArrows && __instance.GameThemeData && __instance.GameThemeData.SelectionCornerPrefab)
                    {
                        Animator animator = __instance.getEnabledCornersAnimator();
                        if (animator)
                        {
                            animator.Play("ButtonDeselected");
                        }
                    }
                    break;
            }
            return false;
        }
    }
}
