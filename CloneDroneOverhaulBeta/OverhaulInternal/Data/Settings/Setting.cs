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

        public ESettingType Type { get; set; }

        public bool Error => Type == ESettingType.None || Field == null || string.IsNullOrEmpty(RawPath) || string.IsNullOrEmpty(Category) || string.IsNullOrEmpty(Section) || string.IsNullOrEmpty(Name);

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

            Field.SetValue(null, GetPref<object>(this));
        }

        public void ParentSettingToThis(in SettingInfo setting)
        {
            ChildSettings.Add(setting.RawPath);
            setting.IsChildSetting = true;
        }

        public static ESettingType GetSettingType<T>()
        {
            if (typeof(T) == typeof(bool))
            {
                return ESettingType.Bool;
            }
            if (typeof(T) == typeof(int))
            {
                return ESettingType.Int;
            }
            if (typeof(T) == typeof(float))
            {
                return ESettingType.Float;
            }
            if (typeof(T) == typeof(string))
            {
                return ESettingType.String;
            }
            return ESettingType.None;
        }

        public static ESettingType GetSettingType(in object @object)
        {
            if (@object is bool)
            {
                return ESettingType.Bool;
            }
            if (@object is int)
            {
                return ESettingType.Int;
            }
            if (@object is float)
            {
                return ESettingType.Float;
            }
            if (@object is string)
            {
                return ESettingType.String;
            }
            return ESettingType.None;
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

        public static void SavePref(in SettingInfo setting, in object value)
        {
            if (setting == null || setting.Error)
            {
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingError);
            }

            switch (setting.Type)
            {
                case ESettingType.Bool:
                    int a = (bool)value ? 1 : 0;
                    PlayerPrefs.SetInt(setting.RawPath, a);
                    break;
                case ESettingType.Int:
                    int b = (int)value;
                    PlayerPrefs.SetInt(setting.RawPath, b);
                    break;
                case ESettingType.Float:
                    float c = (float)value;
                    PlayerPrefs.SetFloat(setting.RawPath, c);
                    break;
                case ESettingType.String:
                    string d = (string)value;
                    PlayerPrefs.SetString(setting.RawPath, d);
                    break;
                default:
                    OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingSaveError);
                    break;
            }

            setting.Field.SetValue(null, value);
            DispatchSettingsRefreshedEvent();

            PlayerPrefs.Save();
        }

        public static T GetPref<T>(in SettingInfo setting)
        {
            if (setting == null || setting.Error)
            {
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_SettingError);
            }

            object result = null;
            switch (setting.Type)
            {
                case ESettingType.Bool:
                    result = PlayerPrefs.GetInt(setting.RawPath) == 1;
                    break;
                case ESettingType.Int:
                    result = PlayerPrefs.GetInt(setting.RawPath);
                    break;
                case ESettingType.Float:
                    result = PlayerPrefs.GetFloat(setting.RawPath);
                    break;
                case ESettingType.String:
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
            OverhaulEventManager.DispatchEvent(SettingsController.SettingChangedEventString);
        }
    }
}
