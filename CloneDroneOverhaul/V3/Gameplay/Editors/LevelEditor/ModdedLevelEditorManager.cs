using ModLibrary;
using System.Collections.Generic;
using UnityEngine;

namespace CloneDroneOverhaul.LevelEditor
{
    public class ModdedLevelEditorManager
    {

        public ObjectPlacedInLevel PlaceObject(LevelObjectEntry objectPlacedLevelObjectEntry, Transform levelRoot)
        {
            Transform transform = OverhaulCacheAndGarbageController.GetModdedLevelEditorResource(objectPlacedLevelObjectEntry.PathUnderResources);

            if (transform == null)
            {
                transform = Resources.Load<Transform>(Singleton<LevelObjectsLibraryManager>.Instance.GetRenamedPath(objectPlacedLevelObjectEntry.PathUnderResources));
                if (transform == null)
                {
                    if (objectPlacedLevelObjectEntry.PathUnderResources.StartsWith("modded/"))
                    {
                        Debug.LogError("Looks like this level requires a mod called " + objectPlacedLevelObjectEntry.PathUnderResources.Split(new char[]
                        {
                        '/'
                        })[1] + "ask around in the discord on how you fix this :)");
                    }
                    else
                    {
                        Debug.LogError("PlaceObjectInLevelRoot, Can't find asset: " + objectPlacedLevelObjectEntry.PathUnderResources);
                    }
                    return null;
                }
            }

            Transform transform2 = UnityEngine.Object.Instantiate<Transform>(transform);
            transform2.SetParent(levelRoot, false);

            if (!objectPlacedLevelObjectEntry.IsSection())
            {
                transform2.gameObject.AddComponent<SectionMember>();
            }

            ObjectPlacedInLevel objectPlacedInLevel = transform2.gameObject.AddComponent<ObjectPlacedInLevel>();
            objectPlacedInLevel.LevelObjectEntry = objectPlacedLevelObjectEntry;
            objectPlacedInLevel.Initialize(levelRoot);

            object[] args = new object[]
            {
                objectPlacedInLevel
            };
            Singleton<LevelEditorObjectPlacementManager>.Instance.CallPrivateMethod("registerObjectInAllObjectList", args);

            return objectPlacedInLevel;
        }

    }
}
