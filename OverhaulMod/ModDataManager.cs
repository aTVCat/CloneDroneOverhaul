using OverhaulMod.Utils;
using System.IO;

namespace OverhaulMod
{
    public class ModDataManager : Singleton<ModDataManager>
    {
        public static void WriteFile(string name, string content, bool useSavesFolder)
        {
            ModFileUtils.WriteText(content, Path.Combine(getDirectory(useSavesFolder), name));
        }

        public static void SerializeToFile(string name, object obj, bool useSavesFolder)
        {
            ModJsonUtils.WriteStream(Path.Combine(getDirectory(useSavesFolder), name), obj);
        }

        public static string ReadFile(string name, bool useSavesFolder)
        {
            return ModFileUtils.ReadText(Path.Combine(getDirectory(useSavesFolder), name));
        }

        public static T DeserializeFile<T>(string name, bool useSavesFolder)
        {
            return ModJsonUtils.DeserializeStream<T>(Path.Combine(getDirectory(useSavesFolder), name));
        }

        public static bool FileExists(string name, bool useSavesFolder)
        {
            return File.Exists(Path.Combine(getDirectory(useSavesFolder), name));
        }

        private static string getDirectory(bool saves) => saves ? ModDirectories.SavesFolder : ModDirectories.ModUserDataFolder;
    }
}