using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.HUD
{
    public class OverhaulUIPrefabs : OverhaulBehaviour
    {
        private Dictionary<string, GameObject> m_CachedPrefabs;

        public override void Awake()
        {
            m_CachedPrefabs = new Dictionary<string, GameObject>();
        }

        protected override void OnDisposed()
        {
            if (!m_CachedPrefabs.IsNullOrEmpty())
                m_CachedPrefabs.Clear();

            m_CachedPrefabs = null;
        }

        public GameObject InstantiatePrefab(string key, Transform parent, bool visible, string assetBundle = OverhaulAssetLoader.ModAssetBundle_Part1)
        {
            _ = m_CachedPrefabs.TryGetValue(key, out GameObject prefab);
            if (prefab == null)
            {
                prefab = OverhaulAssetLoader.GetAsset(key, assetBundle, false);
                m_CachedPrefabs.Add(key, prefab);
            }
            else if (!prefab)
            {
                prefab = OverhaulAssetLoader.GetAsset(key, assetBundle, false);
                m_CachedPrefabs[key] = prefab;
            }

            GameObject result = Instantiate(prefab, parent);
            result.SetActive(visible);
            return result;
        }
    }
}
