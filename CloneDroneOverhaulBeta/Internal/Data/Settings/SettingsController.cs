using CDOverhaul.HUD;
using System.Collections.Generic;
using System.Reflection;

namespace CDOverhaul
{
    public static class SettingsController
    {
        public const string SettingChangedEventString = "OnSettingChanged";

        private static readonly List<SettingInfo> m_Settings = new List<SettingInfo>();
        private static readonly Dictionary<string, SettingDescription> m_SettingDescriptions = new Dictionary<string, SettingDescription>();
        private static readonly List<string> m_HiddenEntries = new List<string>() { "WeaponSkins" };

        public static OverhaulParametersMenu HUD;

        private static bool _hasAddedSettings;

        internal static void Initialize()
        {
            OverhaulCanvasController h = OverhaulMod.Core.HUDController;
            HUD = h.AddHUD<OverhaulParametersMenu>(h.HUDModdedObject.GetObject<ModdedObject>(3));

            if (_hasAddedSettings)
            {
                return;
            }

            foreach (System.Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (FieldInfo field in type.GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    OverhaulSettingAttribute neededAttribute = field.GetCustomAttribute<OverhaulSettingAttribute>();
                    if (neededAttribute != null)
                    {
                        SettingSliderParameters sliderParams = field.GetCustomAttribute<SettingSliderParameters>();
                        SettingDropdownParameters dropdownParams = field.GetCustomAttribute<SettingDropdownParameters>();
                        SettingInfo info = AddSetting(neededAttribute.SettingRawPath, neededAttribute.DefaultValue, field, sliderParams, dropdownParams);
                        if (neededAttribute.IsHidden)
                        {
                            m_HiddenEntries.Add(info.RawPath);
                        }
                        if (!string.IsNullOrEmpty(neededAttribute.Description))
                        {
                            AddDescription(neededAttribute.SettingRawPath, neededAttribute.Description, neededAttribute.Img4_3Path, neededAttribute.Img16_9Path);
                        }
                        if (!string.IsNullOrEmpty(neededAttribute.ParentSettingRawPath))
                        {
                            ParentSetting(neededAttribute.SettingRawPath, neededAttribute.ParentSettingRawPath);
                        }
                    }
                }
            }

            DelegateScheduler.Instance.Schedule(SettingInfo.DispatchSettingsRefreshedEvent, 0.1f);

            _hasAddedSettings = true;
        }

        /// <summary>
        /// Add a setting and get full info about one
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="defaultValue"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static SettingInfo AddSetting<T>(in string path, in T defaultValue, in FieldInfo field, in SettingSliderParameters sliderParams = null, in SettingDropdownParameters dropdownParams = null)
        {
            SettingInfo newSetting = new SettingInfo();
            newSetting.SetUp<T>(path, defaultValue, field, sliderParams, dropdownParams);
            m_Settings.Add(newSetting);
            return newSetting;
        }

        public static void AddDescription(in string settingPath, in string description, in string img43filename, in string img169filename)
        {
            if (GetSetting(settingPath) == null)
            {
                return;
            }
            SettingDescription desc = new SettingDescription(description, img43filename, img169filename);
            m_SettingDescriptions.Add(settingPath, desc);
        }

        public static void ParentSetting(in string settingPath, in string targetSettingPath)
        {
            SettingInfo s1 = GetSetting(settingPath, true);
            SettingInfo s2 = GetSetting(targetSettingPath, true);
            s2.ParentSettingToThis(s1);
        }

        /// <summary>
        /// Get all available categories including hidden if <paramref name="includeHidden"/> is true
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllCategories(in bool includeHidden = false)
        {
            List<string> result = new List<string>();
            foreach (SettingInfo s in m_Settings)
            {
                if (!result.Contains(s.Category))
                {
                    if (!IsEntryHidden(s.Category) || includeHidden)
                    {
                        result.Add(s.Category);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all available sections under category including hidden if <paramref name="includeHidden"/> is true
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllSections(in string categoryToSearchIn, in bool includeHidden = false)
        {
            List<string> result = new List<string>();
            foreach (SettingInfo s in m_Settings)
            {
                if (s.Category == categoryToSearchIn && !result.Contains(s.Category + "." + s.Section))
                {
                    if (!IsEntryHidden(s.Category + "." + s.Section) || includeHidden)
                    {
                        result.Add(s.Category + "." + s.Section);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get all available settings including hidden if <paramref name="includeHidden"/> is true
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllSettings(in string categoryToSearchIn, in string sectionToSearchIn, in bool includeHidden = false)
        {
            List<string> result = new List<string>();
            foreach (SettingInfo s in m_Settings)
            {
                if (s.Category == categoryToSearchIn && s.Section == sectionToSearchIn && !result.Contains(s.RawPath))
                {
                    if ((!IsEntryHidden(s.RawPath) || includeHidden) && !s.IsChildSetting)
                    {
                        result.Add(s.RawPath);
                    }
                }
            }
            return result;
        }

        public static List<string> GetChildrenSettings(in string rawPath)
        {
            return GetSetting(rawPath, true).ChildSettings;
        }

        /// <summary>
        /// Get setting info by typing path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="includeHidden"></param>
        /// <returns></returns>
        public static SettingInfo GetSetting(in string path, in bool includeHidden = false)
        {
            foreach (SettingInfo s in m_Settings)
            {
                if (s.RawPath == path && (!IsEntryHidden(s.RawPath) || includeHidden))
                {
                    return s;
                }
            }
            return null;
        }


        public static SettingDescription GetSettingDescription(in string path)
        {
            _ = m_SettingDescriptions.TryGetValue(path, out SettingDescription result);
            return result;
        }

        /// <summary>
        /// Check if the setting is hidden
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsEntryHidden(in string path)
        {
            if (OverhaulVersion.IsDebugBuild)
            {
                return false;
            }

            bool isCategory = !path.Contains(".");
            string path1 = null;

            if (isCategory)
            {
                path1 = path;
                return m_HiddenEntries.Contains(path1);
            }

            string[] array = path.Split('.');

            bool isSection = array.Length == 2;
            if (isSection)
            {
                path1 = array[0] + "." + array[1];
            }

            bool isSetting = array.Length == 3;
            if (isSetting)
            {
                path1 = array[0] + "." + array[1] + "." + array[2];
            }
            return m_HiddenEntries.Contains(path1);
        }
    }
}
