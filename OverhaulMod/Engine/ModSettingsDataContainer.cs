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
