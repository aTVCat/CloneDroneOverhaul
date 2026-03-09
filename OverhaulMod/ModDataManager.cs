using OverhaulMod.Utils;
using System.IO;

namespace OverhaulMod
{
    public class ModDataManager : Singleton<ModDataManager>
    {
        /// <summary>
        /// A Folder where all game data is stored
        /// </summary>
        public static string userDataFolder
        {
            get
            {
                return ModCore.ModUserDataFolder;
            }
        }

        /// <summary>
        /// "saves" Folder located under mod Folder
        /// </summary>
        public static string savesFolder
        {
            get
            {
                return ModCore.SavesFolder;
            }
        }

        public void WriteFile(string name, string content, bool useSavesFolder)
        {
            ModFileUtils.WriteText(content, Path.Combine(useSavesFolder ? savesFolder : userDataFolder, name));
        }

        public void SerializeToFile(string name, object obj, bool useSavesFolder)
        {
            ModJsonUtils.WriteStream(Path.Combine(useSavesFolder ? savesFolder : userDataFolder, name), obj);
        }

        public string ReadFile(string name, bool useSavesFolder)
        {
            return ModFileUtils.ReadText(Path.Combine(useSavesFolder ? savesFolder : userDataFolder, name));
        }

        public T DeserializeFile<T>(string name, bool useSavesFolder)
        {
            return ModJsonUtils.DeserializeStream<T>(Path.Combine(useSavesFolder ? savesFolder : userDataFolder, name));
        }

        public bool FileExists(string name, bool useSavesFolder)
        {
            return File.Exists(Path.Combine(useSavesFolder ? savesFolder : userDataFolder, name));
        }
    }
}
