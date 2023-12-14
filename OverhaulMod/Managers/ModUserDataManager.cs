using OverhaulMod.Utils;
using System.IO;

namespace OverhaulMod
{
    public class ModUserDataManager : Singleton<ModUserDataManager>
    {
        private string m_folder;
        public string folder
        {
            get
            {
                if (m_folder == null)
                {
                    m_folder = ModCache.dataRepository.GetRootDataPath(false) + "/";
                }
                return m_folder;
            }
        }

        public string savesFolder
        {
            get
            {
                return ModCore.savesFolder;
            }
        }

        public void WriteFile(string name, string content, bool writeToSavesFolder)
        {
            ModIOUtils.WriteText(content, (writeToSavesFolder ? savesFolder : folder) + name);
        }

        public string ReadFile(string name, bool readFromSavesFolder)
        {
            return ModIOUtils.TryReadText((readFromSavesFolder ? savesFolder : folder) + name, out _);
        }

        public bool HasFile(string name, bool checkInSavesFolder)
        {
            return File.Exists((checkInSavesFolder ? savesFolder : folder) + name);
        }
    }
}
