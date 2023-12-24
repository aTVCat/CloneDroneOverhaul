using System.Collections.Generic;

namespace OverhaulMod.Utils
{
    public static class ModLevelUtils
    {
        public static string WEATHER_METADATA_KEY = "OverhaulWeather";
        public static string WEATHER_INTENSITY_METADATA_KEY = "OverhaulWeatherIntensity";

        public static void SetMetaDataValue(string key, string value, ref Dictionary<string, string> keyValues)
        {
            if (keyValues == null)
                return;

            if (keyValues.ContainsKey(key))
                keyValues[key] = value;
            else
                keyValues.Add(key, value);
        }

        public static void SetMetaDataValue(string key, string value, ref LevelEditorLevelData levelEditorLevelData)
        {
            if (levelEditorLevelData == null)
                return;

            SetMetaDataValue(key, value, ref levelEditorLevelData.ModdedMetadata);
        }

        public static void SetEditingLevelMetaDataValue(string key, string value)
        {
            LevelEditorLevelData data = LevelEditorDataManager.Instance?.GetCurrentLevelData();
            SetMetaDataValue(key, value, ref data);
        }

        public static string GetMetaDataValue(string key, ref Dictionary<string, string> keyValues)
        {
            return keyValues == null || !keyValues.ContainsKey(key) ? null : keyValues[key];
        }

        public static string GetMetaDataValue(string key, ref LevelEditorLevelData levelEditorLevelData)
        {
            return levelEditorLevelData == null ? null : GetMetaDataValue(key, ref levelEditorLevelData.ModdedMetadata);
        }

        public static string GetEditingLevelMetaDataValue(string key)
        {
            LevelEditorLevelData data = LevelEditorDataManager.Instance?.GetCurrentLevelData();
            return GetMetaDataValue(key, ref data);
        }

        public static string GetCurrentLevelMetaDataValue(string key)
        {
            Dictionary<string, string> dictionary = ModGameUtils.currentLevelMetaData;
            return GetMetaDataValue(key, ref dictionary);
        }
    }
}
