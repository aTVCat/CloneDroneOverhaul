using CDOverhaul.HUD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulSettingsController
    {
        /// <summary>
        /// This event is sent if any setting value has changed
        /// </summary>
        public const string SettingChangedEventString = "OnSettingChanged";

        public const BindingFlags Flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// If the default value of a setting equals this one, the setting will be shown as button in UI
        /// </summary>
        public const long SettingEventDispatcherFlag = 10000000000000L;

        /// <summary>
        /// All existing settings
        /// </summary>
        private static readonly List<SettingInfo> m_Settings = new List<SettingInfo>();
        private static readonly Dictionary<string, OverhaulSettingDescription> m_SettingDescriptions = new Dictionary<string, OverhaulSettingDescription>();

        /// <summary>
        /// Categories, sections and settings to be hidden in settings menu
        /// </summary>
        private static readonly List<string> m_HiddenEntries = new List<string>() { "Player", "WeaponSkins", "Graphics.Rendering" };

        /// <summary>
        /// Loaded images from Assets/Settings/Ico directory
        /// </summary>
        private static readonly Dictionary<string, Sprite> m_CachedCategoryIcons = new Dictionary<string, Sprite>();
        private static Sprite m_UnknownCategoryIcon;

        /// <summary>
        /// UI instance
        /// </summary>
        public static ParametersMenu HUD;

        internal static void Initialize()
        {
            if (!OverhaulSessionController.GetKey<bool>("HasAddedOverhaulSettings"))
            {
                OverhaulSessionController.SetKey("HasAddedOverhaulSettings", true);

                List<OverhaulSettingAttribute> toParent = new List<OverhaulSettingAttribute>();
                Type[] allTypes = OverhaulMod.GetAllTypes();
                int typeIndex = 0;
                do
                {
                    Type currentType = allTypes[typeIndex];
                    FieldInfo[] allFields = currentType.GetFields(Flags);
                    if (allFields.IsNullOrEmpty())
                    {
                        typeIndex++;
                        continue;
                    }

                    int fieldIndex = 0;
                    do
                    {
                        FieldInfo currentField = allFields[fieldIndex];

                        OverhaulSettingAttribute mainAttribute = currentField.GetCustomAttribute<OverhaulSettingAttribute>();
                        if (mainAttribute == null)
                        {
                            fieldIndex++;
                            continue;
                        }

                        OverhaulSettingWithNotification notificationAttribute = currentField.GetCustomAttribute<OverhaulSettingWithNotification>();
                        OverhaulUpdatedSetting updatedSettingAttribute = currentField.GetCustomAttribute<OverhaulUpdatedSetting>();
                        OverhaulSettingSliderParameters sliderParametersAttribute = currentField.GetCustomAttribute<OverhaulSettingSliderParameters>();
                        OverhaulSettingDropdownParameters dropdownParametersAttribute = currentField.GetCustomAttribute<OverhaulSettingDropdownParameters>();
                        OverhaulSettingWithForcedInputField forcedInputFieldAttribute = currentField.GetCustomAttribute<OverhaulSettingWithForcedInputField>();
                        OverhaulSettingRequireUpdate requireUpdateAttribute = currentField.GetCustomAttribute<OverhaulSettingRequireUpdate>();

                        SettingInfo newSettingInfo = AddSetting(mainAttribute.SettingRawPath, mainAttribute.DefaultValue, currentField, updatedSettingAttribute);
                        newSettingInfo.SliderParameters = sliderParametersAttribute;
                        newSettingInfo.DropdownParameters = dropdownParametersAttribute;
                        newSettingInfo.ForceInputField = forcedInputFieldAttribute != null;
                        newSettingInfo.SendMessageOfType = notificationAttribute != null ? notificationAttribute.Type : (byte)0;
                        AddDescription(mainAttribute.SettingRawPath, mainAttribute.Description);

                        if (newSettingInfo.DefaultValue is long && (long)newSettingInfo.DefaultValue == SettingEventDispatcherFlag)
                            newSettingInfo.EventDispatcher = (OverhaulSettingWithEvent)currentField.GetValue(null);

                        if (mainAttribute.IsHidden || (requireUpdateAttribute != null && !requireUpdateAttribute.ShouldBeVisible()))
                            m_HiddenEntries.Add(newSettingInfo.RawPath);

                        if (!string.IsNullOrEmpty(mainAttribute.ParentSettingRawPath))
                            toParent.Add(mainAttribute);

                        fieldIndex++;
                    } while (fieldIndex < allFields.Length);

                    typeIndex++;
                } while (typeIndex < allTypes.Length);

                SetSettingDependency("Gameplay.Camera.View mode", "Gameplay.Camera.Sync camera with head rotation", 1);
                foreach (OverhaulSettingAttribute neededAttribute in toParent)
                {
                    SetSettingParent(neededAttribute.SettingRawPath, neededAttribute.ParentSettingRawPath);
                }

#if DEBUG
                DelegateScheduler.Instance.Schedule(delegate
                {
                    foreach (SettingInfo neededAttribute in m_Settings)
                    {
                        if (OverhaulLocalizationController.Error)
                        {
                            return;
                        }

                        if (!OverhaulLocalizationController.HasTranslation(ParametersMenu.SettingTranslationPrefix + neededAttribute.Name))
                        {
                            OverhaulLocalizationController.Localization.AddTranslation(ParametersMenu.SettingTranslationPrefix + neededAttribute.Name);
                            OverhaulLocalizationController.Localization.Translations["en"][ParametersMenu.SettingTranslationPrefix + neededAttribute.Name] = neededAttribute.Name;
                        }
                        if (!OverhaulLocalizationController.HasTranslation(ParametersMenu.SettingDescTranslationPrefix + neededAttribute.Name))
                        {
                            OverhaulLocalizationController.Localization.AddTranslation(ParametersMenu.SettingDescTranslationPrefix + neededAttribute.Name);
                        }
                        OverhaulSettingDescription desc = OverhaulSettingsController.GetSettingDescription(neededAttribute.RawPath);
                        if (desc != null) OverhaulLocalizationController.Localization.Translations["en"][ParametersMenu.SettingDescTranslationPrefix + neededAttribute.Name] = desc.Description;

                        if (!OverhaulLocalizationController.HasTranslation(ParametersMenu.SectionTranslationPrefix + neededAttribute.Section))
                        {
                            OverhaulLocalizationController.Localization.AddTranslation(ParametersMenu.SectionTranslationPrefix + neededAttribute.Section);
                            OverhaulLocalizationController.Localization.Translations["en"][ParametersMenu.SectionTranslationPrefix + neededAttribute.Section] = neededAttribute.Section;
                        }
                        if (!OverhaulLocalizationController.HasTranslation(ParametersMenu.SettingButtonTranslationPrefix + neededAttribute.Name))
                        {
                            OverhaulLocalizationController.Localization.AddTranslation(ParametersMenu.SettingButtonTranslationPrefix + neededAttribute.Name);
                            OverhaulLocalizationController.Localization.Translations["en"][ParametersMenu.SettingButtonTranslationPrefix + neededAttribute.Name] = neededAttribute.Name;
                        }

                        if (!OverhaulLocalizationController.HasTranslation(ParametersMenu.CategoryTranslationPrefix + neededAttribute.Category))
                        {
                            OverhaulLocalizationController.Localization.AddTranslation(ParametersMenu.CategoryTranslationPrefix + neededAttribute.Category);
                            OverhaulLocalizationController.Localization.Translations["en"][ParametersMenu.CategoryTranslationPrefix + neededAttribute.Category] = neededAttribute.Category;
                        }
                        if (!OverhaulLocalizationController.HasTranslation(ParametersMenu.CategoryDescTranslationPrefix + neededAttribute.Category))
                        {
                            OverhaulLocalizationController.Localization.AddTranslation(ParametersMenu.CategoryDescTranslationPrefix + neededAttribute.Category);
                            OverhaulLocalizationController.Localization.Translations["en"][ParametersMenu.CategoryDescTranslationPrefix + neededAttribute.Category] = GetCategoryDescription(neededAttribute.Category);
                        }
                    }
                }, 1f);
#endif
            }
            DelegateScheduler.Instance.Schedule(SettingInfo.DispatchSettingsRefreshedEvent, 0.1f);
        }

        internal static void CreateHUD()
        {
            OverhaulCanvasController h = OverhaulMod.Core.CanvasController;
            if (!h)
                return;

            HUD = h.AddHUD<ParametersMenu>(h.HUDModdedObject.GetObject<ModdedObject>(3));
        }

        /// <summary>
        /// Add a setting and get full info about one
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="defaultValue"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static SettingInfo AddSetting<T>(in string path, in T defaultValue, in FieldInfo field, in OverhaulUpdatedSetting formelyKnown = null)
        {
            SettingInfo newSetting = new SettingInfo();
            newSetting.SetUp<T>(path, defaultValue, field, formelyKnown);
            m_Settings.Add(newSetting);
            return newSetting;
        }

        public static void AddDescription(in string settingPath, in string description)
        {
            if (GetSetting(settingPath) == null)
                return;

            OverhaulSettingDescription desc = new OverhaulSettingDescription(description);
            if (!m_SettingDescriptions.ContainsKey(settingPath))
            {
                m_SettingDescriptions.Add(settingPath, desc);
            }
        }

        public static void SetSettingDependency(in string toDepend, in string targetSetting, in object targetValue)
        {
            SettingInfo info = GetSetting(targetSetting);
            SettingInfo info2 = GetSetting(toDepend);
            if (info == null || info2 == null)
                return;

            info.CanBeLockedBy = info2;
            info.ValueToUnlock = targetValue;
        }

        public static void SetSettingParent(in string settingPath, in string targetSettingPath)
        {
            SettingInfo s1 = GetSetting(settingPath, true);
            SettingInfo s2 = GetSetting(targetSettingPath, true);
            if (s1 == null || s2 == null)
                return;

            s2.ParentSettingToThis(s1);
            SetSettingDependency(targetSettingPath, settingPath, true);
        }

        public static Sprite GetSpriteForCategory(in string categoryName)
        {
            string path = OverhaulMod.Core.ModDirectory + "Assets/Settings/Ico/" + categoryName + "-S-16x16.png";
            bool exists = File.Exists(path);
            if (!exists)
            {
                path = OverhaulMod.Core.ModDirectory + "Assets/Settings/Ico/UnknownCategory.png";
                if (!File.Exists(path))
                {
                    return null;
                }
                else
                {
                    if (m_UnknownCategoryIcon != null)
                    {
                        return m_UnknownCategoryIcon;
                    }

                    Texture2D texture1 = new Texture2D(1, 1)
                    {
                        filterMode = FilterMode.Point
                    };
                    _ = texture1.LoadImage(File.ReadAllBytes(path), false);
                    texture1.Apply();

                    m_UnknownCategoryIcon = texture1.ToSprite();
                    return m_UnknownCategoryIcon;
                }
            }

            if (m_CachedCategoryIcons.ContainsKey(categoryName))
            {
                return m_CachedCategoryIcons[categoryName];
            }

            Texture2D texture = new Texture2D(1, 1);
            if (File.Exists(path))
            {
                byte[] content = File.ReadAllBytes(path);
                if (!content.IsNullOrEmpty())
                {
                    texture.filterMode = FilterMode.Point;
                    _ = texture.LoadImage(content, false);
                    texture.Apply();

                    Sprite sprite = texture.ToSprite();
                    m_CachedCategoryIcons.Add(categoryName, sprite);
                    return sprite;
                }
            }
            return null;
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
                        result.Add(s.Category);
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
                        result.Add(s.Category + "." + s.Section);
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
                        result.Add(s.RawPath);
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
                    return s;
            }
            return null;
        }

        public static OverhaulSettingDescription GetSettingDescription(in string path)
        {
            _ = m_SettingDescriptions.TryGetValue(path, out OverhaulSettingDescription result);
            return result;
        }

        public static string GetCategoryDescription(string category)
        {
            switch (category)
            {
                case "Graphics":
                    return "Change the visuals of the game or the way meshes are rendered by\nSome of the settings can reduce your FPS!";
                case "Optimization":
                    return "Reduce memory usage";
                case "Game interface":
                    return "Bring some new things to the game's HUD";
                case "Gameplay":
                    return "Customize the gameplay experience for yourself";
                case "Shortcuts":
                    return "Open menus during the game, being in the settings";
            }
            return string.Empty;
        }

        /// <summary>
        /// Check if the setting is hidden
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsEntryHidden(in string path)
        {
            if (OverhaulVersion.IsDebugBuild)
                return false;

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
                path1 = array[0] + "." + array[1];

            bool isSetting = array.Length == 3;
            if (isSetting)
                path1 = array[0] + "." + array[1] + "." + array[2];

            return m_HiddenEntries.Contains(path1);
        }

        public static void SetSettingValue(string rawPath, object value)
        {
            SettingInfo setting = GetSetting(rawPath, true);
            if (setting == null)
                return;

            SettingInfo.SavePref(setting, value);
        }

        public static void ResetSettingValue(string rawPath)
        {
            SettingInfo setting = GetSetting(rawPath, true);
            if (setting == null)
                return;

            SettingInfo.SavePref(setting, setting.DefaultValue);
        }
    }
}
