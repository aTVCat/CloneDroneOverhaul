using CDOverhaul.Graphics;
using CDOverhaul.HUD;
using CDOverhaul.MultiplayerSandbox;
using HarmonyLib;
using ModLibrary;
using Steamworks;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(GameDataManager))]
    internal static class GameDataManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("GetCurrentLevelID")]
        private static void GetCurrentLevelID_Postfix(GameDataManager __instance, ref string __result)
        {            
            if (!FirstUseSetupUI.HasSetTheModUp && GameModeManager.IsOnTitleScreen())
                __result = "U6Bronze2";
        }
    }
}
