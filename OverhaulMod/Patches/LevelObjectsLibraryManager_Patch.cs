using HarmonyLib;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(LevelObjectsLibraryManager))]
    internal static class LevelObjectsLibraryManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Initialize")]
        private static void Initialize_Postfix(LevelObjectsLibraryManager __instance)
        {
            ModLevelEditorManager levelEditorManager = ModLevelEditorManager.Instance;

            List<LevelObjectEntry> list = __instance._levelObjects ?? JsonConvert.DeserializeObject<List<LevelObjectEntry>>(Resources.Load<TextAsset>("Data/LevelObjects/LevelObjectManifest").text, LevelObjectsLibraryManager.getSerializerSettings());
            if (levelEditorManager)
            {
                levelEditorManager.AddObject("Axe1", "EnemySpawns", ref list, PrefabStorage.axe1Spawner, null);
                levelEditorManager.AddObject("Axe2", "EnemySpawns", ref list, PrefabStorage.axe2Spawner, null);
                levelEditorManager.AddObject("Axe3", "EnemySpawns", ref list, PrefabStorage.axe3Spawner, null);
                levelEditorManager.AddObject("Axe4", "EnemySpawns", ref list, PrefabStorage.axe4Spawner, null);

                levelEditorManager.AddObject("Scythe1", "EnemySpawns", ref list, PrefabStorage.scythe1Spawner, null);
                levelEditorManager.AddObject("Scythe2", "EnemySpawns", ref list, PrefabStorage.scythe2Spawner, null);
                levelEditorManager.AddObject("Scythe3", "EnemySpawns", ref list, PrefabStorage.scythe3Spawner, null);
                levelEditorManager.AddObject("Scythe4", "EnemySpawns", ref list, PrefabStorage.scythe4Spawner, null);
                levelEditorManager.AddObject("SprinterScythe1", "EnemySpawns", ref list, PrefabStorage.scytheSprinter1Spawner, null);
                levelEditorManager.AddObject("SprinterScythe2", "EnemySpawns", ref list, PrefabStorage.scytheSprinter2Spawner, null);

                levelEditorManager.AddObject("Halberd1", "EnemySpawns", ref list, PrefabStorage.halberd1Spawner, null);
                levelEditorManager.AddObject("Halberd2", "EnemySpawns", ref list, PrefabStorage.halberd2Spawner, null);
                levelEditorManager.AddObject("Halberd3", "EnemySpawns", ref list, PrefabStorage.halberd3Spawner, null);
            }

            __instance._levelObjects = list;
            __instance._visibleLevelObjects = list;
        }
    }
}
