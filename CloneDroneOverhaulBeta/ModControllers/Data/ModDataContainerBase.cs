using System;
using System.Collections.Generic;

namespace CDOverhaul
{
    public class ModDataContainerBase
    {
        [NonSerialized]
        private static readonly Dictionary<string, ModDataContainerBase> _cachedDatas = new Dictionary<string, ModDataContainerBase>();

        [NonSerialized]
        public bool IsLoaded;

        [NonSerialized]
        public string SavePath;

        [NonSerialized]
        public string FileName;

        public void SaveData(in bool useModFolder = false, in string modFolderName = null)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                return;
            }

            OnPrepareToSave();
            ModDataController.SaveData(this, FileName, useModFolder, modFolderName);
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

        /// <summary>
        /// Add missing values if you have need to
        /// </summary>
        public virtual void RepairMissingFields()
        {

        }

        protected virtual void OnPrepareToSave()
        {

        }
    }
}
