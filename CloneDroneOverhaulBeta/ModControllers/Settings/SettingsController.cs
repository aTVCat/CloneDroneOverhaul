using CDOverhaul.HUD;
using System.Collections.Generic;
using System.Reflection;

namespace CDOverhaul
{
    public static class SettingsController
    {
        public const string SettingChangedEventString = "OnSettingChanged";

        private static readonly List<SettingInfo> _settings = new List<SettingInfo>();
        private static readonly Dictionary<string, SettingDescription> _descriptions = new Dictionary<string, SettingDescription>();
        private static readonly List<string> _hiddenEntries = new List<string>() { "TestCat" };

        public static UISettingsMenu HUD;

        private static bool _hasAddedSettings;

        internal static void Initialize()
        {
            HUDControllers h = OverhaulBase.Core.HUDController;
            HUD = h.AddHUD<UISettingsMenu>(h.HUDModdedObject.GetObject<ModdedObject>(3));

            if (_hasAddedSettings)
            {
                return;
            }

            OverhaulEventManager.AddEvent(SettingChangedEventString);

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    var neededAttribute = field.GetCustomAttribute<OverhaulSettingAttribute>();
                    if (neededAttribute != null)
                    {
                        AddSetting(neededAttribute.MySetting, neededAttribute.DefaultValue, field);
                        if (neededAttribute.IsHidden)
                        {
                            _hiddenEntries.Add(neededAttribute.MySetting);
                        }
                    }
                }
            }

            AddDescription("TestCat.TestSec.TestSet", "Some description", "TestSet.png", null);
            AddDescription("Robots.Events.Death Animation", "Robots won't instantly freeze, instead, they will slow down", null, null);

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
        public static SettingInfo AddSetting<T>(in string path, in T defaultValue, in FieldInfo field)
        {
            SettingInfo newSetting = new SettingInfo();
            newSetting.SetUp<T>(path, defaultValue, field);
            _settings.Add(newSetting);
            return newSetting;
        }

        public static void AddDescription(in string settingPath, in string description, in string img43filename, in string img169filename)
        {
            if (GetSetting(settingPath) == null)
            {
                return;
            }
            SettingDescription desc = new SettingDescription(description, img43filename, img169filename);
            _descriptions.Add(settingPath, desc);
        }

        /// <summary>
        /// Get all available categories including hidden if <paramref name="includeHidden"/> is true
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllCategories(in bool includeHidden = false)
        {
            List<string> result = new List<string>();
            foreach (SettingInfo s in _settings)
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
            foreach (SettingInfo s in _settings)
            {
                if (s.Category == categoryToSearchIn && !result.Contains(s.Section))
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
            foreach (SettingInfo s in _settings)
            {
                if (s.Category == categoryToSearchIn && s.Section == sectionToSearchIn && !result.Contains(s.RawPath))
                {
                    if (!IsEntryHidden(s.RawPath) || includeHidden)
                    {
                        result.Add(s.RawPath);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get setting info by typing path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="includeHidden"></param>
        /// <returns></returns>
        public static SettingInfo GetSetting(in string path, in bool includeHidden = false)
        {
            foreach (SettingInfo s in _settings)
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
            SettingDescription result = null;
            _descriptions.TryGetValue(path, out result);
            return result;
        }

        /// <summary>
        /// Check if the setting is hidden
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsEntryHidden(in string path)
        {
            bool isCategory = !path.Contains(".");
            bool isSection = false;
            bool isSetting = false;
            string path1 = null;

            if (isCategory)
            {
                path1 = path;
                return _hiddenEntries.Contains(path1);
            }

            string[] array = path.Split('.');

            isSection = array.Length == 2;
            if (isSection)
            {
                path1 = array[0] + "." + array[1];
            }
            isSetting = array.Length == 3;
            if (isSetting)
            {
                path1 = array[0] + "." + array[1] + "." + array[2];
            }
            return _hiddenEntries.Contains(path1);
        }
    }
}
