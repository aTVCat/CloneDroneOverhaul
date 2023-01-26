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
        public static void SaveData<T>(T data, in string fileName, in bool useModFolder, in string modFolder) where T : ModDataContainerBase
        {
            if (useModFolder)
            {
                string str = JsonConvert.SerializeObject(data, DataRepository.Instance.GetSettings());
                File.WriteAllText(OverhaulBase.Core.ModFolder + "Assets/" + modFolder + fileName, str);
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
                T result = null;
                if (File.Exists(OverhaulBase.Core.ModFolder + "Assets/" + modFolder + "/" + fileName))
                {
                    result = (T)JsonConvert.DeserializeObject(File.ReadAllText(OverhaulBase.Core.ModFolder + "Assets/" + modFolder + "/" + fileName), DataRepository.Instance.GetSettings());
                }
                else
                {
                    result = Activator.CreateInstance<T>();
                    result.SaveData<T>(true, modFolder);
                }
                result.IsLoaded = true;
                result.FileName = fileName;
                result.SavePath = OverhaulBase.Core.ModFolder + "Assets/" + modFolder + fileName;
                return result;
            }

            DataRepository.Instance.TryLoad("Overhaul/" + fileName, out T data, false);
            if (data == null)
            {
                data = Activator.CreateInstance<T>();
            }
            data.IsLoaded = true;
            data.FileName = fileName;
            data.SavePath = DataRepository.Instance.GetFullPath(fileName, false);
            return data;
        }
    }
}
