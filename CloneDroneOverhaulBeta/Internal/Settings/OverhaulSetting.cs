using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CDOverhaul
{
    public class SettingInfo
    {
        /// <summary>
        /// <b>Example: </b>Category.Section.Setting
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

        public OverhaulSettingTypes Type { get; set; }

        public SettingInfo CanBeLockedBy { get; set; }
        public object ValueToUnlock { get; set; }
        public bool IsUnlocked() => CanBeLockedBy == null || object.Equals(CanBeLockedBy.Field.GetValue(null), ValueToUnlock);

        public bool ForceInputField { get; set; }

        public OverhaulSettingWithEvent EventDispatcher { get; set; }

        public byte SendMessageOfType { get; set; }

        public bool Error => Type == OverhaulSettingTypes.None || Field == null || string.IsNullOrEmpty(RawPath);

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
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingError);

            TryAddPref(this, formelyKnown);
            TuneUpValues();
        }

        internal void TuneUpValues()
        {
            if (Error)
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingError);

            if (Type == OverhaulSettingTypes.Other)
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

        public void ParentSettingToThis(in SettingInfo setting)
        {
            ChildSettings.Add(setting.RawPath);
            setting.IsChildSetting = true;
        }

        public static OverhaulSettingTypes GetSettingType<T>()
        {
            return typeof(T) == typeof(bool)
                ? OverhaulSettingTypes.Bool
                : typeof(T) == typeof(int)
                ? OverhaulSettingTypes.Int
                : typeof(T) == typeof(float)
                ? OverhaulSettingTypes.Float
                : typeof(T) == typeof(string) ? OverhaulSettingTypes.String : typeof(T) == typeof(long) ? OverhaulSettingTypes.Other : OverhaulSettingTypes.None;
        }

        public static OverhaulSettingTypes GetSettingType(in object @object)
        {
            return @object is bool
                ? OverhaulSettingTypes.Bool
                : @object is int
                ? OverhaulSettingTypes.Int
                : @object is float
                ? OverhaulSettingTypes.Float
                : @object is string ? OverhaulSettingTypes.String : @object is long ? OverhaulSettingTypes.Other : OverhaulSettingTypes.None;
        }

        public static void TryAddPref(in SettingInfo setting, in OverhaulUpdatedSetting formelyKnown)
        {
            if (setting == null || setting.Error)
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingError);

            if (!PlayerPrefs.HasKey(setting.RawPath))
            {
                if (formelyKnown != null && !string.IsNullOrEmpty(formelyKnown.RawPath) && PlayerPrefs.HasKey(formelyKnown.RawPath))
                {
                    switch (setting.Type)
                    {
                        case OverhaulSettingTypes.Bool:
                            SavePref(setting, PlayerPrefs.GetInt(formelyKnown.RawPath) == 1);
                            break;
                        case OverhaulSettingTypes.Int:
                            SavePref(setting, PlayerPrefs.GetInt(formelyKnown.RawPath));
                            break;
                        case OverhaulSettingTypes.Float:
                            SavePref(setting, PlayerPrefs.GetFloat(formelyKnown.RawPath));
                            break;
                        case OverhaulSettingTypes.String:
                            SavePref(setting, PlayerPrefs.GetString(formelyKnown.RawPath));
                            break;
                    }
                    return;
                }
                SavePref(setting, setting.DefaultValue);
            }
        }

        public static bool HasPref(in SettingInfo setting)
        {
            if (setting == null || setting.Error)
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingError);

            return PlayerPrefs.HasKey(setting.RawPath);
        }

        public static void SavePref(in SettingInfo setting, in object value, in bool dispatchEvent = true)
        {
            if (setting == null || setting.Error)
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingError);

            if (setting.Type == OverhaulSettingTypes.Other)
                return;

            switch (setting.Type)
            {
                case OverhaulSettingTypes.Bool:
                    int a = (bool)value ? 1 : 0;
                    PlayerPrefs.SetInt(setting.RawPath, a);
                    setting.Field.SetValue(null, (bool)value);
                    break;
                case OverhaulSettingTypes.Int:
                    int b = (int)value;
                    PlayerPrefs.SetInt(setting.RawPath, b);
                    setting.Field.SetValue(null, (int)value);
                    break;
                case OverhaulSettingTypes.Float:
                    float c = (float)value;
                    PlayerPrefs.SetFloat(setting.RawPath, c);
                    setting.Field.SetValue(null, (float)value);
                    break;
                case OverhaulSettingTypes.String:
                    string d = (string)value;
                    PlayerPrefs.SetString(setting.RawPath, d);
                    setting.Field.SetValue(null, (string)value);
                    break;
                default:
                    OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingSaveError);
                    break;
            }

            if (dispatchEvent) DispatchSettingsRefreshedEvent();

            PlayerPrefs.Save();
        }

        public static T GetPref<T>(in SettingInfo setting)
        {
            if (setting == null || setting.Error)
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingError);

            if (setting.Type == OverhaulSettingTypes.Other)
                return default;

            object result = null;
            switch (setting.Type)
            {
                case OverhaulSettingTypes.Bool:
                    result = PlayerPrefs.GetInt(setting.RawPath) == 1;
                    break;
                case OverhaulSettingTypes.Int:
                    result = PlayerPrefs.GetInt(setting.RawPath);
                    break;
                case OverhaulSettingTypes.Float:
                    result = PlayerPrefs.GetFloat(setting.RawPath);
                    break;
                case OverhaulSettingTypes.String:
                    result = PlayerPrefs.GetString(setting.RawPath);
                    break;
                default:
                    OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingGetError);
                    break;
            }
            return (T)result;
        }

        public static void DispatchSettingsRefreshedEvent() => OverhaulEventsController.DispatchEvent(OverhaulSettingsController.SettingChangedEventString);
    }
}
