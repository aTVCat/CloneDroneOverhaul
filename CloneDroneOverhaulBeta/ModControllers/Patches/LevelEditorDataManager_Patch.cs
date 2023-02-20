/*using HarmonyLib;
using ModLibrary;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(LevelEditorDataManager))]
    internal static class LevelEditorDataManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("CheckHasChangedFromFile")]
        private static bool CheckHasChangedFromFile_Prefix()
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("LoadOrCreateLevelDataFor")]
        private static bool LoadOrCreateLevelDataFor_Prefix(LevelEditorDataManager __instance, string pathUnderLevelsFolder)
        {
            string fullPath = DataRepository.Instance.GetFullPath(LevelEditorFilesManager.Instance.SavePathForLevel(pathUnderLevelsFolder), __instance.IsUsingResourcesFolder());
            /*
            JsonSerializerSettings settings = DataRepository.Instance.GetSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            
            StreamReader w = File.OpenText(fullPath);
            using (JsonTextReader jsonReader = new JsonTextReader(w))
            {
                JsonSerializer serializer = JsonSerializer.Create(settings);
                serializer.Deserialize<LevelEditorLevelData>(jsonReader);
                jsonReader.Close();
            }
            w.Close();
            w.Dispose();//

            bool fileExists = File.Exists(fullPath);

            if (fileExists)
            {
                using (WebClient client = new WebClient())
                {
                    using (StreamReader sr = new StreamReader(client.OpenRead("file://" + fullPath)))
                    {
                        using (JsonReader reader = new JsonTextReader(sr))
                        {
                            JsonSerializer serializer = new JsonSerializer();

                            // read the json from a stream
                            // json size doesn't matter because only a small piece is read at a time from the HTTP request
                            LevelEditorLevelData result = serializer.Deserialize<LevelEditorLevelData>(reader);
                            if (string.IsNullOrEmpty(result.GeneratedUniqueID))
                            {
                                result.AssignNewGeneratedUniqueID();
                            }
                            if (result.DifficultyConfigurations == null || result.DifficultyConfigurations.Count == 0)
                            {
                                result.DifficultyConfigurations = new List<LevelEditorDifficultyData>
                                {
                                    new LevelEditorDifficultyData(DifficultyTier.Bronze)
                                };
                            }

                            __instance.SetPrivateField<LevelEditorLevelData>("_currentLevelData", result);

                            reader.Close();
                        }
                        sr.Close();
                        sr.Dispose();
                    }
                    client.Dispose();
                }
                __instance.GetPrivateField<LevelEditorConfigData>("_currentLevelEditorConfigData").LastEditedLevelPathUnderLevelsFolder = pathUnderLevelsFolder;
                __instance.SaveLevelEditorConfigData();
                return false;
            }

            LevelEditorLevelData levelDataNew = new LevelEditorLevelData();
            levelDataNew.AssignNewGeneratedUniqueID();
            levelDataNew.DifficultyConfigurations = new List<LevelEditorDifficultyData>
            {
                new LevelEditorDifficultyData(DifficultyTier.Bronze)
            };
            __instance.GetPrivateField<LevelEditorConfigData>("_currentLevelEditorConfigData").LastEditedLevelPathUnderLevelsFolder = pathUnderLevelsFolder;
            __instance.SaveLevelEditorConfigData();

            return false;
        }
    }
}*/
