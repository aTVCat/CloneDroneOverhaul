using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CDOverhaul
{
    public class OverhaulSettingInfo_Old
    {
        /// <summary>
        /// <b>Example: </b>Category.Section.Setting name
        /// </summary>
        public string RawPath { get; set; }
        public object DefaultValue { get; set; }
        public FieldInfo Field { get; set; }

        public string Category { get; set; }
        public string Section { get; set; }
        public string Name { get; set; }

        public bool IsChildSetting { get; set; }

        public readonly List<string> ChildSettings = new List<string>();

        public OverhaulSettingSliderParameters SliderParameters { get; set; }
        public OverhaulSettingDropdownParameters DropdownParameters { get; set; }

        public EOverhaulSettingType Type { get; set; }

        public OverhaulSettingInfo_Old CanBeLockedBy { get; set; }
        public object ValueToUnlock { get; set; }
        public bool IsUnlocked() => CanBeLockedBy == null || object.Equals(CanBeLockedBy.Field.GetValue(null), ValueToUnlock);

        public bool ForceInputField { get; set; }

        public OverhaulSettingWithEvent EventDispatcher { get; set; }

        public byte SendMessageOfType { get; set; }

        public bool Error => Type == EOverhaulSettingType.None || Field == null || string.IsNullOrEmpty(RawPath);

        internal void SetUp<T>(in string path, in object defValue, in FieldInfo field, in OverhaulUpdatedSetting formelyKnown = null)
        {
            RawPath = path;
            Type = GetSettingType(defValue);
            DefaultValue = defValue;
            Field = field;

            string[] array = path.Split('.');
            Category = array[0];
            Section = array[1];
            Name = array[2];

            if (Error)
                return;

            TryAddPref(this, formelyKnown);
            TuneUpValues();
        }

        internal void TuneUpValues()
        {
            if (Error || Type == EOverhaulSettingType.Other)
                return;

            object obj = GetPref<object>(this);
            try
            {
                Field.SetValue(null, obj);
            }
            catch
            {
                return;
            }
        }

        public void ParentSettingToThis(in OverhaulSettingInfo_Old setting)
        {
            ChildSettings.Add(setting.RawPath);
            setting.IsChildSetting = true;
        }

        public static EOverhaulSettingType GetSettingType<T>()
        {
            return typeof(T) == typeof(bool)
                ? EOverhaulSettingType.Bool
                : typeof(T) == typeof(int)
                ? EOverhaulSettingType.Int
                : typeof(T) == typeof(float)
                ? EOverhaulSettingType.Float
                : typeof(T) == typeof(string) ? EOverhaulSettingType.String : typeof(T) == typeof(long) ? EOverhaulSettingType.Other : EOverhaulSettingType.None;
        }

        public static EOverhaulSettingType GetSettingType(in object @object)
        {
            return @object is bool
                ? EOverhaulSettingType.Bool
                : @object is int
                ? EOverhaulSettingType.Int
                : @object is float
                ? EOverhaulSettingType.Float
                : @object is string ? EOverhaulSettingType.String : @object is long ? EOverhaulSettingType.Other : EOverhaulSettingType.None;
        }

        public static void TryAddPref(in OverhaulSettingInfo_Old setting, in OverhaulUpdatedSetting formelyKnown)
        {
            if (setting == null || setting.Error)
                return;

            if (!PlayerPrefs.HasKey(setting.RawPath))
            {
                if (formelyKnown != null && !string.IsNullOrEmpty(formelyKnown.RawPath) && PlayerPrefs.HasKey(formelyKnown.RawPath))
                {
                    switch (setting.Type)
                    {
                        case EOverhaulSettingType.Bool:
                            SavePref(setting, PlayerPrefs.GetInt(formelyKnown.RawPath) == 1);
                            break;
                        case EOverhaulSettingType.Int:
                            SavePref(setting, PlayerPrefs.GetInt(formelyKnown.RawPath));
                            break;
                        case EOverhaulSettingType.Float:
                            SavePref(setting, PlayerPrefs.GetFloat(formelyKnown.RawPath));
                            break;
                        case EOverhaulSettingType.String:
                            SavePref(setting, PlayerPrefs.GetString(formelyKnown.RawPath));
                            break;
                    }
                    return;
                }
                SavePref(setting, setting.DefaultValue);
            }
        }

        public static bool HasPref(in OverhaulSettingInfo_Old setting)
        {
            if (setting == null || setting.Error)
                return false;

            return PlayerPrefs.HasKey(setting.RawPath);
        }

        public static void SavePref(in OverhaulSettingInfo_Old setting, in object value, in bool dispatchEvent = true)
        {
            if (setting == null || setting.Error || setting.Type == EOverhaulSettingType.Other)
                return;

            switch (setting.Type)
            {
                case EOverhaulSettingType.Bool:
                    int a = (bool)value ? 1 : 0;
                    PlayerPrefs.SetInt(setting.RawPath, a);
                    setting.Field.SetValue(null, (bool)value);
                    break;
                case EOverhaulSettingType.Int:
                    int b = (int)value;
                    PlayerPrefs.SetInt(setting.RawPath, b);
                    setting.Field.SetValue(null, (int)value);
                    break;
                case EOverhaulSettingType.Float:
                    float c = (float)value;
                    PlayerPrefs.SetFloat(setting.RawPath, c);
                    setting.Field.SetValue(null, (float)value);
                    break;
                case EOverhaulSettingType.String:
                    string d = (string)value;
                    PlayerPrefs.SetString(setting.RawPath, d);
                    setting.Field.SetValue(null, (string)value);
                    break;
            }

            if (dispatchEvent) 
                DispatchSettingsRefreshedEvent();

            PlayerPrefs.Save();
        }

        public static T GetPref<T>(in OverhaulSettingInfo_Old setting)
        {
            if (setting == null || setting.Error || setting.Type == EOverhaulSettingType.Other)
                return default;

            object result = null;
            switch (setting.Type)
            {
                case EOverhaulSettingType.Bool:
                    result = PlayerPrefs.GetInt(setting.RawPath) == 1;
                    break;
                case EOverhaulSettingType.Int:
                    result = PlayerPrefs.GetInt(setting.RawPath);
                    break;
                case EOverhaulSettingType.Float:
                    result = PlayerPrefs.GetFloat(setting.RawPath);
                    break;
                case EOverhaulSettingType.String:
                    result = PlayerPrefs.GetString(setting.RawPath);
                    break;
            }
            return (T)result;
        }

        public static void DispatchSettingsRefreshedEvent() => OverhaulEvents.DispatchEvent(OverhaulSettingsManager_Old.SETTING_VALUE_UPDATED_EVENT);
    }
}
