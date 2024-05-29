using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod.Engine
{
    public class ModSettingsDataManager : Singleton<ModSettingsDataManager>
    {
        public const string SETTINGS_FILE_NAME = "modSettings.json";

        private string m_settingsFilePath;
        public string settingsFilePath
        {
            get
            {
                if(m_settingsFilePath == null)
                {
                    m_settingsFilePath = Path.Combine(ModCore.modDataFolder, SETTINGS_FILE_NAME);
                }
                return m_settingsFilePath;
            }
        }

        private ModSettingsDataContainer m_dataContainer;
        public ModSettingsDataContainer dataContainer
        {
            get
            {
                return m_dataContainer;
            }
            set
            {
                m_dataContainer = value;
            }
        }

        public bool areAnyChangesMade
        {
            get;
            set;
        }

        public override void Awake()
        {
            base.Awake();

            FileStream fileStream = createFileIfNotCreated();
            if(fileStream != null)
            {
                writeToFile(fileStream, createDefaultDataContainer());
                fileStream.Dispose();
            }
            else
            {
                ModSettingsDataContainer modSettingsDataContainer;
                try
                {
                    using (fileStream = new FileStream(settingsFilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] array = new byte[fileStream.Length];
                        fileStream.Read(array, 0, array.Length);

                        modSettingsDataContainer = ModJsonUtils.Deserialize<ModSettingsDataContainer>(ModIOUtils.GetString(array));
                        modSettingsDataContainer.FixValues();
                    }
                }
                catch
                {
                    if (fileStream != null)
                        fileStream.Dispose();

                    modSettingsDataContainer = new ModSettingsDataContainer();
                    modSettingsDataContainer.FixValues();
                }
                dataContainer = modSettingsDataContainer;
            }
        }

        private void OnApplicationQuit()
        {
            Save();
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

        private void writeToFile(FileStream fileStream, ModSettingsDataContainer modSettingsDataContainer)
        {
            byte[] bytes = ModIOUtils.GetBytes(ModJsonUtils.Serialize(modSettingsDataContainer));
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
            if(force || areAnyChangesMade)
            {
                areAnyChangesMade = false;
                ModJsonUtils.WriteStream(settingsFilePath, dataContainer);
            }
        }
    }
}
