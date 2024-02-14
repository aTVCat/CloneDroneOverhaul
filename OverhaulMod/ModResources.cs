using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod
{
    public class ModResources : Singleton<ModResources>
    {
        internal const string ASSET_BUNDLES_FOLDER = "assets/assetBundles/";

        private Dictionary<string, Dictionary<string, UnityEngine.Object>> m_assets;

        private Dictionary<string, AssetBundle> m_bundles;

        private List<string> m_loadingBundles;

        private List<string> m_loadingAssets;

        public int loadedAssetBundlesCount
        {
            get
            {
                return m_assets.Count;
            }
        }

        public int loadedAssetsCount
        {
            get
            {
                int count = 0;
                foreach (string key in m_assets.Keys)
                {
                    count += m_assets[key].Count;
                }
                return count;
            }
        }

        public override void Awake()
        {
            base.Awake();

            m_assets = new Dictionary<string, Dictionary<string, UnityEngine.Object>>();
            m_bundles = new Dictionary<string, AssetBundle>();
            m_loadingBundles = new List<string>();
            m_loadingAssets = new List<string>();
        }

        /// <summary>
        /// Check if asset bundle is loaded
        /// </summary>
        /// <param name="assetBundle"></param>
        /// <returns></returns>
        public bool IsBundleLoaded(string assetBundle)
        {
            return m_assets.ContainsKey(assetBundle);
        }

        /// <summary>
        /// Check if object and asset bundle are loaded
        /// </summary>
        /// <param name="assetBundle"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public bool IsAssetLoaded(string assetBundle, string objectName)
        {
            return IsBundleLoaded(assetBundle) && m_assets[assetBundle].ContainsKey(objectName);
        }

        public AssetBundle GetBundle(string assetBundle)
        {
            return !IsBundleLoaded(assetBundle) ? null : m_bundles[assetBundle];
        }

        public T LoadAsset<T>(string assetBundle, string objectName, string startPath = ASSET_BUNDLES_FOLDER) where T : UnityEngine.Object
        {
            if (!IsBundleLoaded(assetBundle))
            {
                cacheAssetBundle(assetBundle, AssetBundle.LoadFromFile(ModCore.folder + startPath + assetBundle));
            }

            if (!IsAssetLoaded(assetBundle, objectName))
            {
                cacheObject(assetBundle, objectName, GetBundle(assetBundle).LoadAsset<T>(objectName));
            }
            return (T)m_assets[assetBundle][objectName];
        }

        public void LoadAssetAsync<T>(string assetBundle, string objectName, Action<T> callback, Action<string> errorCallback, string startPath = ASSET_BUNDLES_FOLDER) where T : UnityEngine.Object
        {
            if (IsAssetLoaded(assetBundle, objectName))
            {
                callback?.Invoke((T)m_assets[assetBundle][objectName]);
                return;
            }

            _ = ModActionUtils.RunCoroutine(loadAssetAsyncCoroutine(assetBundle, objectName, typeof(T), delegate (UnityEngine.Object obj)
            {
                callback?.Invoke((T)obj);
            }, errorCallback, startPath));
        }

        public AssetBundle LoadBundle(string assetBundle, string startPath = ASSET_BUNDLES_FOLDER)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(ModCore.folder + startPath + assetBundle);
            if (!IsBundleLoaded(assetBundle))
                cacheAssetBundle(assetBundle, bundle);
            return bundle;
        }

        public void LoadBundleAsync(string assetBundle, Action<AssetBundle> successCallback, Action<string> errorCallback, string startPath = ASSET_BUNDLES_FOLDER)
        {
            string bundlePath = ModCore.folder + startPath + assetBundle;
            if (IsBundleLoaded(assetBundle))
            {
                successCallback?.Invoke(GetBundle(assetBundle));
                return;
            }

            if (m_loadingBundles.Contains(bundlePath))
            {
                _ = ModActionUtils.RunCoroutine(waitUntilBundleIsLoadedCoroutine(bundlePath, delegate (AssetBundle b)
                {
                    successCallback?.Invoke(b);
                }, errorCallback));
                return;
            }

            _ = ModActionUtils.RunCoroutine(loadBundleCoroutine(bundlePath, delegate (AssetBundle b)
            {
                successCallback?.Invoke(b);
            }, errorCallback));
        }

        private IEnumerator loadAssetAsyncCoroutine(string assetBundle, string objectName, Type assetType, Action<UnityEngine.Object> callback, Action<string> errorCallback, string startPath = ASSET_BUNDLES_FOLDER)
        {
            string errorString = null;
            if (!IsBundleLoaded(assetBundle))
            {
                string bundleName = ModCore.folder + startPath + assetBundle;
                bool bundleLoadDone = false;
                AssetBundle bundle = null;

                if (m_loadingBundles.Contains(bundleName))
                {
                    yield return new WaitUntil(() => !m_loadingBundles.Contains(bundleName));
                    bundleLoadDone = true;
                    bundle = GetBundle(assetBundle);
                    if (!bundle)
                    {
                        errorCallback?.Invoke("Bundle load failed.");
                        yield break;
                    }
                }
                else
                {
                    _ = ModActionUtils.RunCoroutine(loadBundleCoroutine(bundleName, delegate (AssetBundle b)
                    {
                        bundle = b;
                        bundleLoadDone = true;
                    }, delegate (string error)
                    {
                        errorString = error;
                        bundleLoadDone = true;
                    }));
                }

                while (!bundleLoadDone)
                    yield return null;

                if (!string.IsNullOrEmpty(errorString))
                {
                    errorCallback?.Invoke(errorString);
                    yield break;
                }
            }

            bool assetLoadDone = false;
            UnityEngine.Object asset = null;
            _ = ModActionUtils.RunCoroutine(loadAssetCoroutine(objectName, assetBundle, assetType, delegate (UnityEngine.Object obj)
            {
                asset = obj;
                assetLoadDone = true;
            }, delegate (string error)
            {
                errorString = error;
                assetLoadDone = true;
            }));

            while (!assetLoadDone)
                yield return null;

            if (!string.IsNullOrEmpty(errorString))
            {
                errorCallback?.Invoke(errorString);
                yield break;
            }
            callback?.Invoke(asset);
            yield break;
        }

        private IEnumerator loadBundleCoroutine(string bundlePath, Action<AssetBundle> callback, Action<string> errorCallback)
        {
            m_loadingBundles.Add(bundlePath);
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
            yield return request;
            if (!request.assetBundle)
            {
                _ = m_loadingBundles.Remove(bundlePath);
                errorCallback?.Invoke("Asset bundle load failed.");
                yield break;
            }
            cacheAssetBundle(bundlePath.Substring(bundlePath.LastIndexOf('/') + 1), request.assetBundle);
            callback?.Invoke(request.assetBundle);
            _ = m_loadingBundles.Remove(bundlePath);
            yield break;
        }

        private IEnumerator loadAssetCoroutine(string assetPath, string assetBundle, Type assetType, Action<UnityEngine.Object> callback, Action<string> errorCallback)
        {
            string value = $"{assetBundle}.{assetPath}";
            m_loadingAssets.Add(value);
            AssetBundleRequest request = GetBundle(assetBundle).LoadAssetAsync(assetPath, assetType);
            yield return request;
            if (!request.asset)
            {
                _ = m_loadingAssets.Remove(value);
                errorCallback?.Invoke("Asset load failed.");
                yield break;
            }
            cacheObject(assetBundle, assetPath, request.asset);
            callback?.Invoke(request.asset);
            _ = m_loadingAssets.Remove(value);
            yield break;
        }

        private IEnumerator waitUntilBundleIsLoadedCoroutine(string bundlePath, Action<AssetBundle> callback, Action<string> errorCallback)
        {
            string sub = bundlePath.Substring(bundlePath.LastIndexOf('/') + 1);
            yield return new WaitUntil(() => !m_loadingBundles.Contains(bundlePath));
            if (m_bundles.ContainsKey(sub))
            {
                callback?.Invoke(m_bundles[sub]);
            }
            else
            {
                errorCallback?.Invoke("Could not load asset bundle");
            }
            yield break;
        }

        private void cacheAssetBundle(string assetBundle, AssetBundle bundle)
        {
            if (IsBundleLoaded(assetBundle))
            {
                return;
            }
            m_assets.Add(assetBundle, new Dictionary<string, UnityEngine.Object>());
            m_bundles.Add(assetBundle, bundle);
        }

        private void cacheObject(string assetBundle, string objectName, UnityEngine.Object @object)
        {
            if (!IsBundleLoaded(assetBundle))
            {
                return;
            }

            if (IsAssetLoaded(assetBundle, objectName))
            {
                return;
            }

            m_assets[assetBundle].Add(objectName, @object);
        }

        public void UnloadAssetBundle(string assetBundle)
        {
            if (!IsBundleLoaded(assetBundle))
            {
                return;
            }

            AssetBundle bundle = m_bundles[assetBundle];
            bundle.Unload(true);

            _ = m_assets.Remove(assetBundle);
            _ = m_bundles.Remove(assetBundle);
        }

        public override string ToString()
        {
            return $"ModResources, loaded bundles/assets: {loadedAssetBundlesCount}/{loadedAssetsCount}";
        }

        public static T Load<T>(string assetBundle, string objectName, string startPath = ASSET_BUNDLES_FOLDER) where T : UnityEngine.Object
        {
            return Instance.LoadAsset<T>(assetBundle, objectName, startPath);
        }
    }
}
