using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationCacheManager : Singleton<PersonalizationCacheManager>
    {
        private Dictionary<string, byte[]> m_cachedFiles;

        private bool m_isCaching;

        public override void Awake()
        {
            base.Awake();
            m_cachedFiles = new Dictionary<string, byte[]>();
        }

        public bool TryGet(string path, out byte[] array)
        {
            Dictionary<string, byte[]> d = m_cachedFiles;
            if (!d.TryGetValue(path, out array))
                return false;

            return true;
        }

        public void Remove(string path)
        {
            if (m_cachedFiles.ContainsKey(path))
            {
                byte[] array = m_cachedFiles[path];
                _ = m_cachedFiles.Remove(path);
            }
        }

        public void CacheFiles(List<PersonalizationItemInfo> personalizationItemInfos)
        {
            if (m_isCaching)
                return;

            m_isCaching = true;
            _ = base.StartCoroutine(cacheFilesCoroutine(personalizationItemInfos));
        }

        private IEnumerator cacheFilesCoroutine(List<PersonalizationItemInfo> personalizationItemInfos)
        {
            Dictionary<string, byte[]> d = m_cachedFiles;
            d.Clear();

            int i = 0;
            do
            {
                yield return null;

                PersonalizationItemInfo item = personalizationItemInfos[i];
                List<string> importedFiles = item.ImportedFiles;

                if (!importedFiles.IsNullOrEmpty())
                {
                    int j = 0;
                    do
                    {
                        string path = PersonalizationItemInfo.GetImportedFileFullPath(item, importedFiles[j]);
                        if (File.Exists(path))
                        {
                            try
                            {
                                byte[] array = ModIOUtils.ReadBytes(path);
                                d.Add(path.Replace("/", "\\"), array);
                            }
                            catch
                            {

                            }
                        }

                        j++;
                    } while (j < importedFiles.Count);
                }

                i++;
            } while (i < personalizationItemInfos.Count);

            m_isCaching = false;
            yield break;
        }
    }
}
