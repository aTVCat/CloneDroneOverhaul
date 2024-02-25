using HarmonyLib;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(LevelEditorLevelObject))]
    internal static class LevelEditorLevelObject_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("DeserializeInto")]
        private static void DeserializeInto_Postfix(LevelEditorLevelObject __instance, ObjectPlacedInLevel objectPlacedInLevel)
        {
            if (objectPlacedInLevel)
            {
                LevelEditorObjectAdvancedBehaviour objectAdvancedBehaviour = objectPlacedInLevel.GetComponent<LevelEditorObjectAdvancedBehaviour>();
                if(!objectAdvancedBehaviour)
                    objectAdvancedBehaviour = objectPlacedInLevel.gameObject.AddComponent<LevelEditorObjectAdvancedBehaviour>();

                objectAdvancedBehaviour.SerializedObject = __instance;
            }
        }
    }
}
