using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationCacheManager : Singleton<PersonalizationCacheManager>
    {
        private Dictionary<string, byte[]> _cachedFiles;

        private bool _isCaching;

        public override void Awake()
        {
            base.Awake();
            _cachedFiles = new Dictionary<string, byte[]>();
        }

        public bool TryGet(string path, out byte[] array)
        {
            Dictionary<string, byte[]> d = _cachedFiles;
            if (!d.TryGetValue(path, out array))
                return false;

            return true;
        }

        public void Remove(string path)
        {
            if (_cachedFiles.ContainsKey(path))
            {
                _ = _cachedFiles[path];
                _ = _cachedFiles.Remove(path);
            }
        }

        public void CacheFiles(List<PersonalizationItemInfo> personalizationItemInfos)
        {
            if (_isCaching)
                return;

            _isCaching = true;
            _ = base.StartCoroutine(cacheFilesCoroutine(personalizationItemInfos));
        }

        private IEnumerator cacheFilesCoroutine(List<PersonalizationItemInfo> personalizationItemInfos)
        {
            Dictionary<string, byte[]> d = _cachedFiles;
            d.Clear();

            if (personalizationItemInfos == null || personalizationItemInfos.Count == 0) yield break;

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
                                byte[] array = ModFileUtils.ReadBytes(path);
                                d.Add(path.Replace("/", "\\"), array);
                            }
                            catch (Exception)
                            {

                            }
                        }

                        j++;
                    } while (j < importedFiles.Count);
                }

                i++;
            } while (i < personalizationItemInfos.Count);

            _isCaching = false;
            yield break;
        }
    }
}
