using HarmonyLib;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(DataRepository))]
    internal static class DataRepository_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Save")]
        private static bool Save_Prefix(DataRepository __instance, object serializedObject, string fileName, bool useResourcesFolder = false, bool writeBackup = true)
        {
            string fullPath = __instance.GetFullPath(fileName, useResourcesFolder);
            EnsureDirectoryForFileCreated(fullPath);

            string file = Application.temporaryCachePath + "CloneDroneLevelData.json";
            JsonSerializerSettings settings = __instance.GetSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            StreamWriter w = File.CreateText(file);
            using (JsonTextWriter jsonWriter = new JsonTextWriter(w))
            {
                JsonSerializer serializer = JsonSerializer.Create(settings);
                serializer.Serialize(jsonWriter, serializedObject);
                jsonWriter.Flush();
            }
            w.Close();
            w.Dispose();

            if (File.Exists(fullPath)) File.Delete(fullPath);
            File.Move(file, fullPath);

            return false;
        }

        private static void EnsureDirectoryForFileCreated(string fullPath)
        {
            string directoryName = System.IO.Path.GetDirectoryName(fullPath);
            if (directoryName != null)
            {
                Directory.CreateDirectory(directoryName);
            }
        }
    }
}
