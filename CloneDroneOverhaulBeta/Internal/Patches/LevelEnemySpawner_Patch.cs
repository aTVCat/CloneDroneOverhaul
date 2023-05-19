using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Multiplayer;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(LevelEnemySpawner))]
    internal static class LevelEnemySpawner_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Start")]
        private static void Start_Prefix(LevelEnemySpawner __instance)
        {
            if (!OverhaulMod.IsModInitialized)
            {
                return;
            }

            string name = __instance.gameObject.name;
            if(name != "BusinessSword(Clone)")
            {
                return;
            }            
            __instance.SupportsColorOverride = true;
        }
    }
}
