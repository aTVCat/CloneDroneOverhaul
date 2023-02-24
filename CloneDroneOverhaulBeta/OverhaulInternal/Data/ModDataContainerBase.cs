using System;
using System.Collections.Generic;
using System.Threading;

namespace CDOverhaul
{
    public abstract class ModDataContainerBase
    {
        /// <summary>
        /// Define if this container base was loaded from file
        /// </summary>
        [NonSerialized]
        public bool IsLoadedFromFile;

        [NonSerialized]
        public string SavePath;

        [NonSerialized]
        public string FileName;

        public abstract void RepairFields();

        protected virtual void OnPreSave()
        {

        }

        protected virtual void OnPostSave()
        {

        }

        #region Static

        [NonSerialized]
        private static readonly Dictionary<string, ModDataContainerBase> m_CachedDatas = new Dictionary<string, ModDataContainerBase>();

        public void SaveData(bool useModFolder = false, string modFolderName = null)
        {
            if (string.IsNullOrEmpty(FileName)) return;

            ThreadStart start = new ThreadStart(delegate
            {
                OnPreSave();
                OverhaulDataController.SaveData(this, FileName, useModFolder, modFolderName);
                OnPostSave();
            });
            Thread newThread = new Thread(start);
        }

        public static T GetData<T>(string fileName, bool useModFolder = false, string modFolderName = null) where T : ModDataContainerBase
        {
            bool containsKey = m_CachedDatas.ContainsKey(fileName);
            if (containsKey)
            {
                return (T)m_CachedDatas[fileName];
            }

            T data = OverhaulDataController.GetData<T>(fileName, useModFolder, modFolderName);
            data.RepairFields();
            if (!containsKey)
            {
                m_CachedDatas.Add(fileName, data);
            }

            return data;
        }

        #endregion
    }
}
