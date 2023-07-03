using CDOverhaul.Graphics;
using CDOverhaul.MultiplayerSandbox;
using HarmonyLib;
using Steamworks;
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

        [HarmonyPrefix]
        [HarmonyPatch("onCharacterKilled")]
        private static bool onCharacterKilled_Prefix(Character character)
        {
            character.DisableInput();

            if (!OverhaulGamemodeManager.IsMultiplayerSandbox())
                return true;

            if (!(character is FirstPersonMover))
                return true;

            FirstPersonMover firstPersonMover = character as FirstPersonMover;
            return false;
        }
    }
}
