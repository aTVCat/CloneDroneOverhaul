using System;
using System.Collections.Generic;
using System.Reflection;

namespace CDOverhaul
{
    public abstract class OverhaulDataBase : OverhaulDisposable
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

        protected virtual void OnPreSave() { }
        protected virtual void OnPostSave() { }

        protected override void OnDisposed() => OverhaulDisposable.AssignNullToAllVars(this);
        public virtual void Unload() => Dispose();

        #region Static

        [NonSerialized]
        private static readonly Dictionary<string, OverhaulDataBase> m_CachedDatas = new Dictionary<string, OverhaulDataBase>();

        public void SaveData(bool useModFolder = false, string modFolderName = null)
        {
            if (string.IsNullOrEmpty(FileName))
                return;

            OnPreSave();
            SavedInVersion = OverhaulVersion.ModVersion;
            OverhaulDataController.SaveData(this, FileName, useModFolder, modFolderName);
            OnPostSave();
        }

        public static T GetData<T>(string fileName, bool useModFolder = false, string modFolderName = null, bool ignoreLoaded = false) where T : OverhaulDataBase
        {
            bool containsKey = m_CachedDatas.ContainsKey(fileName);
            if (containsKey && !ignoreLoaded)
                return (T)m_CachedDatas[fileName];

            T data = OverhaulDataController.GetData<T>(fileName, useModFolder, modFolderName);
            data.RepairFields();
            if (!containsKey)
                m_CachedDatas.Add(fileName, data);
            else
                m_CachedDatas[fileName] = data;

            return data;
        }

        #endregion
    }
}
