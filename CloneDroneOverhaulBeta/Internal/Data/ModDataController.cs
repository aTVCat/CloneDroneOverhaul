using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace CDOverhaul
{
    /// <summary>
    /// A simplified interaction with <see cref="DataRepository"/>
    /// </summary>
    internal static class OverhaulDataController
    {
        public const string OverhaulDataDirectoryName = "Overhaul/";

        /// <summary>
        /// Save data as .json file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataContainer"></param>
        /// <param name="fileName"></param>
        public static void SaveData(object dataContainer, in string fileName, in bool useModFolder, in string modFolder)
        {
            if (!(dataContainer is OverhaulDataBase))
            {
                return;
            }

            if (useModFolder)
            {
                string str = JsonConvert.SerializeObject(dataContainer, DataRepository.Instance.GetSettings());
                File.WriteAllText(OverhaulMod.Core.ModDirectory + "Assets/" + modFolder + "/" + fileName + ".json", str);
                return;
            }

            if (!Directory.Exists(Application.persistentDataPath + OverhaulDataDirectoryName))
            {
                _ = Directory.CreateDirectory(Application.persistentDataPath + OverhaulDataDirectoryName);
            }
            DataRepository.Instance.Save(dataContainer, OverhaulDataDirectoryName + fileName + ".json", false, false);
        }

        /// <summary>
        /// Load data from disk
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T GetData<T>(in string fileName, in bool useModFolder, in string modFolder) where T : OverhaulDataBase
        {
            if (useModFolder)
            {
                string path = OverhaulMod.Core.ModDirectory + "Assets/" + modFolder + "/" + fileName + ".json";

                T result;
                if (File.Exists(path))
                {
                    result = JsonConvert.DeserializeObject<T>(File.ReadAllText(path), DataRepository.Instance.GetSettings());
                }
                else
                {
                    result = Activator.CreateInstance<T>();
                }

                SetUpContainer(result, fileName, path);
                return result;
            }

            _ = DataRepository.Instance.TryLoad(OverhaulDataDirectoryName + fileName + ".json", out T data, false);
            if(data == null) data = Activator.CreateInstance<T>();
            SetUpContainer(data, fileName, DataRepository.Instance.GetFullPath(fileName, false));
            return data;
        }

        internal static void SetUpContainer(in OverhaulDataBase container, in string fileName, in string savePath)
        {
            if(container == null)
            {
                return;
            }

            container.IsLoadedFromFile = true;
            container.FileName = fileName;
            container.SavePath = savePath;
            container.RepairFields();
        }
    }
}
