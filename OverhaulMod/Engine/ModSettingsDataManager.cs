using OverhaulMod.Utils;
using System.IO;

namespace OverhaulMod.Engine
{
    public class ModSettingsDataManager : Singleton<ModSettingsDataManager>
    {
        public const string SETTINGS_FILE_NAME_OLD = "modSettings.json";
        public const string SETTINGS_FILE_NAME = "Settings.json";

        private string m_settingsFilePathOld;
        public string settingsFilePathOld
        {
            get
            {
                if (m_settingsFilePathOld == null)
                {
                    m_settingsFilePathOld = Path.Combine(ModCore.modUserDataFolder, SETTINGS_FILE_NAME_OLD);
                }
                return m_settingsFilePathOld;
            }
        }

        private string m_settingsFilePath;
        public string settingsFilePath
        {
            get
            {
                if (m_settingsFilePath == null)
                {
                    m_settingsFilePath = Path.Combine(ModCore.modUserDataFolder, SETTINGS_FILE_NAME);
                }
                return m_settingsFilePath;
            }
        }

        public ModSettingsDataContainer dataContainer { get; set; }

        public bool areAnyChangesMade
        {
            get;
            set;
        }

        public override void Awake()
        {
            base.Awake();

            renameOldFile();

            FileStream fileStream = createFileIfNotCreated();
            if (fileStream != null)
            {
                writeToFile(fileStream, createDefaultDataContainer());
            }
            else
            {
                ModSettingsDataContainer modSettingsDataContainer;
                try
                {
                    fileStream = new FileStream(settingsFilePath, FileMode.Open, FileAccess.Read);
                    byte[] array = new byte[fileStream.Length];
                    _ = fileStream.Read(array, 0, array.Length);

                    modSettingsDataContainer = ModJsonUtils.Deserialize<ModSettingsDataContainer>(ModFileUtils.GetString(array));
                    modSettingsDataContainer.FixValues();
                }
                catch
                {
                    modSettingsDataContainer = new ModSettingsDataContainer();
                    modSettingsDataContainer.FixValues();
                }
                dataContainer = modSettingsDataContainer;
            }
            if (fileStream != null)
                fileStream.Dispose();
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
                if (setting.tag.HasFlag(ModSetting.Tag.IgnoreExport))
                    continue;

                switch (setting.valueType)
                {
                    case ModSetting.ValueType.Bool:
                        modSettingsDataContainer.IntValues.Add(setting.GetPlayerPrefKey(), ((bool)setting.GetValue()) ? 1 : 0);
                        break;
                    case ModSetting.ValueType.Int:
                        modSettingsDataContainer.IntValues.Add(setting.GetPlayerPrefKey(), (int)setting.GetValue());
                        break;
                    case ModSetting.ValueType.Float:
                        modSettingsDataContainer.FloatValues.Add(setting.GetPlayerPrefKey(), (float)setting.GetValue());
                        break;
                    case ModSetting.ValueType.String:
                        modSettingsDataContainer.StringValues.Add(setting.GetPlayerPrefKey(), (string)setting.GetValue());
                        break;
                }
            }

            return modSettingsDataContainer;
        }

        private ModSettingsDataContainer createDefaultDataContainer()
        {
            ModSettingsDataContainer modSettingsDataContainer = new ModSettingsDataContainer();
            modSettingsDataContainer.FixValues();
            dataContainer = modSettingsDataContainer;
            return modSettingsDataContainer;
        }

        private FileStream createFileIfNotCreated()
        {
            if (!File.Exists(settingsFilePath))
            {
                return File.Create(settingsFilePath);
            }
            return null;
        }

        private void renameOldFile()
        {
            string oldPath = settingsFilePathOld;
            string newPath = settingsFilePath;
            if (File.Exists(oldPath) && !File.Exists(newPath))
            {
                File.Move(oldPath, newPath);
            }
        }

        private void writeToFile(FileStream fileStream, ModSettingsDataContainer modSettingsDataContainer)
        {
            byte[] bytes = ModFileUtils.GetBytes(ModJsonUtils.Serialize(modSettingsDataContainer));
            fileStream.Write(bytes, 0, bytes.Length);
        }

        public void SetInt(string key, int value)
        {
            areAnyChangesMade = true;
            dataContainer.SetInt(key, value);
        }

        public void SetFloat(string key, float value)
        {
            areAnyChangesMade = true;
            dataContainer.SetFloat(key, value);
        }

        public void SetString(string key, string value)
        {
            areAnyChangesMade = true;
            dataContainer.SetString(key, value);
        }

        public int GetInt(string key, int defaultValue)
        {
            return dataContainer.GetInt(key, defaultValue);
        }

        public float GetFloat(string key, float defaultValue)
        {
            return dataContainer.GetFloat(key, defaultValue);
        }

        public string GetString(string key, string defaultValue)
        {
            return dataContainer.GetString(key, defaultValue);
        }

        public void Save(bool force = false)
        {
            if (force || areAnyChangesMade)
            {
                areAnyChangesMade = false;
                ModJsonUtils.WriteStream(settingsFilePath, dataContainer);
            }
        }
    }
}
