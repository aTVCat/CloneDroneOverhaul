using System;
using System.Collections.Generic;

namespace CDOverhaul
{
    public class ModDataContainerBase
    {
        private static Dictionary<string, ModDataContainerBase> _cachedDatas = new Dictionary<string, ModDataContainerBase>();

        [NonSerialized]
        public bool IsLoaded;

        [NonSerialized]
        public string SavePath;

        [NonSerialized]
        public string FileName;

        public void SaveData<T>(in bool useModFolder = false, in string modFolderName = null) where T : ModDataContainerBase
        {
            if (string.IsNullOrEmpty(FileName))
            {
                return;
            }

            OnPrepareToSave();
            ModDataController.SaveData((T)this, FileName, useModFolder, modFolderName);
        }

        /// <summary>
        /// Add missing values if you have need
        /// </summary>
        public virtual void RepairMissingFields()
        {

        }

        protected virtual void OnPrepareToSave()
        {

        }

        public static T GetData<T>(in string fileName, in bool useModFolder = false, in string modFolderName = null) where T : ModDataContainerBase
        {
            if (_cachedDatas.ContainsKey(fileName))
            {
                return (T)_cachedDatas[fileName];
            }

            T data = ModDataController.GetData<T>(fileName, useModFolder, modFolderName);
            data.RepairMissingFields();
            if (!_cachedDatas.ContainsKey(fileName))
            {
                _cachedDatas.Add(fileName, data);
            }

            return data;
        }
    }
}
