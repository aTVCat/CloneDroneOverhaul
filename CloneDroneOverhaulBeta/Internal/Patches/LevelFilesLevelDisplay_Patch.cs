using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModLibrary;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(LevelFilesLevelDisplay))]
    internal static class LevelFilesLevelDisplay_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnPointerDown")]
        private static bool OnPointerDown_Prefix()
        {
            return !OverhaulMod.IsModInitialized || !LevelEditorDataManager_Patch.IOStateInfo.IsInProgress;
        }
    }
}
