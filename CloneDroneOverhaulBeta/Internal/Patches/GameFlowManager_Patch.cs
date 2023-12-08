using CDOverhaul.Graphics;
using HarmonyLib;
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
                bloomEndIntensity = OverhaulGraphicsController.BloomIntensity;
                bloomEndThreshold = OverhaulGraphicsController.BloomThreshold;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("ShowTitleScreen")]
        private static void ShowTitleScreen_Postfix(GameFlowManager __instance)
        {
            Bloom bl = ArenaCameraManager.Instance.TitleScreenLevelCamera.GetComponent<Bloom>();
            if (bl)
            {
                bl.bloomIntensity = OverhaulGraphicsController.BloomIntensity;
                bl.bloomThreshold = OverhaulGraphicsController.BloomThreshold;
            }

            Animator an = ArenaCameraManager.Instance.TitleScreenLevelCamera.GetComponent<Animator>();
            if (an)
            {
                Object.Destroy(an);
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
