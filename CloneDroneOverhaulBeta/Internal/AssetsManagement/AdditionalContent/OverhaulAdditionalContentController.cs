using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.NetworkAssets.AdditionalContent
{
    public class OverhaulAdditionalContentController : OverhaulController
    {
        public const string AdditionalContentDirectoryName = "AdditionalContent";

        private static string m_CachedAdditionalContentPath;
        public static string AdditionalContentDirectory => m_CachedAdditionalContentPath;

        public static readonly string[] ExcludedDirectories = new string[]
        {
            "Archives"
        };

        public override void Initialize()
        {
            if (string.IsNullOrEmpty(m_CachedAdditionalContentPath))
            {
                m_CachedAdditionalContentPath = OverhaulMod.Core.ModDirectory + AdditionalContentDirectoryName + "/";
            }
        }

        #region Content detection

        private string[] getContentFolders()
        {
            string path = AdditionalContentDirectory;
            bool folderExists = Directory.Exists(path);
            if (!folderExists)
            {
                return null;
            }

            return Directory.GetDirectories(path);
        }

        private List<string> getRawContentInfos(string[] directories)
        {
            if(directories.IsNullOrEmpty() || directories.Length < 2)
            {
                return null;
            }

            List<string> rawInfoList = new List<string>();
            foreach(string directory in directories)
            {
                // Skip if excluded
                foreach(string excluded in ExcludedDirectories)
                {
                    if (directory.EndsWith(excluded))
                    {
                        continue;
                    }
                }

                string infoFilePath = directory + "/ContentInfo.json";
                bool infoFileExists = File.Exists(infoFilePath);
                if (!infoFileExists)
                {
                    continue;
                }

                string content = OverhaulCore.ReadTextFile(infoFilePath);
                rawInfoList.Add(content);
            }
            return rawInfoList;
        }

        private List<OverhaulAdditionalContentPackInfo> getPackInfos(List<string> rawContent)
        {
            List<OverhaulAdditionalContentPackInfo> result = new List<OverhaulAdditionalContentPackInfo>();
            int index = 0;
            do
            {
                try
                {
                    OverhaulAdditionalContentPackInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<OverhaulAdditionalContentPackInfo>(rawContent[index]);
                    info.PreviousFolderName = info.GetPackFolderName();
                    result.Add(info);
                }
                catch
                {
                }
                index++;
            } while (index < rawContent.Count);

            return result;
        }

        public List<OverhaulAdditionalContentPackInfo> GetAllInstalledContent()
        {
            return getPackInfos(getRawContentInfos(getContentFolders()));
        }

        #endregion

        public override string[] Commands()
        {
            throw new NotImplementedException();
        }
        public override string OnCommandRan(string[] command)
        {
            throw new NotImplementedException();
        }
    }
}
