using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using CloneDroneOverhaul.V3.Utilities;

namespace CloneDroneOverhaul.Modules
{
    public class ModDataManager : ModuleBase
    {
        public static string Mod_DataFolder => Application.persistentDataPath + "/CloneDroneOverhaul/";
        public static string Mod_Folder => OverhaulMain.Instance.ModInfo.FolderPath;
        public static string Mod_TempFolder => Mod_DataFolder + "/Temp/";
        public static string Addons_Folder => Mod_DataFolder + "Addons/";
        public static string Data_Folder => Mod_DataFolder + "SavedData/";
        public static string DublicatedLevelsFolder => Mod_DataFolder + "CopiedLevels/";
        public static string AddonsCompliedDlls => Mod_DataFolder + "CompiledDlls/";
        public static string FileExtension => ".cdosave";
        public bool CanOthersAddData()
        {
            return this != null && _hasInitialized;
        }
        private bool _hasInitialized;

        public override void Start()
        {
            checkFolders();
            _hasInitialized = true;
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

            V3.Base.ModDataController.CreateFoldersIfRequired(folders);
        }

        public T LoadData<T>(string path) where T : class
        {
            T result = null;
            result = V3.Utilities.OverhaulUtilities.ByteSaver.FromByteArray<T>(File.ReadAllBytes(path));
            return result;
        }
        public void SaveData<T>(T obj, string path) where T : class
        {
            byte[] array = null;
            array = V3.Utilities.OverhaulUtilities.ByteSaver.ObjectToByteArray(obj);
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
                    OverhaulMain.Timer.CompleteNextFrame(obj.LoadData);
                }
                else
                {
                    obj.LoadData();
                }
            }
            return obj;
        }
    }

    public class DataBase
    {
        protected internal ModDataManager DataManagerReference;

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
                V3.Base.V3_MainModController.SendSettingWasRefreshed(id, value);
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
                SavedSettingEntry entryNew = new SavedSettingEntry
                {
                    ID = id
                };
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
                CloneDroneOverhaulSettingsData.Data data = new CloneDroneOverhaulSettingsData.Data
                {
                    SavedSettings = new List<Data.SavedSettingEntry>()
                };
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

        public object GetSettingValue(in string id)
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

        public T GetSettingValue<T>(in string id)
        {
            return (T)GetSettingValue(id);
        }
    }
}
