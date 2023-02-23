using System;
using System.Collections.Generic;
using System.Threading;

namespace CDOverhaul
{
    public class ModDataContainerBase
    {
        [NonSerialized]
        private static readonly Dictionary<string, ModDataContainerBase> _cachedDatas = new Dictionary<string, ModDataContainerBase>();

        /// <summary>
        /// Define if this container base was loaded from file
        /// </summary>
        [NonSerialized]
        public bool IsLoadedFromFile;

        [NonSerialized]
        public string SavePath;

        [NonSerialized]
        public string FileName;

        public virtual void RepairFields()
        {

        }

        protected virtual void OnPreSave()
        {

        }

        protected virtual void OnPostSave()
        {

        }

        public void SaveData(bool useModFolder = false, string modFolderName = null)
        {
            if (string.IsNullOrEmpty(FileName)) return;

            ThreadStart start = new ThreadStart(delegate
            {
                OnPreSave();
                ModDataController.SaveData(this, FileName, useModFolder, modFolderName);
                OnPostSave();
            });
            Thread newThread = new Thread(start);
        }

        public static T GetData<T>(string fileName, bool useModFolder = false, string modFolderName = null) where T : ModDataContainerBase
        {
            bool containsKey = _cachedDatas.ContainsKey(fileName);
            if (containsKey)
            {
                return (T)_cachedDatas[fileName];
            }

            T data = ModDataController.GetData<T>(fileName, useModFolder, modFolderName);
            data.RepairFields();
            if (!containsKey)
            {
                _cachedDatas.Add(fileName, data);
            }

            return data;
        }
    }
}
