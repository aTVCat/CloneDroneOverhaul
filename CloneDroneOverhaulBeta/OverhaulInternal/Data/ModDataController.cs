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
            if (!(dataContainer is ModDataContainerBase))
            {
                return;
            }

            if (useModFolder)
            {
                string str = JsonConvert.SerializeObject(dataContainer, DataRepository.Instance.GetSettings());
                File.WriteAllText(OverhaulMod.Core.ModDirectory + "Assets/" + modFolder + fileName, str);
                return;
            }

            if (!Directory.Exists(Application.persistentDataPath + OverhaulDataDirectoryName))
            {
                Directory.CreateDirectory(Application.persistentDataPath + OverhaulDataDirectoryName);
            }
            DataRepository.Instance.Save(dataContainer, OverhaulDataDirectoryName + fileName, false, false);
        }

        /// <summary>
        /// Load data from disk
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T GetData<T>(in string fileName, in bool useModFolder, in string modFolder) where T : ModDataContainerBase
        {
            if (useModFolder)
            {
                string path = OverhaulMod.Core.ModDirectory + "Assets/" + modFolder + "/" + fileName + ".json";
                bool needsToSave = false;

                T result = Activator.CreateInstance<T>();
                if (File.Exists(path))
                {
                    result = JsonConvert.DeserializeObject<T>(File.ReadAllText(path), DataRepository.Instance.GetSettings());
                }
                else
                {
                    needsToSave = true;
                }

                SetUpContainer(result, fileName, path);
                if (needsToSave)
                {
                    result.SaveData(true, modFolder);
                }

                return result;
            }

            _ = DataRepository.Instance.TryLoad(OverhaulDataDirectoryName + fileName + ".json", out T data, false);
            SetUpContainer(data, fileName, DataRepository.Instance.GetFullPath(fileName, false));
            return data;
        }

        internal static void SetUpContainer(in ModDataContainerBase container, in string fileName, in string savePath)
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
