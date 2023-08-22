using CDOverhaul.Visuals;
using HarmonyLib;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(GameFlowManager))]
    internal static class GameFlowManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("bloomValueChangeCoroutine")]
        private static void bloomValueChangeCoroutine_Prefix(Bloom bloom, float duration, ref float bloomEndIntensity, ref float bloomEndThreshold)
        {
            if (bloom.GetComponent<Animator>() != null)
                Object.Destroy(bloom.GetComponent<Animator>());

            if (duration == 1.5f)
            {
                bloomEndIntensity = BloomOverhaulImageEffect.BloomIntensity;
                bloomEndThreshold = BloomOverhaulImageEffect.BloomThreshold;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("ShowTitleScreen")]
        private static void ShowTitleScreen_Postfix(GameFlowManager __instance)
        {
            Bloom bl = ArenaCameraManager.Instance.TitleScreenLevelCamera.GetComponent<Bloom>();
            if (bl)
            {
                bl.bloomIntensity = BloomOverhaulImageEffect.BloomIntensity;
                bl.bloomThreshold = BloomOverhaulImageEffect.BloomThreshold;
            }

            Animator an = ArenaCameraManager.Instance.TitleScreenLevelCamera.GetComponent<Animator>();
            if (an)
            {
                Object.Destroy(an);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("onCharacterKilled")]
        private static bool onCharacterKilled_Prefix(Character character)
        {
            character.DisableInput();

            return !OverhaulGamemodeManager.IsMultiplayerSandbox() || !(character is FirstPersonMover);
        }

        [HarmonyPrefix]
        [HarmonyPatch("Start")]
        private static void Start_Prefix()
        {
            foreach(OverhaulController overhaulController in OverhaulController.allControllers)
            {
                OverhaulDebug.Log("Calling OnSceneReloaded - " + overhaulController.GetType().ToString(), EDebugType.ModRl);
                overhaulController.OnSceneReloaded();
            }
        }

        /*
        [HarmonyPrefix]
        [HarmonyPatch("ShowTitleScreen")]
        private static bool ShowTitleScreen_Prefix(GameFlowManager __instance)
        {
            if (FirstUseSetupUI.HasSetTheModUp)
                return true;

            LevelManager.Instance.SpawnCurrentLevel(false, "U6Bronze2", null).MoveNext();
            __instance.StartCoroutine(__instance.CallPrivateMethod<IEnumerator>("waitThenShowTitleScreenCamera", null));

            GameUIRoot.Instance.TitleScreenUI.Show();
            GameUIRoot.Instance.RefreshCursorEnabled();
            return false;
        }*/
    }
}
