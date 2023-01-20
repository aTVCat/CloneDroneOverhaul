using System;
using System.Collections.Generic;
using System.Linq;
using ModLibrary;
using UnityEngine;

namespace CloneDroneOverhaul.V3
{
    public static class ObjectSpawner
    {
        private static Dictionary<string, Transform> _cachedObjects = new Dictionary<string, Transform>();

        public static ObjectPlacedInLevel PlaceObject(in LevelObjectEntry objectPlacedLevelObjectEntry, in Transform levelRoot)
        {
            string path = objectPlacedLevelObjectEntry.PathUnderResources;

            Transform transform = null;
            if (_cachedObjects.ContainsKey(path))
            {
                transform = _cachedObjects[path];
            }
            else
            {
                transform = OverhaulCacheAndGarbageController.GetModdedLevelEditorResource(path);

                if (transform == null)
                {
                    transform = Resources.Load<Transform>(LevelObjectsLibraryManager.Instance.GetRenamedPath(path));
                    if (transform == null)
                    {
                        if (objectPlacedLevelObjectEntry.PathUnderResources.StartsWith("modded/"))
                        {
                            Debug.LogError("Looks like this level requires a mod called " + path.Split(new char[]
                            {
                        '/'
                            })[1] + "ask around in the discord on how you fix this :)");
                        }
                        else
                        {
                            Debug.LogError("PlaceObjectInLevelRoot, Can't find asset: " + path);
                        }
                        return null;
                    }
                }

                if(transform != null)
                {
                    _cachedObjects.Add(path, transform);
                }
            }

            Transform transform2 = UnityEngine.Object.Instantiate(transform, levelRoot);

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
            LevelEditorObjectPlacementManager.Instance.CallPrivateMethod("registerObjectInAllObjectList", args);

            return objectPlacedInLevel;
        }
    }
}
