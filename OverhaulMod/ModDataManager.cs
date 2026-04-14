using OverhaulMod.Utils;
using System.IO;

namespace OverhaulMod
{
    public class ModDataManager : Singleton<ModDataManager>
    {
        /// <summary>
        /// A folder where all game data is stored
        /// </summary>
        public static string UserDataFolder
        {
            get
            {
                return ModCore.ModUserDataFolder;
            }
        }

        /// <summary>
        /// "saves" Folder located under mod Folder
        /// </summary>
        public static string SavesFolder
        {
            get
            {
                return ModCore.SavesFolder;
            }
        }

        public static void WriteFile(string name, string content, bool useSavesFolder)
        {
            ModFileUtils.WriteText(content, Path.Combine(useSavesFolder ? SavesFolder : UserDataFolder, name));
        }

        public static void SerializeToFile(string name, object obj, bool useSavesFolder)
        {
            ModJsonUtils.WriteStream(Path.Combine(useSavesFolder ? SavesFolder : UserDataFolder, name), obj);
        }

        public static string ReadFile(string name, bool useSavesFolder)
        {
            return ModFileUtils.ReadText(Path.Combine(useSavesFolder ? SavesFolder : UserDataFolder, name));
        }

        public static T DeserializeFile<T>(string name, bool useSavesFolder)
        {
            return ModJsonUtils.DeserializeStream<T>(Path.Combine(useSavesFolder ? SavesFolder : UserDataFolder, name));
        }

        public static bool FileExists(string name, bool useSavesFolder)
        {
            return File.Exists(Path.Combine(useSavesFolder ? SavesFolder : UserDataFolder, name));
        }
    }
}
