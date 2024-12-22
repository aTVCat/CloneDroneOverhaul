using System;
using System.Collections.Generic;

namespace OverhaulMod.Engine
{
    public class ModSettingsPreset
    {
        public int? QualityLevel;

        public AntiAliasingMode? AntiAliasingMode;

        public Dictionary<string, int> IntValues;

        public Dictionary<string, float> FloatValues;

        public Dictionary<string, string> StringValues;

        public ModSettingsPreset()
        {
        }

        public ModSettingsPreset(bool initialize)
        {
            if (initialize)
            {
                IntValues = new Dictionary<string, int>();
                FloatValues = new Dictionary<string, float>();
                StringValues = new Dictionary<string, string>();
            }
        }

        public void AddValue(string setting, object value)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");
            if (value == null)
                throw new ArgumentNullException("value");

            if (value is bool boolValue)
            {
                IntValues.Add(setting, boolValue ? 1 : 0);
            }
            else if (value is int intValue)
            {
                IntValues.Add(setting, intValue);
            }
            else if (value is float floatValue)
            {
                FloatValues.Add(setting, floatValue);
            }
            else if (value is string stringValue)
            {
                StringValues.Add(setting, stringValue);
            }
        }

        public void Apply()
        {
            SettingsManager settingsManager = SettingsManager.Instance;
            if (QualityLevel != null)
            {
                settingsManager.SetQuality(QualityLevel.Value);
            }
            if (AntiAliasingMode != null)
            {
                settingsManager.SetAntiAliasing(AntiAliasingMode.Value);
            }

            ModSettingsManager modSettingsManager = ModSettingsManager.Instance;
            foreach (KeyValuePair<string, int> kv in IntValues)
            {
                ModSetting modSetting = modSettingsManager.GetSetting(kv.Key);
                if (modSetting.valueType == ModSetting.ValueType.Bool)
                {
                    modSetting.SetBoolValue(kv.Value == 0 ? false : true);
                }
                else
                {
                    modSetting.SetIntValue(kv.Value);
                }
            }

            foreach (KeyValuePair<string, float> kv in FloatValues)
            {
                ModSetting modSetting = modSettingsManager.GetSetting(kv.Key);
                modSetting.SetFloatValue(kv.Value);
            }

            foreach (KeyValuePair<string, string> kv in StringValues)
            {
                ModSetting modSetting = modSettingsManager.GetSetting(kv.Key);
                modSetting.SetStringValue(kv.Value);
            }
        }
    }
}
