using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(LevelEditorDataManager))]
    internal static class LevelEditorDataManager_Patch
    {
        // Reduces memory usage
        [HarmonyPrefix]
        [HarmonyPatch("CheckHasChangedFromFile")]
        private static bool CheckHasChangedFromFile_Prefix(ref bool __result)
        {
            if (!OverhaulMod.IsModInitialized)
                return true;

            __result = true;
            return false;
        }

        /*
        [HarmonyPrefix]
        [HarmonyPatch("SaveLevelData")]
        private static bool SaveLevelData_Prefix(string levelPathFromLevelsDir, LevelEditorLevelData currentLevelData, ref bool __result, LevelEditorDataManager __instance)
        {
            if (!OverhaulMod.IsModInitialized)
                return true;

            __result = LevelEditorFilesManager.Instance.LevelFileExists(levelPathFromLevelsDir);
            string path = DataRepository.Instance.GetFullPath(LevelEditorFilesManager.Instance.SavePathForLevel(levelPathFromLevelsDir), __instance.IsUsingResourcesFolder());

            File.WriteAllText(path, string.Empty);
            using (FileStream stream = File.OpenWrite(path))
            using (StreamWriter writer = new StreamWriter(stream))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer ser = JsonSerializer.Create(DataRepository.Instance.GetSettings());
                ser.Serialize(jsonWriter, currentLevelData);
            }
            return false;
        }*/

        /*
        [HarmonyPrefix]
        [HarmonyPatch("LoadOrCreateLevelDataFor")]
        private static bool LoadOrCreateLevelDataFor(string pathUnderLevelsFolder, LevelEditorDataManager __instance)
        {
            if (!OverhaulMod.IsModInitialized) 
                return true;

            bool isNewLevel = false;
            string fullPath = DataRepository.Instance.GetFullPath(LevelEditorFilesManager.Instance.SavePathForLevel(pathUnderLevelsFolder), __instance.IsUsingResourcesFolder());

            LevelEditorLevelData loadedLevelData = null;
            using (FileStream s = File.OpenRead(fullPath))
            using (StreamReader sr = new StreamReader(s))
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
                loadedLevelData = serializer.Deserialize<LevelEditorLevelData>(reader);
            }
            __instance._currentLevelData = loadedLevelData;

            LevelEditorLevelData levelData = __instance._currentLevelData;
            if (levelData == null)
            {
                levelData = new LevelEditorLevelData();
                isNewLevel = true;
            }
            if (levelData.DifficultyConfigurations == null)
            {
                levelData.DifficultyConfigurations = new List<LevelEditorDifficultyData>() { new LevelEditorDifficultyData(DifficultyTier.Bronze) };
            }
            if (string.IsNullOrEmpty(levelData.GeneratedUniqueID))
            {
                levelData.AssignNewGeneratedUniqueID();
            }

            __instance._currentLevelData = levelData;
            __instance._currentLevelEditorConfigData.LastEditedLevelPathUnderLevelsFolder = pathUnderLevelsFolder;
            __instance.SaveLevelEditorConfigData();
            if(isNewLevel) 
                __instance.SaveLevel();

            return false;
        }*/
    }
}
