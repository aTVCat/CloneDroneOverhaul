using CloneDroneOverhaul.PooledPrefabs;
using CloneDroneOverhaul.Utilities;
using ModLibrary;
using UnityEngine;
using UnityEngine.Rendering;
using UnityStandardAssets.ImageEffects;
using System.Collections.Generic;
using System.IO;
using System;

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
            if (!Directory.Exists(Mod_DataFolder))
            {
                Directory.CreateDirectory(Mod_DataFolder);
                OverhaulMain.Timer.AddActionToCompleteNextFrame(showMessageAboutFolderCreation, new object[] { Mod_DataFolder });
            }
            if (!Directory.Exists(Addons_Folder))
            {
                Directory.CreateDirectory(Addons_Folder);
                OverhaulMain.Timer.AddActionToCompleteNextFrame(showMessageAboutFolderCreation, new object[] { Addons_Folder });
            }
            if (!Directory.Exists(Data_Folder))
            {
                Directory.CreateDirectory(Data_Folder);
                OverhaulMain.Timer.AddActionToCompleteNextFrame(showMessageAboutFolderCreation, new object[] { Data_Folder });
            }
            if (!Directory.Exists(Mod_TempFolder))
            {
                Directory.CreateDirectory(Mod_TempFolder);
                OverhaulMain.Timer.AddActionToCompleteNextFrame(showMessageAboutFolderCreation, new object[] { Mod_TempFolder });
            }
            if (!Directory.Exists(AddonsCompliedDlls))
            {
                Directory.CreateDirectory(AddonsCompliedDlls);
                OverhaulMain.Timer.AddActionToCompleteNextFrame(showMessageAboutFolderCreation, new object[] { AddonsCompliedDlls });
            }
        }
        private void showMessageAboutFolderCreation(object[] args)
        {
            CloneDroneOverhaul.UI.Notifications.Notification notif = new UI.Notifications.Notification();
            notif.SetUp("New folder created", "Created: " + (string)args[0], 20, Vector2.zero, Color.clear, new UI.Notifications.Notification.NotificationButton[] { new UI.Notifications.Notification.NotificationButton { Action = new UnityEngine.Events.UnityAction(notif.HideThis), Text = "OK" } });
        }

        public T LoadData<T>(string path) where T : class
        {
            T result = null;
            try
            {
                result = ByteSaver.FromByteArray<T>(File.ReadAllBytes(path));
            }
            catch(Exception ex)
            {
                ModuleManagement.ShowError_Type2("Error occured, while loading data from path: " + path, "Details:" + ex.Message);
            }
            return result;
        }
        public void SaveData<T>(T obj, string path) where T : class
        {
            try
            {
                byte[] array = null;
                array = ByteSaver.ObjectToByteArray(obj);
                File.WriteAllBytes(path, array);
            }
            catch (Exception ex)
            {
                ModuleManagement.ShowError_Type2("Error occured, while saving data in: " + path, "Details:" + ex.Message);
            }
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
    }

    public class DataBase
    {
        internal protected ModDataManager DataManagerReference;

        public void LoadData()
        {
            try
            {
                if (!checkFolders())
                {
                    return;
                }
                TryLoadData();
            }
            catch(System.Exception ex)
            {
                ModuleManagement.ShowError_Type2(this.GetType().ToString() + " Data error", "Details (TryLoadData method): " + ex.Message);
            }
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

    public class CloneDroneOverhaulSettingsData : DataBase
    {
        public CloneDroneOverhaulSettingsData.Data SettingsData;

        [Serializable]
        public class Data
        {
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
        }

        protected override bool CheckFolders()
        {
            if(!File.Exists(ModDataManager.Data_Folder + "SettingsData" + ModDataManager.FileExtension))
            {
                CloneDroneOverhaulSettingsData.Data data = new CloneDroneOverhaulSettingsData.Data();
                data.SavedSettings = new List<Data.SavedSettingEntry>();
                base.DataManagerReference.SaveData<CloneDroneOverhaulSettingsData.Data>(data, ModDataManager.Data_Folder + "SettingsData" + ModDataManager.FileExtension);
            }
            return true;
        }

        public override void TrySaveData()
        {
            base.DataManagerReference.SaveData<CloneDroneOverhaulSettingsData.Data>(SettingsData, ModDataManager.Data_Folder + "SettingsData" + ModDataManager.FileExtension);
        }

        public void SaveSetting(string id, object value, bool onlyAdd)
        {            
            SettingsData.UpdateValue(id, value, onlyAdd);
            if (onlyAdd)
            {
                return;
            }
            TrySaveData();
        }

        public object GetSettingValue(string id)
        {
            foreach(Data.SavedSettingEntry entry in SettingsData.SavedSettings)
            {
                if(entry.ID == id)
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
