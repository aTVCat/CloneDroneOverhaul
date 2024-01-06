using LevelEditorPatch;
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
                    m_userDataFolder = ModCache.dataRepository.GetRootDataPath(false) + "/";
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
            ModIOUtils.WriteText(JsonUtils.Serialize(obj), (useSavesFolder ? savesFolder : userDataFolder) + name);
        }

        public string ReadFile(string name, bool useSavesFolder)
        {
            return ModIOUtils.ReadText((useSavesFolder ? savesFolder : userDataFolder) + name);
        }

        public T DeserializeFromFile<T>(string name, bool useSavesFolder)
        {
            return ModJsonUtils.DeserializeStream<T>((useSavesFolder ? savesFolder : userDataFolder) + name);
        }

        public bool FileExists(string name, bool useSavesFolder)
        {
            return File.Exists((useSavesFolder ? savesFolder : userDataFolder) + name);
        }
    }
}
