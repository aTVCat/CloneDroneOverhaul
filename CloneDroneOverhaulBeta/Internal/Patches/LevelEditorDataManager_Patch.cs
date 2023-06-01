using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(LevelEditorDataManager))]
    internal static class LevelEditorDataManager_Patch
    {
        public static readonly OverhaulCore.IOStateInfo IOStateInfo = new OverhaulCore.IOStateInfo();

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

#if AllowLevelDataPatches

        [HarmonyPrefix]
        [HarmonyPatch("SaveLevelData")]
        private static bool SaveLevelData_Prefix(string levelPathFromLevelsDir, LevelEditorLevelData currentLevelData, ref bool __result, LevelEditorDataManager __instance)
        {
            if (!OverhaulMod.IsModInitialized || __instance.IsUsingResourcesFolder()) return true;
            if (IOStateInfo.IsInProgress) return false;

            __result = LevelEditorFilesManager.Instance.LevelFileExists(levelPathFromLevelsDir);
            /*
            string contentToSave = JsonConvert.SerializeObject(currentLevelData, Formatting.None, DataRepository.Instance.GetSettings());
            StaticCoroutineRunner.StartStaticCoroutine(OverhaulCore.WriteTextCoroutine(DataRepository.Instance.GetFullPath(LevelEditorFilesManager.Instance.SavePathForLevel(levelPathFromLevelsDir), false), contentToSave, IOStateInfo));
            */
            using (FileStream stream = File.OpenWrite(DataRepository.Instance.GetFullPath(LevelEditorFilesManager.Instance.SavePathForLevel(levelPathFromLevelsDir))))
            using (StreamWriter writer = new StreamWriter(stream))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer ser = JsonSerializer.Create(DataRepository.Instance.GetSettings());
                ser.Serialize(jsonWriter, currentLevelData);

                jsonWriter.Close();
                writer.Close();
                stream.Close();
            }
            GC.Collect();
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("LoadOrCreateLevelDataFor")]
        private static bool LoadOrCreateLevelDataFor(string pathUnderLevelsFolder, LevelEditorDataManager __instance)
        {
            if (!OverhaulMod.IsModInitialized || __instance.IsUsingResourcesFolder()) return true;
            __instance.SetPrivateField<LevelEditorLevelData>("_currentLevelData", null);

            bool isNewLevel = false;
            string fullPath = DataRepository.Instance.GetFullPath(LevelEditorFilesManager.Instance.SavePathForLevel(pathUnderLevelsFolder), false);

            LevelEditorLevelData loadedLevelData = null;
            using (FileStream s = File.OpenRead(fullPath))
            using (StreamReader sr = new StreamReader(s))
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
                loadedLevelData = serializer.Deserialize<LevelEditorLevelData>(reader);

                reader.Close();
                sr.Close();
                s.Close();
            }
            __instance.SetPrivateField("_currentLevelData", loadedLevelData);

            LevelEditorLevelData levelData = __instance.GetPrivateField<LevelEditorLevelData>("_currentLevelData");
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

            __instance.SetPrivateField("_currentLevelData", levelData);
            __instance.GetPrivateField<LevelEditorConfigData>("_currentLevelEditorConfigData").LastEditedLevelPathUnderLevelsFolder = pathUnderLevelsFolder;
            __instance.SaveLevelEditorConfigData();
            if(isNewLevel) __instance.SaveLevel();
            GC.Collect();
            return false;
        }

#endif
    }
}
