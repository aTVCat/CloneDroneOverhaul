using System.Collections.Generic;

namespace OverhaulMod.Engine
{
    public class ModSettingsDataContainer
    {
        public Dictionary<string, int> IntValues;

        public Dictionary<string, float> FloatValues;

        public Dictionary<string, string> StringValues;

        public void FixValues()
        {
            if (IntValues == null)
                IntValues = new Dictionary<string, int>();

            if (FloatValues == null)
                FloatValues = new Dictionary<string, float>();

            if (StringValues == null)
                StringValues = new Dictionary<string, string>();
        }

        public void SetValues(ModSettingsDataContainer otherContainer, bool fromUI = false)
        {
            if (otherContainer == null)
                return;

            ModSettingsManager modSettingsManager = ModSettingsManager.Instance;
            if (otherContainer.IntValues != null)
            {
                foreach (KeyValuePair<string, int> keyValue in otherContainer.IntValues)
                {
                    if (modSettingsManager.HasSettingWithName(keyValue.Key))
                    {
                        if(modSettingsManager.GetSetting(keyValue.Key).valueType == ModSetting.ValueType.Bool)
                        {
                            if (ModSettingsManager.GetBoolValue(keyValue.Key) != (keyValue.Value == 0 ? false : true))
                                ModSettingsManager.SetBoolValue(keyValue.Key, keyValue.Value == 0 ? false : true, fromUI);
                        }
                        else
                        {
                            if (ModSettingsManager.GetIntValue(keyValue.Key) != keyValue.Value)
                                ModSettingsManager.SetIntValue(keyValue.Key, keyValue.Value);
                        }
                    }
                }
            }
            if (otherContainer.FloatValues != null)
            {
                foreach (KeyValuePair<string, float> keyValue in otherContainer.FloatValues)
                {
                    if (modSettingsManager.HasSettingWithName(keyValue.Key) && ModSettingsManager.GetFloatValue(keyValue.Key) != keyValue.Value)
                        ModSettingsManager.SetFloatValue(keyValue.Key, keyValue.Value, fromUI);
                }
            }
            if (otherContainer.StringValues != null)
            {
                foreach (KeyValuePair<string, string> keyValue in otherContainer.StringValues)
                {
                    if (modSettingsManager.HasSettingWithName(keyValue.Key) && ModSettingsManager.GetStringValue(keyValue.Key) != keyValue.Value)
                        ModSettingsManager.SetStringValue(keyValue.Key, keyValue.Value, fromUI);
                }
            }
        }

        public void SetInt(string key, int value)
        {
            Dictionary<string, int> dictionary = IntValues;
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }

        public void SetFloat(string key, float value)
        {
            Dictionary<string, float> dictionary = FloatValues;
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }

        public void SetString(string key, string value)
        {
            Dictionary<string, string> dictionary = StringValues;
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }

        public int GetInt(string key, int defaultValue)
        {
            if (IntValues.TryGetValue(key, out int value))
                return value;

            return defaultValue;
        }

        public float GetFloat(string key, float defaultValue)
        {
            if (FloatValues.TryGetValue(key, out float value))
                return value;

            return defaultValue;
        }

        public string GetString(string key, string defaultValue)
        {
            if (StringValues.TryGetValue(key, out string value))
                return value;

            return defaultValue;
        }
    }
}
