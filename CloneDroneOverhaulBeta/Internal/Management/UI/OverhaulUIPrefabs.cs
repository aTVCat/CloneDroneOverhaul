using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.HUD
{
    public class OverhaulUIPrefabs : OverhaulBehaviour
    {
        private OverhaulUIManager m_UIManager;

        private Dictionary<string, GameObject> m_CachedPrefabs;

        public override void Awake()
        {
            m_CachedPrefabs = new Dictionary<string, GameObject>();
            m_UIManager = OverhaulUIManager.reference;
        }

        protected override void OnDisposed()
        {
            if(!m_CachedPrefabs.IsNullOrEmpty())
                m_CachedPrefabs.Clear();

            m_CachedPrefabs = null;
            m_UIManager = null;
        }

        public GameObject InstantiatePrefab(string key, Transform parent, bool visible, string assetBundle = OverhaulAssetsController.ModAssetBundle_Part1)
        {
            GameObject prefab = null;
            m_CachedPrefabs.TryGetValue(key, out prefab);
            if(prefab == null)
            {
                prefab = OverhaulAssetsController.GetAsset(key, assetBundle, false);
                m_CachedPrefabs.Add(key, prefab);
            }
            else if (!prefab)
            {
                prefab = OverhaulAssetsController.GetAsset(key, assetBundle, false);
                m_CachedPrefabs[key] = prefab;
            }

            GameObject result = Instantiate(prefab, parent);
            result.SetActive(visible);
            return result;
        }
    }
}
