using ModLibrary;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CDOverhaul
{
    public abstract class OverhaulDataBase
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

        public Version SavedInVersion;

        public abstract void RepairFields();

        protected virtual void OnPreSave()
        {

        }

        protected virtual void OnPostSave()
        {

        }

        #region Static

        [NonSerialized]
        private static readonly Dictionary<string, OverhaulDataBase> m_CachedDatas = new Dictionary<string, OverhaulDataBase>();

        public void SaveData(bool useModFolder = false, string modFolderName = null)
        {
            if (string.IsNullOrEmpty(FileName)) return;
            OnPreSave();
            SavedInVersion = OverhaulVersion.ModVersion;
            OverhaulDataController.SaveData(this, FileName, useModFolder, modFolderName);
            OnPostSave();
        }

        public static T GetData<T>(string fileName, bool useModFolder = false, string modFolderName = null) where T : OverhaulDataBase
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
