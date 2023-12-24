using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod
{
    public class ModResources : Singleton<ModResources>
    {
        internal const string ASSET_BUNDLES_FOLDER = "assets/assetBundles/";

        private Dictionary<string, Dictionary<string, UnityEngine.Object>> m_Objects;

        private Dictionary<string, AssetBundle> m_AssetBundles;

        public int cachedAsseBundleCount
        {
            get
            {
                return m_Objects.Count;
            }
        }

        public int cachedObjectCount
        {
            get
            {
                int count = 0;
                foreach (string key in m_Objects.Keys)
                {
                    count += m_Objects[key].Count;
                }
                return count;
            }
        }

        public override void Awake()
        {
            base.Awake();

            m_Objects = new Dictionary<string, Dictionary<string, UnityEngine.Object>>();
            m_AssetBundles = new Dictionary<string, AssetBundle>();
        }

        /// <summary>
        /// Check if asset bundle is loaded
        /// </summary>
        /// <param name="assetBundle"></param>
        /// <returns></returns>
        public bool HasCachedAssetBundle(string assetBundle)
        {
            return m_Objects.ContainsKey(assetBundle);
        }

        /// <summary>
        /// Check if object and asset bundle are loaded
        /// </summary>
        /// <param name="assetBundle"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public bool HasCachedObject(string assetBundle, string objectName)
        {
            return HasCachedAssetBundle(assetBundle) && m_Objects[assetBundle].ContainsKey(objectName);
        }

        public AssetBundle GetAssetBundle(string assetBundle)
        {
            return !HasCachedAssetBundle(assetBundle) ? null : m_AssetBundles[assetBundle];
        }

        public T GetObject<T>(string assetBundle, string objectName) where T : UnityEngine.Object
        {
            if (!HasCachedAssetBundle(assetBundle))
            {
                cacheAssetBundle(assetBundle, AssetBundle.LoadFromFile(ModCore.folder + ASSET_BUNDLES_FOLDER + assetBundle));
            }

            if (!HasCachedObject(assetBundle, objectName))
            {
                cacheObject(assetBundle, objectName, GetAssetBundle(assetBundle).LoadAsset<T>(objectName));
            }
            return (T)m_Objects[assetBundle][objectName];
        }

        private void cacheAssetBundle(string assetBundle, AssetBundle bundle)
        {
            if (HasCachedAssetBundle(assetBundle))
            {
                return;
            }
            m_Objects.Add(assetBundle, new Dictionary<string, UnityEngine.Object>());
            m_AssetBundles.Add(assetBundle, bundle);
        }

        private void cacheObject(string assetBundle, string objectName, UnityEngine.Object @object)
        {
            if (!HasCachedAssetBundle(assetBundle))
            {
                return;
            }

            if (HasCachedObject(assetBundle, objectName))
            {
                return;
            }

            m_Objects[assetBundle].Add(objectName, @object);
        }

        public void UnloadAssetBundle(string assetBundle)
        {
            if (!HasCachedAssetBundle(assetBundle))
            {
                return;
            }

            AssetBundle bundle = m_AssetBundles[assetBundle];
            bundle.Unload(true);

            _ = m_Objects.Remove(assetBundle);
            _ = m_AssetBundles.Remove(assetBundle);
        }

        public override string ToString()
        {
            return $"ModResources, cached ABs: {cachedAsseBundleCount}, cached objects: {cachedObjectCount}";
        }

        public static T Load<T>(string assetBundle, string objectName) where T : UnityEngine.Object
        {
            return Instance.GetObject<T>(assetBundle, objectName);
        }
    }
}
