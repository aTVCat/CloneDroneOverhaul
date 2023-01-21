using System;

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
        public static void SaveData<T>(T data, in string fileName) where T : ModDataContainerBase
        {
            DataRepository.Instance.Save(data, "Overhaul/" + fileName, false, false);
        }

        /// <summary>
        /// Load data from disk
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T GetData<T>(in string fileName) where T : ModDataContainerBase
        {
            DataRepository.Instance.TryLoad("Overhaul/" + fileName, out T data, false);
            if(data == null)
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
