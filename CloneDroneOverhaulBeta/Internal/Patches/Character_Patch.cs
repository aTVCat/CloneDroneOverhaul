using CDOverhaul.Visuals;
using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(Character))]
    internal static class Character_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("MovePlayerCameraBack")]
        private static void MovePlayerCameraBack_Prefix(Character __instance, float moveTime)
        {
            if (!OverhaulMod.IsModInitialized || !ViewModesController.IsFirstPersonModeEnabled)
                return;

            Camera camera = __instance.GetPlayerCamera();
            if (camera != null && moveTime <= 0f)
                camera.transform.localPosition = ViewModesController.DefaultCameraOffset;
        }
    }
}
