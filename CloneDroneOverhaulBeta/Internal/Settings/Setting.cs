using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CDOverhaul
{
    public class SettingInfo
    {
        public string RawPath { get; set; }
        public object DefaultValue { get; set; }
        public FieldInfo Field { get; set; }

        public string Category { get; set; }
        public string Section { get; set; }
        public string Name { get; set; }

        public bool IsChildSetting { get; set; }

        public readonly List<string> ChildSettings = new List<string>();

        public SettingSliderParameters SliderParameters { get; set; }
        public SettingDropdownParameters DropdownParameters { get; set; }

        public SettingType Type { get; set; }

        public SettingInfo CanBeLockedBy { get; set; }
        public object ValueToUnlock { get; set; }
        public bool IsUnlocked()
        {
            return CanBeLockedBy == null || object.Equals(CanBeLockedBy.Field.GetValue(null), ValueToUnlock);
        }

        public bool ForceInputField { get; set; }

        public SettingEventDispatcher EventDispatcher { get; set; }

        public bool Error => Type == SettingType.None || Field == null || string.IsNullOrEmpty(RawPath);

        internal void SetUp<T>(in string path, in object defValue, in FieldInfo field, in SettingSliderParameters sliderParams = null, in SettingDropdownParameters dropdownParams = null)
        {
            RawPath = path;
            Type = GetSettingType(defValue);
            DefaultValue = defValue;
            Field = field;

            string[] array = path.Split('.');
            Category = array[0];
            Section = array[1];
            Name = array[2];

            SliderParameters = sliderParams;
            DropdownParameters = dropdownParams;

            if (Error)
            {
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingError);
            }

            TryAddPref(this);
            TuneUpValues();
        }

        internal void TuneUpValues()
        {
            if (Error)
            {
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingError);
            }

            if (Type == SettingType.Other)
            {
                return;
            }

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

        public static SettingType GetSettingType<T>()
        {
            return typeof(T) == typeof(bool)
                ? SettingType.Bool
                : typeof(T) == typeof(int)
                ? SettingType.Int
                : typeof(T) == typeof(float)
                ? SettingType.Float
                : typeof(T) == typeof(string) ? SettingType.String : typeof(T) == typeof(long) ? SettingType.Other : SettingType.None;
        }

        public static SettingType GetSettingType(in object @object)
        {
            return @object is bool
                ? SettingType.Bool
                : @object is int
                ? SettingType.Int
                : @object is float
                ? SettingType.Float
                : @object is string ? SettingType.String : @object is long ? SettingType.Other : SettingType.None;
        }

        public static void TryAddPref(in SettingInfo setting)
        {
            if (setting == null || setting.Error)
            {
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingError);
            }

            if (!PlayerPrefs.HasKey(setting.RawPath))
            {
                SavePref(setting, setting.DefaultValue);
            }
        }

        public static bool HasPref(in SettingInfo setting)
        {
            if (setting == null || setting.Error)
            {
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingError);
            }

            return PlayerPrefs.HasKey(setting.RawPath);
        }

        public static void SavePref(in SettingInfo setting, in object value, in bool dispatchEvent = true)
        {
            if (setting == null || setting.Error)
            {
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingError);
            }

            if (setting.Type == SettingType.Other)
            {
                return;
            }

            switch (setting.Type)
            {
                case SettingType.Bool:
                    int a = (bool)value ? 1 : 0;
                    PlayerPrefs.SetInt(setting.RawPath, a);
                    setting.Field.SetValue(null, (bool)value);
                    break;
                case SettingType.Int:
                    int b = (int)value;
                    PlayerPrefs.SetInt(setting.RawPath, b);
                    setting.Field.SetValue(null, (int)value);
                    break;
                case SettingType.Float:
                    float c = (float)value;
                    PlayerPrefs.SetFloat(setting.RawPath, c);
                    setting.Field.SetValue(null, (float)value);
                    break;
                case SettingType.String:
                    string d = (string)value;
                    PlayerPrefs.SetString(setting.RawPath, d);
                    setting.Field.SetValue(null, (string)value);
                    break;
                default:
                    OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingSaveError);
                    break;
            }

            if(dispatchEvent) DispatchSettingsRefreshedEvent();

            PlayerPrefs.Save();
        }

        public static T GetPref<T>(in SettingInfo setting)
        {
            if (setting == null || setting.Error)
            {
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingError);
            }

            if (setting.Type == SettingType.Other)
            {
                return default;
            }

            object result = null;
            switch (setting.Type)
            {
                case SettingType.Bool:
                    result = PlayerPrefs.GetInt(setting.RawPath) == 1;
                    break;
                case SettingType.Int:
                    result = PlayerPrefs.GetInt(setting.RawPath);
                    break;
                case SettingType.Float:
                    result = PlayerPrefs.GetFloat(setting.RawPath);
                    break;
                case SettingType.String:
                    result = PlayerPrefs.GetString(setting.RawPath);
                    break;
                default:
                    OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingGetError);
                    break;
            }
            return (T)result;
        }

        public static void DispatchSettingsRefreshedEvent()
        {
            OverhaulEventsController.DispatchEvent(SettingsController.SettingChangedEventString);
        }
    }
}
