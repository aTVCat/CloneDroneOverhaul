using OverhaulMod.Engine.Settings;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class ModSettingsManager : Singleton<ModSettingsManager>
    {
        public const string SETTING_CHANGED_EVENT = "OverhaulSettingChanged";

        public const string SETTING_NAME_TRANSLATION_PREFIX = "setting_name_";

        public const string SETTINGS_INFO_CONTAINER_FILE = "settingInfos.json";

        public static int ExtraResolutionLength;

        private List<ModSetting> _settings;
        private Dictionary<string, ModSetting> _nameToSetting;
        private Dictionary<string, ModSettingSubDescription> _idToDescription;

        private ModSettingElementsContainer _settingsInfos;

        public override void Awake()
        {
            base.Awake();

            _settings = new List<ModSetting>();
            _nameToSetting = new Dictionary<string, ModSetting>();
            _idToDescription = new Dictionary<string, ModSettingSubDescription>();

            loadSettings();
            loadInfos();
            loadDescriptions();
        }

        private void loadInfos()
        {
            ModSettingElementsContainer infosContainer;
            string path = Path.Combine(ModCore.DataFolder, SETTINGS_INFO_CONTAINER_FILE);
            if (File.Exists(path))
            {
                try
                {
                    infosContainer = ModJsonUtils.DeserializeStream<ModSettingElementsContainer>(path);
                }
                catch (Exception)
                {
                    infosContainer = new ModSettingElementsContainer();
                }
            }
            else
            {
                infosContainer = new ModSettingElementsContainer();
            }

            _settingsInfos = infosContainer;
        }

        private void loadDescriptions()
        {
            _idToDescription.Clear();

            string fn = Path.Combine(ModCore.DataFolder, "settingDescriptions.txt");
            if (File.Exists(fn))
            {
                string content;
                try
                {
                    content = ModFileUtils.ReadText(fn);
                }
                catch (Exception)
                {
                    return;
                }

                string[] idWithValues = content.Split(Environment.NewLine.ToCharArray());
                foreach (string entry in idWithValues)
                {
                    string[] split = entry.Split(' ');
                    if (split.Length == 2)
                    {
                        string settingId = split[0];
                        string sub = split[1];

                        if (!sub.IsNullOrEmpty())
                        {
                            string[] subSplit = sub.Split(',');
                            if (subSplit.Length == 2)
                            {
                                string typeText = subSplit[0];
                                string valueText = subSplit[1];

                                if (int.TryParse(typeText, out int type) && int.TryParse(valueText, out int value))
                                {
                                    _idToDescription.Add(settingId, new ModSettingSubDescription(type, value));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void loadSettings()
        {
            foreach (System.Type type in ModCache.ModAssembly.GetTypes())
            {
                foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    ModSetting modSetting = CreateSettingFromField(fieldInfo);
                    if (modSetting == null)
                        continue;

                    _settings.Add(modSetting);
                    _nameToSetting.Add(modSetting.Name, modSetting);
                }
            }
        }

        public void SaveElementDescriptions()
        {
            ModJsonUtils.WriteStream(Path.Combine(ModCore.DataFolder, SETTINGS_INFO_CONTAINER_FILE), _settingsInfos);
        }

        public string GetSubDescription(string settingId)
        {
            if (_idToDescription.ContainsKey(settingId))
            {
                ModSettingSubDescription modSettingSubDescription = _idToDescription[settingId];
                if (modSettingSubDescription.Type == 0)
                {
                    string postfix;
                    switch (modSettingSubDescription.Value)
                    {
                        case 1:
                            postfix = "Low".AddColor(Color.green);
                            break;
                        case 2:
                            postfix = "Medium".AddColor(Color.yellow);
                            break;
                        case 3:
                            postfix = "High".AddColor(Color.yellow);
                            break;
                        case 4:
                            postfix = "Very high".AddColor(Color.red);
                            break;
                        default:
                            postfix = "N/A".AddColor(Color.white);
                            break;
                    }
                    return $"Performance impact: {postfix}";
                }
            }
            return null;
        }

        public bool HasSettingWithName(string name)
        {
            if (name.StartsWith("OverhaulMod."))
                name = name.Substring("OverhaulMod.".Length);

            return _nameToSetting.ContainsKey(name);
        }

        public ModSetting GetSetting(string name)
        {
            if (name.StartsWith("OverhaulMod."))
                name = name.Substring("OverhaulMod.".Length);

            return _nameToSetting.TryGetValue(name, out ModSetting modSetting) ? modSetting : null;
        }

        public List<ModSetting> GetSettings()
        {
            return _settings;
        }

        public List<ModSetting> GetSettings(ModSetting.Tags tag)
        {
            List<ModSetting> result = new List<ModSetting>();
            foreach (ModSetting modSetting in _settings)
            {
                if (modSetting.Tag == tag)
                    result.Add(modSetting);
            }
            return result;
        }

        public void ResetSettings()
        {
            foreach (ModSetting setting in _settings)
                setting.SetValue(setting.DefaultValue);

            ModSettingsDataManager.Instance.Save();
        }

        public void AddSettingValueChangedListener(Action<object> action, string settingId)
        {
            ModSetting setting = GetSetting(settingId);
            action.Invoke(setting.GetFieldValue());
            setting.ValueChangedEvent += action;
        }

        public void RemoveSettingValueChangedListener(Action<object> action, string settingId)
        {
            ModSetting setting = GetSetting(settingId);
            setting.ValueChangedEvent -= action;
        }

        public ModSetting CreateSettingFromField(FieldInfo field, bool setFieldValue = true)
        {
            if (field == null)
                return null;

            ModSettingAttribute modSettingAttribute = field.GetCustomAttribute<ModSettingAttribute>();
            if (modSettingAttribute == null || modSettingAttribute.Name.IsNullOrEmpty() || HasSettingWithName(modSettingAttribute.Name))
                return null;

            ModSetting.ValueTypes valueType;
            if (field.FieldType == typeof(bool))
                valueType = ModSetting.ValueTypes.Bool;
            else if (field.FieldType == typeof(int) || field.FieldType.IsEnum)
                valueType = ModSetting.ValueTypes.Int;
            else if (field.FieldType == typeof(float))
                valueType = ModSetting.ValueTypes.Float;
            else if (field.FieldType == typeof(string))
                valueType = ModSetting.ValueTypes.String;
            else
                return null;

            ModSetting setting = new ModSetting
            {
                Name = modSettingAttribute.Name,
                DefaultValue = modSettingAttribute.DefaultValue,
                Tag = modSettingAttribute.Tag,
                ValueType = valueType,
                Field = field,
                RequiresRestarting = field.GetCustomAttribute<ModSettingRequireRestartAttribute>() != null
            };

            if (setFieldValue)
                field.SetValue(null, setting.GetValue());

            return setting;
        }

        public void SetSettingValueFromUI(string name, object value)
        {
            ModSetting modSetting = GetSetting(name);
            if (modSetting != null)
            {
                modSetting.SetValueFromUI(value);
            }
        }

        public static bool GetBoolValue(string name)
        {
            return (bool)Instance.GetSetting(name).GetFieldValue();
        }

        public static int GetIntValue(string name)
        {
            return (int)Instance.GetSetting(name).GetFieldValue();
        }

        public static float GetFloatValue(string name)
        {
            return (float)Instance.GetSetting(name).GetFieldValue();
        }

        public static string GetStringValue(string name)
        {
            return (string)Instance.GetSetting(name).GetFieldValue();
        }

        public static void ResetValue(string name, bool fromUi = false)
        {
            ModSetting setting = Instance.GetSetting(name);
            if (fromUi)
            {
                setting.SetValueFromUI(setting.DefaultValue);
                return;
            }
            setting.SetValue(setting.DefaultValue);
        }

        public static void SetBoolValue(string name, bool value, bool fromUi = false)
        {
            if (fromUi)
            {
                Instance.GetSetting(name).SetValueFromUI(value);
                return;
            }
            Instance.GetSetting(name).SetValue(value);
        }

        public static void SetIntValue(string name, int value, bool fromUi = false)
        {
            if (fromUi)
            {
                Instance.GetSetting(name).SetValueFromUI(value);
                return;
            }
            Instance.GetSetting(name).SetValue(value);
        }

        public static void SetFloatValue(string name, float value, bool fromUi = false)
        {
            if (fromUi)
            {
                Instance.GetSetting(name).SetValueFromUI(value);
                return;
            }
            Instance.GetSetting(name).SetValue(value);
        }

        public static void SetStringValue(string name, string value, bool fromUi = false)
        {
            if (fromUi)
            {
                Instance.GetSetting(name).SetValueFromUI(value);
                return;
            }
            Instance.GetSetting(name).SetValue(value);
        }
    }
}
