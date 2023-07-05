using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace CDOverhaul.DevTools
{
    public static class OverhaulJsonEditor
    {
        public static string[] GetAllFilePaths(bool ofJsonObjects) => Directory.GetFiles(ofJsonObjects ? OverhaulJsonObject.JsonObjectsFolder : OverhaulJsonObjectPreset.PresetsFolder);
        public static string[] GetAllFileNames(bool ofJsonObjects)
        {
            int i = 0;
            string[] paths = GetAllFilePaths(ofJsonObjects);
            foreach(string @string in paths)
            {
                paths[i] = getName(@string);
                i++;
            }
            return paths;
        }
        public static List<Dropdown.OptionData> GetAllFileNamesOptionsData(bool ofJsonObjects)
        {
            List<Dropdown.OptionData> result = new List<Dropdown.OptionData>();
            string[] array = GetAllFileNames(ofJsonObjects);
            foreach (string @string in array)
            {
                result.Add(new Dropdown.OptionData(@string));
            }
            return result;
        }

        public static OverhaulJsonObject CreateAndSaveJsonObject(string name)
        {
            OverhaulJsonObject jsonObject = new OverhaulJsonObject();
            SaveJsonObject(jsonObject, name);
            return jsonObject;
        }
        public static OverhaulJsonObject CreateAndSaveJsonObjectFromPreset(string name, string presetName)
        {
            OverhaulJsonObject jsonObject = new OverhaulJsonObject(GetJsonObjectPreset(presetName));
            SaveJsonObject(jsonObject, name);
            return jsonObject;
        }

        public static void SaveJsonObject(OverhaulJsonObject jsonObject, string name)
        {
            string path = OverhaulJsonObject.JsonObjectsFolder + name + ".json";
            string data = jsonObject.ToJson();
            OverhaulCore.WriteText(path, data);
        }

        public static void SaveJsonObjectPreset(OverhaulJsonObjectPreset jsonObject, string name)
        {
            string path = OverhaulJsonObjectPreset.PresetsFolder + name + ".json";
            string data = jsonObject.ToJson();
            OverhaulCore.WriteText(path, data);
        }

        public static OverhaulJsonObject GetJsonObject(string path)
        {
            string data = getString(path, true);
            return OverhaulJsonObject.FromJson(data);
        }

        public static OverhaulJsonObjectPreset GetJsonObjectPreset(string path)
        {
            string data = getString(path, false);
            return OverhaulJsonObjectPreset.FromJson(data);
        }

        private static string getString(string path, bool isJsonObject)
        {
            bool hasDotJson = path.Contains(".json");
            bool isPath = path.Contains("/") || path.Contains("\\");
            if (isPath)
            {
                return OverhaulCore.ReadText(path + (hasDotJson ? string.Empty : ".json"));
            }
            return OverhaulCore.ReadText((isJsonObject ? OverhaulJsonObject.JsonObjectsFolder : OverhaulJsonObjectPreset.PresetsFolder) + path + (hasDotJson ? string.Empty : ".json"));
        }

        private static string getName(string @string) => @string.Substring(@string.LastIndexOf('/') + 1).Replace(".json", string.Empty);
    }
}
