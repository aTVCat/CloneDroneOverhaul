using System;
using UnityEngine;

namespace CDOverhaul
{
    public class Setting
    {
        public string RawPath { get; set; }
        public object DefaultValue { get; set; }

        public string Category { get; set; }
        public string Section { get; set; }
        public string Name { get; set; }

        public ESettingType Type { get; set; }

        public bool Error => Type == ESettingType.None || string.IsNullOrEmpty(RawPath) || string.IsNullOrEmpty(Category) || string.IsNullOrEmpty(Section) || string.IsNullOrEmpty(Name);

        internal void SetUp<T>(in string path, in object defValue)
        {
            RawPath = path;
            Type = GetSettingType<T>();
            DefaultValue = defValue;

            string[] array = path.Split('.');
            Category = array[0];
            Section = array[1];
            Name = array[2];

            if (Error)
            {
                throw new Exception(OverhaulExceptions.Exc_SettingError);
            }

            TryAddPref(this);
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

        public static void TryAddPref(in Setting setting)
        {
            if (setting == null || setting.Error)
            {
                throw new Exception(OverhaulExceptions.Exc_SettingError);
                return;
            }

            if (!PlayerPrefs.HasKey(setting.RawPath))
            {
                SavePref(setting, setting.DefaultValue);
            }
        }

        public static bool HasPref(in Setting setting)
        {
            if (setting == null || setting.Error)
            {
                throw new Exception(OverhaulExceptions.Exc_SettingError);
                return false;
            }

            return PlayerPrefs.HasKey(setting.RawPath);
        }

        public static void SavePref(in Setting setting, in object value)
        {
            if (setting == null || setting.Error)
            {
                throw new Exception(OverhaulExceptions.Exc_SettingError);
                return;
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
                    throw new Exception(OverhaulExceptions.Exc_SettingSaveError);
                    break;
            }

            PlayerPrefs.Save();
        }

        public static object GetPref(in Setting setting)
        {
            if (setting == null || setting.Error)
            {
                throw new Exception(OverhaulExceptions.Exc_SettingError);
                return null;
            }

            switch (setting.Type)
            {
                case ESettingType.Bool:
                    return PlayerPrefs.GetInt(setting.RawPath) == 1 ? true : false;
                    break;
                case ESettingType.Int:
                    return PlayerPrefs.GetInt(setting.RawPath);
                    break;
                case ESettingType.Float:
                    return PlayerPrefs.GetFloat(setting.RawPath);
                    break;
                case ESettingType.String:
                    return PlayerPrefs.GetString(setting.RawPath);
                    break;
                default:
                    throw new Exception(OverhaulExceptions.Exc_SettingGetError);
                    break;
            }
            return null;
        }
    }
}
