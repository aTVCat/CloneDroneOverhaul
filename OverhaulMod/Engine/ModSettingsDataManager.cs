using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace OverhaulMod.Engine
{
    public class ModSettingsDataManager : Singleton<ModSettingsDataManager>
    {
        public const string SETTINGS_FILE_NAME = "Settings.json";

        private string _settingsFilePath;
        public string SettingsFilePath
        {
            get
            {
                if (_settingsFilePath == null)
                {
                    _settingsFilePath = Path.Combine(ModDirectories.ModUserDataFolder, SETTINGS_FILE_NAME);
                }
                return _settingsFilePath;
            }
        }

        private ModSettingsDataContainer _dataContainer;

        private bool _hasToSaveFile;

        public override void Awake()
        {
            base.Awake();

            ModSettingsDataContainer modSettingsDataContainer;
            string path = SettingsFilePath;
            if (File.Exists(path))
            {
                try
                {
                    modSettingsDataContainer = ModJsonUtils.DeserializeStreamFormatted<ModSettingsDataContainer>(SettingsFilePath);
                }
                catch (Exception)
                {
                    modSettingsDataContainer = new ModSettingsDataContainer();
                }
            }
            else
            {
                modSettingsDataContainer = new ModSettingsDataContainer();
            }
            _dataContainer = modSettingsDataContainer;
            _dataContainer.FixValues();
        }

        private void Start()
        {
            ModSettingsDataContainer dataContaier = _dataContainer;
            if (dataContaier == null) return;

            ModSettingsManager modSettingsManager = ModSettingsManager.Instance;
            foreach (string key in new List<string>(dataContaier.IntValues.Keys))
            {
                if (!modSettingsManager.HasSetting(key))
                {
                    dataContaier.IntValues.Remove(key);
                    _hasToSaveFile = true;
                }
            }
            foreach (string key in new List<string>(dataContaier.FloatValues.Keys))
            {
                if (!modSettingsManager.HasSetting(key))
                {
                    dataContaier.FloatValues.Remove(key);
                    _hasToSaveFile = true;
                }
            }
            foreach (string key in new List<string>(dataContaier.StringValues.Keys))
            {
                if (!modSettingsManager.HasSetting(key))
                {
                    dataContaier.StringValues.Remove(key);
                    _hasToSaveFile = true;
                }
            }

            if (_hasToSaveFile)
            {
                ModDebug.Log("Removed unused setting keys!");
            }
            Save();
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        public ModSettingsDataContainer CreateDataContainerForExport()
        {
            ModSettingsDataContainer modSettingsDataContainer = new ModSettingsDataContainer();
            modSettingsDataContainer.FixValues();

            foreach (ModSetting setting in ModSettingsManager.Instance.GetSettings())
            {
                if (setting.Tag.HasFlag(ModSetting.Tags.IgnoreExport))
                    continue;

                switch (setting.ValueType)
                {
                    case ModSetting.ValueTypes.Bool:
                        modSettingsDataContainer.IntValues.Add(setting.ID, ((bool)setting.GetValue()) ? 1 : 0);
                        break;
                    case ModSetting.ValueTypes.Int:
                        modSettingsDataContainer.IntValues.Add(setting.ID, (int)setting.GetValue());
                        break;
                    case ModSetting.ValueTypes.Float:
                        modSettingsDataContainer.FloatValues.Add(setting.ID, (float)setting.GetValue());
                        break;
                    case ModSetting.ValueTypes.String:
                        modSettingsDataContainer.StringValues.Add(setting.ID, (string)setting.GetValue());
                        break;
                }
            }

            return modSettingsDataContainer;
        }

        public ModSettingsDataContainer GetDataContainer() => _dataContainer;

        public void SetInt(string key, int value)
        {
            if (_dataContainer.IntValues.ContainsKey(key) && _dataContainer.IntValues[key] == value) return;

            _hasToSaveFile = true;
            _dataContainer.SetInt(key, value);
        }

        public void SetFloat(string key, float value)
        {
            if (_dataContainer.FloatValues.ContainsKey(key) && _dataContainer.FloatValues[key] == value) return;

            _hasToSaveFile = true;
            _dataContainer.SetFloat(key, value);
        }

        public void SetString(string key, string value)
        {
            if (_dataContainer.StringValues.ContainsKey(key) && _dataContainer.StringValues[key] == value) return;

            _hasToSaveFile = true;
            _dataContainer.SetString(key, value);
        }

        public int GetInt(string key, int defaultValue) => _dataContainer.GetInt(key, defaultValue);

        public float GetFloat(string key, float defaultValue) => _dataContainer.GetFloat(key, defaultValue);

        public string GetString(string key, string defaultValue) => _dataContainer.GetString(key, defaultValue);

        public void Save(bool force = false)
        {
            if (force || _hasToSaveFile)
            {
                _hasToSaveFile = false;
                ModJsonUtils.WriteStreamFormatted(SettingsFilePath, _dataContainer);
            }
        }
    }
}