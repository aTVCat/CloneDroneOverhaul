using System.IO;

namespace OverhaulMod.Engine
{
    public class ModDownloadCacheManager : Singleton<ModDownloadCacheManager>
    {
        public const string CACHED_FILE_PREFIX = "overhaulDownload_";

        private string m_tempFolderPath;

        public override void Awake()
        {
            base.Awake();
            m_tempFolderPath = Path.GetTempPath();
        }

        public string GetFileNameOfDownload(string url)
        {
            return $"{CACHED_FILE_PREFIX}{url.GetHashCode()}";
        }

        public string GetPathOfDownload(string url)
        {
            return Path.Combine(m_tempFolderPath, GetFileNameOfDownload(url));
        }

        public bool HasCachedDownload(string url)
        {
            return File.Exists(GetPathOfDownload(url));
        }
    }
}