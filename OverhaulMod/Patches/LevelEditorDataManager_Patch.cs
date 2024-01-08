using HarmonyLib;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(LevelEditorDataManager))]
    internal static class LevelEditorDataManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("DeserializeInto")]
        private static void DeserializeInto_Prefix(Transform levelRoot, ref LevelEditorLevelData currentLevelData, bool isAsync = false)
        {
            ModGameUtils.currentLevelMetaData = currentLevelData?.ModdedMetadata;
        }
    }
}
