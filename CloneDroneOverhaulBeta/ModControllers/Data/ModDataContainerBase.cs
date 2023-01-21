using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

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

        public void SaveData<T>() where T : ModDataContainerBase
        {
            if (string.IsNullOrEmpty(FileName))
            {
                return;
            }

            ModDataController.SaveData((T)this, FileName);
        }

        /// <summary>
        /// Add missing values if you have need
        /// </summary>
        protected virtual void RepairMissingFields()
        {

        }

        public static T GetData<T>(in string fileName) where T : ModDataContainerBase
        {
            if (_cachedDatas.ContainsKey(fileName))
            {
                return (T)_cachedDatas[fileName];
            }

            T data = ModDataController.GetData<T>(fileName);
            data.RepairMissingFields();
            if (!_cachedDatas.ContainsKey(fileName))
            {
                _cachedDatas.Add(fileName, data);
            }

            return data;
        }
    }
}
