using Newtonsoft.Json;
using System;
using System.IO;

namespace CDOverhaul
{
    /// <summary>
    /// A simplified interaction with <see cref="DataRepository"/>
    /// </summary>
    internal static class ModDataController
    {
        /// <summary>
        /// Save data as .json file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        public static void SaveData(object data, in string fileName, in bool useModFolder, in string modFolder)
        {
            if (!(data is ModDataContainerBase))
            {
                return;
            }

            if (useModFolder)
            {
                string str = JsonConvert.SerializeObject(data, DataRepository.Instance.GetSettings());
                File.WriteAllText(OverhaulMod.Core.ModFolder + "Assets/" + modFolder + fileName, str);
                return;
            }
            DataRepository.Instance.Save(data, "Overhaul/" + fileName, false, false);
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
                string path = OverhaulMod.Core.ModFolder + "Assets/" + modFolder + "/" + fileName + ".json";
                bool needsToSave = false;

                T result;
                if (File.Exists(path))
                {
                    result = JsonConvert.DeserializeObject<T>(File.ReadAllText(path), DataRepository.Instance.GetSettings());
                }
                else
                {
                    result = Activator.CreateInstance<T>();
                    needsToSave = true;
                }

                SetUpContainer(result, fileName, path);
                if (needsToSave)
                {
                    result.SaveData(true, modFolder);
                }

                return result;
            }

            _ = DataRepository.Instance.TryLoad("Overhaul/" + fileName + ".json", out T data, false);
            SetUpContainer(data, fileName, DataRepository.Instance.GetFullPath(fileName, false));
            return data;
        }

        internal static void SetUpContainer(in ModDataContainerBase container, in string fileName, in string savePath)
        {
            container.IsLoaded = true;
            container.FileName = fileName;
            container.SavePath = savePath;
            container.RepairMissingFields();
        }
    }
}
