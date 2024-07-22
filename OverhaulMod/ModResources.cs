using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OverhaulMod
{
    /// <summary>
    /// todo: rework in 0.4.1
    /// </summary>
    public class ModResources : Singleton<ModResources>
    {
        internal const string ASSET_BUNDLES_FOLDER = "assets/";

        private static readonly Dictionary<string, Dictionary<string, UnityEngine.Object>> s_assets = new Dictionary<string, Dictionary<string, UnityEngine.Object>>();

        private static readonly Dictionary<string, AssetBundle> s_bundles = new Dictionary<string, AssetBundle>();

        private static readonly List<string> s_loadingBundles = new List<string>();

        private static readonly List<string> s_loadingAssets = new List<string>();

        private void OnDestroy()
        {
            if (!s_bundles.IsNullOrEmpty())
                foreach (AssetBundle assetBundle in s_bundles.Values)
                    if (assetBundle)
                        assetBundle.Unload(false);

            s_bundles.Clear();
            s_assets.Clear();
        }

        /// <summary>
        /// Check if asset bundle is loaded
        /// </summary>
        /// <param name="assetBundle"></param>
        /// <returns></returns>
        public bool IsBundleLoaded(string assetBundle)
        {
            return s_assets.ContainsKey(assetBundle);
        }

        /// <summary>
        /// Check if object and asset bundle are loaded
        /// </summary>
        /// <param name="assetBundle"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public bool IsAssetLoaded(string assetBundle, string objectName)
        {
            return IsBundleLoaded(assetBundle) && s_assets[assetBundle].ContainsKey(objectName);
        }

        public AssetBundle GetBundle(string assetBundle)
        {
            return !IsBundleLoaded(assetBundle) ? null : s_bundles[assetBundle];
        }

        public T LoadAsset<T>(string assetBundle, string objectName, string startPath = null) where T : UnityEngine.Object
        {
            if (!IsBundleLoaded(assetBundle))
            {
                string bundleName = startPath == null ? Path.Combine(ModCore.folder, ASSET_BUNDLES_FOLDER, assetBundle) : Path.Combine(startPath, assetBundle);
                cacheAssetBundle(assetBundle, AssetBundle.LoadFromFile(bundleName));
            }

            if (!IsAssetLoaded(assetBundle, objectName))
            {
                cacheObject(assetBundle, objectName, GetBundle(assetBundle).LoadAsset<T>(objectName));
            }
            return (T)s_assets[assetBundle][objectName];
        }

        public void LoadAssetAsync<T>(string assetBundle, string objectName, Action<T> callback, Action<string> errorCallback, string startPath = null) where T : UnityEngine.Object
        {
            if (IsAssetLoaded(assetBundle, objectName))
            {
                callback?.Invoke((T)s_assets[assetBundle][objectName]);
                return;
            }

            _ = ModActionUtils.RunCoroutine(loadAssetAsyncCoroutine(assetBundle, objectName, typeof(T), delegate (UnityEngine.Object obj)
            {
                try
                {
                    callback?.Invoke((T)obj);
                }
                catch
                {

                }
            }, errorCallback, startPath), true);
        }

        public AssetBundle LoadBundle(string assetBundle, string startPath = null)
        {
            string bundleName = startPath == null ? Path.Combine(ModCore.folder, ASSET_BUNDLES_FOLDER, assetBundle) : Path.Combine(startPath, assetBundle);
            AssetBundle bundle = AssetBundle.LoadFromFile(bundleName);
            if (!IsBundleLoaded(assetBundle))
                cacheAssetBundle(assetBundle, bundle);
            return bundle;
        }

        public void LoadBundleAsync(string assetBundle, Action<AssetBundle> successCallback, Action<string> errorCallback, string startPath = null)
        {
            string bundlePath = startPath == null ? Path.Combine(ModCore.folder, ASSET_BUNDLES_FOLDER, assetBundle) : Path.Combine(startPath, assetBundle);
            if (IsBundleLoaded(assetBundle))
            {
                successCallback?.Invoke(GetBundle(assetBundle));
                return;
            }

            if (s_loadingBundles.Contains(bundlePath))
            {
                _ = ModActionUtils.RunCoroutine(waitUntilBundleIsLoadedCoroutine(bundlePath, delegate (AssetBundle b)
                {
                    try
                    {
                        successCallback?.Invoke(b);
                    }
                    catch
                    {

                    }
                }, errorCallback), true);
                return;
            }

            _ = ModActionUtils.RunCoroutine(loadBundleCoroutine(bundlePath, delegate (AssetBundle b)
            {
                try
                {
                    successCallback?.Invoke(b);
                }
                catch
                {

                }
            }, errorCallback), true);
        }

        private IEnumerator loadAssetAsyncCoroutine(string assetBundle, string objectName, Type assetType, Action<UnityEngine.Object> callback, Action<string> errorCallback, string startPath = null)
        {
            string errorString = null;
            string bundleName = startPath == null ? Path.Combine(ModCore.folder, ASSET_BUNDLES_FOLDER, assetBundle) : Path.Combine(startPath, assetBundle);
            bool bundleLoadDone = false;
            AssetBundle bundle = null;

            if (s_loadingBundles.Contains(bundleName))
            {
                yield return new WaitUntil(() => !s_loadingBundles.Contains(bundleName));
                bundleLoadDone = true;
                bundle = GetBundle(assetBundle);
                if (!bundle)
                {
                    errorCallback?.Invoke("Bundle load failed.");
                    yield break;
                }
            }
            else if (!IsBundleLoaded(assetBundle))
            {
                _ = ModActionUtils.RunCoroutine(loadBundleCoroutine(bundleName, delegate (AssetBundle b)
                {
                    bundle = b;
                    bundleLoadDone = true;
                }, delegate (string error)
                {
                    errorString = error;
                    bundleLoadDone = true;
                }), true);

                while (!bundleLoadDone)
                    yield return null;
            }

            if (!errorString.IsNullOrEmpty())
            {
                errorCallback?.Invoke(errorString);
                yield break;
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
            }), true);

            while (!assetLoadDone)
                yield return null;

            if (!errorString.IsNullOrEmpty())
            {
                errorCallback?.Invoke(errorString);
                yield break;
            }
            callback?.Invoke(asset);
            yield break;
        }

        private IEnumerator loadBundleCoroutine(string bundlePath, Action<AssetBundle> callback, Action<string> errorCallback)
        {
            s_loadingBundles.Add(bundlePath);
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
            yield return request;
            if (!request.assetBundle)
            {
                _ = s_loadingBundles.Remove(bundlePath);
                errorCallback?.Invoke("Asset bundle load failed.");
                yield break;
            }
            cacheAssetBundle(Path.GetFileName(bundlePath), request.assetBundle);
            callback?.Invoke(request.assetBundle);
            _ = s_loadingBundles.Remove(bundlePath);
            yield break;
        }

        private IEnumerator loadAssetCoroutine(string assetPath, string assetBundle, Type assetType, Action<UnityEngine.Object> callback, Action<string> errorCallback)
        {
            string value = $"{assetBundle}.{assetPath}";
            s_loadingAssets.Add(value);
            AssetBundleRequest request = GetBundle(assetBundle).LoadAssetAsync(assetPath, assetType);
            yield return request;
            if (!request.asset)
            {
                _ = s_loadingAssets.Remove(value);
                errorCallback?.Invoke("Asset load failed.");
                yield break;
            }
            cacheObject(assetBundle, assetPath, request.asset);
            callback?.Invoke(request.asset);
            _ = s_loadingAssets.Remove(value);
            yield break;
        }

        private IEnumerator waitUntilBundleIsLoadedCoroutine(string bundlePath, Action<AssetBundle> callback, Action<string> errorCallback)
        {
            string sub = Path.GetFileName(bundlePath);
            yield return new WaitUntil(() => !s_loadingBundles.Contains(bundlePath));
            if (s_bundles.ContainsKey(sub))
            {
                callback?.Invoke(s_bundles[sub]);
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
            s_assets.Add(assetBundle, new Dictionary<string, UnityEngine.Object>());
            s_bundles.Add(assetBundle, bundle);
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

            s_assets[assetBundle].Add(objectName, @object);
        }

        public void UnloadAssetBundle(string assetBundle)
        {
            if (!IsBundleLoaded(assetBundle))
            {
                return;
            }

            AssetBundle bundle = s_bundles[assetBundle];
            bundle.Unload(true);

            _ = s_assets.Remove(assetBundle);
            _ = s_bundles.Remove(assetBundle);
        }

        public static T Load<T>(string assetBundle, string objectName, string startPath = null) where T : UnityEngine.Object
        {
            return Instance.LoadAsset<T>(assetBundle, objectName, startPath);
        }
    }
}
