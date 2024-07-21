using OverhaulMod.Utils;
using System.IO;

namespace OverhaulMod
{
    public class ModDataManager : Singleton<ModDataManager>
    {
        private string m_userDataFolder;
        /// <summary>
        /// A folder where all game data is stored
        /// </summary>
        public string userDataFolder
        {
            get
            {
                if (m_userDataFolder == null)
                {
                    m_userDataFolder = ModCore.modDataFolder;
                }
                return m_userDataFolder;
            }
        }

        /// <summary>
        /// "saves" folder located under mod folder
        /// </summary>
        public string savesFolder
        {
            get
            {
                return ModCore.savesFolder;
            }
        }

        public void WriteFile(string name, string content, bool useSavesFolder)
        {
            ModIOUtils.WriteText(content, (useSavesFolder ? savesFolder : userDataFolder) + name);
        }

        public void SerializeToFile(string name, object obj, bool useSavesFolder)
        {
            ModJsonUtils.WriteStream((useSavesFolder ? savesFolder : userDataFolder) + name, obj);
        }

        public string ReadFile(string name, bool useSavesFolder)
        {
            return ModIOUtils.ReadText((useSavesFolder ? savesFolder : userDataFolder) + name);
        }

        public T DeserializeFile<T>(string name, bool useSavesFolder)
        {
            return ModJsonUtils.DeserializeStream<T>((useSavesFolder ? savesFolder : userDataFolder) + name);
        }

        public bool FileExists(string name, bool useSavesFolder)
        {
            return File.Exists((useSavesFolder ? savesFolder : userDataFolder) + name);
        }
    }
}
