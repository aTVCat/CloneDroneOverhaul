using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CloneDroneOverhaul.Modules
{
    public class ModDataManager : ModuleBase
    {
        public static string Mod_DataFolder
        {
            get
            {
                return Application.persistentDataPath + "/CloneDroneOverhaul/";
            }
        }
        public static string Mod_Folder
        {
            get
            {
                return OverhaulMain.Instance.ModInfo.FolderPath;
            }
        }
        public static string Mod_TempFolder
        {
            get
            {
                return Mod_DataFolder + "/Temp/";
            }
        }
        public static string Addons_Folder
        {
            get
            {
                return Mod_DataFolder + "Addons/";
            }
        }
        public static string Data_Folder
        {
            get
            {
                return Mod_DataFolder + "SavedData/";
            }
        }
        public static string DublicatedLevelsFolder
        {
            get
            {
                return Mod_DataFolder + "CopiedLevels/";
            }
        }
        public static string AddonsCompliedDlls
        {
            get
            {
                return Mod_DataFolder + "CompiledDlls/";
            }
        }
        public static string FileExtension
        {
            get
            {
                return ".cdosave";
            }
        }
        public bool CanOthersAddData()
        {
            return this != null && this._hasInitialized;
        }
        private bool _hasInitialized;

        public override void Start()
        {
            checkFolders();
            this._hasInitialized = true;
        }
        private void checkFolders()
        {
            string[] folders = new string[]
            {
                Mod_DataFolder,
                Data_Folder,
                Addons_Folder,
                Mod_TempFolder,
                AddonsCompliedDlls,
                DublicatedLevelsFolder
            };

            CreateFoldersIfNeeded(folders);
        }

        public T LoadData<T>(string path) where T : class
        {
            T result = null;
            result = ByteSaver.FromByteArray<T>(File.ReadAllBytes(path));
            return result;
        }
        public void SaveData<T>(T obj, string path) where T : class
        {
            byte[] array = null;
            array = ByteSaver.ObjectToByteArray(obj);
            File.WriteAllBytes(path, array);
        }
        public T CreateInstanceOfDataClass<T>(bool loadData, bool loadDataNextFrame) where T : DataBase
        {
            T obj = Activator.CreateInstance<T>();
            obj.DataManagerReference = this;
            if (loadData)
            {
                if (loadDataNextFrame)
                {
                    OverhaulMain.Timer.AddNoArgActionToCompleteNextFrame(obj.LoadData);
                }
                else
                {
                    obj.LoadData();
                }
            }
            return obj;
        }

        /// <summary>
        /// Creates a folder if one doesn't exist
        /// </summary>
        /// <param name="path"></param>
        public void CreateFolderIfNeeded(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Same as method above, but for multiple folders
        /// </summary>
        /// <param name="paths"></param>
        public void CreateFoldersIfNeeded(string[] paths)
        {
            foreach (string path in paths)
            {
                CreateFolderIfNeeded(path);
            }
        }
    }

    public class DataBase
    {
        internal protected ModDataManager DataManagerReference;

        public void LoadData()
        {
            if (!checkFolders())
            {
                return;
            }
            TryLoadData();
        }
        protected virtual void TryLoadData()
        {

        }
        public virtual void TrySaveData()
        {

        }
        private bool checkFolders()
        {
            return CheckFolders();
        }
        protected virtual bool CheckFolders()
        {
            ModuleManagement.ShowError_Type2(GetType().ToString() + " Override error", "DataBase subclass must override ChechFolders method");
            return false;
        }
    }

    public class ModdedLevelEditorSaveData : DataBase
    {
        public ModdedLevelEditorSaveData.Data LEData;

        [Serializable]
        public class Data
        {
        }

        protected override void TryLoadData()
        {
            LEData = base.DataManagerReference.LoadData<ModdedLevelEditorSaveData.Data>(ModDataManager.Data_Folder + "ModdedLevelEditorData" + ModDataManager.FileExtension);
        }

        protected override bool CheckFolders()
        {
            if (!File.Exists(ModDataManager.Data_Folder + "ModdedLevelEditorData" + ModDataManager.FileExtension))
            {
                ModdedLevelEditorSaveData.Data data = new ModdedLevelEditorSaveData.Data();
                LEData = data;
                TrySaveData();
                return false;
            }
            return true;
        }

        public override void TrySaveData()
        {
            base.DataManagerReference.SaveData<ModdedLevelEditorSaveData.Data>(LEData, ModDataManager.Data_Folder + "ModdedLevelEditorData" + ModDataManager.FileExtension);
        }
    }

    public class CloneDroneOverhaulSettingsData : DataBase
    {
        public CloneDroneOverhaulSettingsData.Data SettingsData;


        [Serializable]
        public class Data
        {
            [NonSerialized]
            public CloneDroneOverhaulSettingsData DataBase;

            public List<SavedSettingEntry> SavedSettings;

            public void UpdateValue(string id, object value, bool onlyAdd)
            {
                SavedSettingEntry entry = GetSettingSave(id, value, onlyAdd);
                if (onlyAdd)
                {
                    return;
                }
                entry.Value = value;
                BaseStaticReferences.ModuleManager.OnSettingRefreshed(id, value, false);
            }

            public SavedSettingEntry GetSettingSave(string id, object newSettingValue, bool isNewSetting)
            {
                if (SavedSettings == null)
                {
                    SavedSettings = new List<SavedSettingEntry>();
                }
                foreach (SavedSettingEntry entry in SavedSettings)
                {
                    if (entry.ID == id)
                    {
                        return entry;
                    }
                }
                SavedSettingEntry entryNew = new SavedSettingEntry();
                entryNew.ID = id;
                if (isNewSetting)
                {
                    entryNew.Value = newSettingValue;
                }
                SavedSettings.Add(entryNew);
                return entryNew;
            }

            [Serializable]
            public class SavedSettingEntry
            {
                public string ID;
                public object Value;
            }
        }

        protected override void TryLoadData()
        {
            SettingsData = base.DataManagerReference.LoadData<CloneDroneOverhaulSettingsData.Data>(ModDataManager.Data_Folder + "SettingsData" + ModDataManager.FileExtension);
            SettingsData.DataBase = this;
        }

        protected override bool CheckFolders()
        {
            if (!File.Exists(ModDataManager.Data_Folder + "SettingsData" + ModDataManager.FileExtension))
            {
                CloneDroneOverhaulSettingsData.Data data = new CloneDroneOverhaulSettingsData.Data();
                data.SavedSettings = new List<Data.SavedSettingEntry>();
                SettingsData = data;
                SettingsData.DataBase = this;
                TrySaveData();
            }
            return true;
        }

        public override void TrySaveData()
        {
            base.DataManagerReference.SaveData<CloneDroneOverhaulSettingsData.Data>(SettingsData, ModDataManager.Data_Folder + "SettingsData" + ModDataManager.FileExtension);
        }

        public void SaveSetting(string id, object value, bool onlyAdd)
        {
            /*
            if(SettingsData == null)
            {
                CheckFolders();
            }*/
            SettingsData.UpdateValue(id, value, onlyAdd);
            if (onlyAdd)
            {
                return;
            }
            TrySaveData();
        }

        public object GetSettingValue(string id)
        {
            foreach (Data.SavedSettingEntry entry in SettingsData.SavedSettings)
            {
                if (entry.ID == id)
                {
                    return entry.Value;
                }
            }
            return null;
        }

        public T GetSettingValue<T>(string id)
        {
            return (T)GetSettingValue(id);
        }
    }
}
