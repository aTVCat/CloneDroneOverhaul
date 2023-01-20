using System;
using System.Collections.Generic;
using UnityEngine;
using ModLibrary;

namespace CloneDroneOverhaul.V3.Base
{
    public class ModSetting
    {
        public string SettingName { get; set; }
        public string SettingCategory { get; set; }
        public string SettingSection { get; set; }
        public int PathLength { get; set; }
        public string RawPath { get; set; }
        public object DefaultValue { get; set; }
        public EPlayerPrefType Type { get; set; }

        public List<ModSetting> ChildrenSettings { get; set; }
        public bool HasChildrenSettings { get; private set; }

        public bool IsHiddenFromMainView { get; set; }

        public MinMaxRange Slider_Range { get; set; }
        public bool Slider_Int { get; set; }

        public Type Dropdown_Type { get; set; }
        public List<UnityEngine.UI.Dropdown.OptionData> Dropdown_Options { get; set; }
        public bool Dropdown_Translate { get; set; }

        public ModSetting(string path, in object defaultValue, in SModSettingSliderSettings? sliderSettings = null, in SModSettingDropdownSettings? dropdownSettings = null)
        {
            string pathToParse = path;
            string[] str = pathToParse.Split('.');

            RawPath = path;
            SettingName = str[2];
            SettingCategory = str[0];
            SettingSection = str[1];
            PathLength = str.Length;
            DefaultValue = defaultValue;
            ChildrenSettings = new List<ModSetting>();
            HasChildrenSettings = false;
            IsHiddenFromMainView = false;
            Slider_Int = false;
            Slider_Range = null;
            Dropdown_Options = null;
            Dropdown_Type = null;

            if(sliderSettings != null)
            {
                Slider_Int = sliderSettings.Value.Int;
                Slider_Range = sliderSettings.Value.Range;
            }

            if(dropdownSettings != null)
            {
                Dropdown_Options = dropdownSettings.Value.Options;
                Dropdown_Type = dropdownSettings.Value.Type;
                Dropdown_Translate = dropdownSettings.Value.Translate;
            }

            Type type = defaultValue.GetType();
            EPlayerPrefType outType = EPlayerPrefType.None;
            if (type == typeof(bool))
            {
                outType = EPlayerPrefType.Bool;
            }
            if (type == typeof(int))
            {
                outType = EPlayerPrefType.Int;
            }
            if (type == typeof(float))
            {
                outType = EPlayerPrefType.Int;
            }
            if (type == typeof(string))
            {
                outType = EPlayerPrefType.String;
            }

            Type = outType;
        }

        public void AddChild(in ModSetting setting)
        {
            HasChildrenSettings = true;
            ChildrenSettings.Add(setting);
        }
    }
}
