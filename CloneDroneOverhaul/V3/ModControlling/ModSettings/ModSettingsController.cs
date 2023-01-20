using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneDroneOverhaul.V3.Base
{
    public static class ModSettingsController
    {
        private static bool _hasAddedSettings;

        /// <summary>
        /// All settings in mod
        /// </summary>
        public static ModSetting[] Settings = new ModSetting[512];
        private static Dictionary<string, int> _cachedIndexes = new Dictionary<string, int>();
        public static Dictionary<string, List<string>> CachedSections = new Dictionary<string, List<string>>();
        private static int _currentArrayIndex = 0;

        internal static void Initialize()
        {
            if (!_hasAddedSettings)
            {
                _hasAddedSettings = true;

                if (OverhaulDescription.TEST_FEATURES_ENABLED)
                {
                    NewSetting("Test Category.TestSection.TestSetting", true);
                    NewSetting("Test Category.TestSection.TestIntSetting", 1, new SModSettingSliderSettings(0, 10, true));
                    NewSetting("Test Category.TestSection.TestFloatSetting", 0.5f, new SModSettingSliderSettings(0, 9.5f, false));
                    NewSetting("Test Category.TestSection.TestStringSetting", "Hello");
                    NewSetting("Test Category.TestSection.TestDropdownSetting", 1, null, new SModSettingDropdownSettings(typeof(GameMode), false));
                    NewSetting("Test Category.TestSection.TestDropdownKeycodeSetting", 1, null, new SModSettingDropdownSettings(typeof(UnityEngine.KeyCode), false));

                    AddChildToSetting("Test Category.TestSection.TestDropdownSetting", "Test Category.TestSection.TestDropdownKeycodeSetting");
                }

                NewSetting("Graphics.Settings.FPS Cap", 0, null, new SModSettingDropdownSettings(typeof(Graphics.EFPSCapValue), true));
                NewSetting("Graphics.Camera.Camera Roll", true);
            }
        }

        /// <summary>
        /// Add new setting to registry
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="defaultValue"></param>
        public static void NewSetting(in string path, in object defaultValue, in SModSettingSliderSettings? sliderSettings = null, in SModSettingDropdownSettings? dropdownSettings = null)
        {
            ModSetting newSetting = new ModSetting(path, defaultValue, sliderSettings, dropdownSettings);
            Settings[_currentArrayIndex] = newSetting;
            _cachedIndexes.Add(path, _currentArrayIndex);

            if (!CachedSections.ContainsKey(newSetting.SettingCategory))
            {
                CachedSections.Add(newSetting.SettingCategory, new List<string>());
                CachedSections[newSetting.SettingCategory].Add(newSetting.SettingSection);
            }
            else
            {
                if (!CachedSections[newSetting.SettingCategory].Contains(newSetting.SettingSection))
                {
                    CachedSections[newSetting.SettingCategory].Add(newSetting.SettingSection);
                }
            }

            _currentArrayIndex++;

            if (!ModDataController.HasPlayerPref(path))
            {
                ModDataController.SavePlayerPref(path, defaultValue);
            }
        }

        /// <summary>
        /// Get setting with path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ModSetting GetSetting(in string path)
        {
            return Settings[_cachedIndexes[path]];
        }

        /// <summary>
        /// Hide setting from main view
        /// </summary>
        /// <param name="setting"></param>
        public static void HideSetting(in string path)
        {
            ModSetting s1 = GetSetting(path);
            s1.IsHiddenFromMainView = true;
        }

        /// <summary>
        /// Attach <paramref name="childSetting"/> to <paramref name="targetSetting"/> and hide it from main view
        /// </summary>
        /// <param name="targetSetting"></param>
        /// <param name="childSetting"></param>
        public static void AddChildToSetting(in string targetSetting, in string childSetting)
        {
            ModSetting s1 = GetSetting(targetSetting);
            ModSetting s2 = GetSetting(childSetting);
            s1.AddChild(s2);
            s2.IsHiddenFromMainView = true;
        }

        /// <summary>
        /// Dublicate of <see cref="ModDataController.GetPlayerPref(in string, in EPlayerPrefType)"/>, but there's no need to specify pref type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settingValue"></param>
        /// <returns></returns>
        public static T GetSettingValue<T>(in string setting)
        {
            T result = default(T);

            result = (T)ModDataController.GetPlayerPref(setting, GetSetting(setting).Type);

            return result;
        }
    }
}
