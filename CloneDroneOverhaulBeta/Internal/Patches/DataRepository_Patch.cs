/*using CDOverhaul.Patches;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(DataRepository))]
    internal static class DataRepository_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Save")]
        private static bool Save_Prefix(object serializedObject, string fileName, bool useResourcesFolder = false, bool writeBackup = true)
        {
            if (!OverhaulMod.IsModInitialized)
            {
                return true;
            }

            DataRepository dataRepository = DataRepository.Instance;
            string fullPath = dataRepository.GetFullPath(fileName, useResourcesFolder);
            string contents = JsonConvert.SerializeObject(serializedObject, Formatting.None, dataRepository.GetSettings());
            string directoryName = Path.GetDirectoryName(fullPath);
            if (directoryName != null)
            {
                Directory.CreateDirectory(directoryName);
            }

            if (File.Exists(fullPath) && !useResourcesFolder && writeBackup)
            {
                bool success = OverhaulCore.TryReadText(fullPath, out string contents2, out Exception excRead);
                if (success)
                {
                    OverhaulCore.TryWriteText(fullPath + ".bak", contents2, out Exception excWrite);
                }
            }
            OverhaulCore.TryWriteText(fullPath, contents, out Exception excWrite2);
            return false;
        }
    }
}*/
