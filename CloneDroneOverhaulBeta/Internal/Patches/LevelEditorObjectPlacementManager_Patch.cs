using HarmonyLib;
using ModLibrary;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(LevelEditorObjectPlacementManager))]
    internal static class LevelEditorObjectPlacementManager_Patch
    {
        private static LevelObjectsLibraryManager s_LevelObjectsLibraryManager;
        public static readonly Dictionary<string, Transform> s_CachedPrefabs = new Dictionary<string, Transform>();

        [HarmonyPrefix]
        [HarmonyPatch("PlaceObjectInLevelRoot")]
        private static bool PlaceObjectInLevelRoot_Prefix(LevelEditorObjectPlacementManager __instance, ref ObjectPlacedInLevel __result, LevelObjectEntry objectPlacedLevelObjectEntry, Transform levelRoot)
        {
            if (!s_LevelObjectsLibraryManager)
                s_LevelObjectsLibraryManager = LevelObjectsLibraryManager.Instance;

            string path = objectPlacedLevelObjectEntry.PathUnderResources;

            if (!s_CachedPrefabs.TryGetValue(path, out Transform prefabToInstantiate))
            {
                prefabToInstantiate = Resources.Load<Transform>(path);
                if (!prefabToInstantiate)
                {
                    prefabToInstantiate = Resources.Load<Transform>(s_LevelObjectsLibraryManager.GetRenamedPath(path));
                    if (!prefabToInstantiate)
                    {
                        if (objectPlacedLevelObjectEntry.PathUnderResources.StartsWith("modded/"))
                        {
                            // Todo: Show dialogue window here
                        }
                        else
                        {
                            // Todo: Show dialogue window here
                        }

                        __result = null;
                        return false;
                    }
                }
                s_CachedPrefabs.Add(path, prefabToInstantiate);
            }

            Transform transform2 = Object.Instantiate(prefabToInstantiate, levelRoot, false);
            if (!objectPlacedLevelObjectEntry.IsSection())
                _ = transform2.gameObject.AddComponent<SectionMember>();

            __result = transform2.gameObject.AddComponent<ObjectPlacedInLevel>();
            __result.LevelObjectEntry = objectPlacedLevelObjectEntry;
            __result.Initialize(levelRoot);

            if (OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsGpuInstancingEnabled)
                _ = __result.gameObject.AddComponent<OverhaulGPUInstanceObjectBehaviour>();

            __instance.registerObjectInAllObjectList(__result);
            return false;
        }
    }
}
