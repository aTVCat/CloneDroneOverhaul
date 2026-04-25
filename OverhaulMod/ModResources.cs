using OverhaulMod.Content;
using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OverhaulMod
{
    public class ModResources : Singleton<ModResources>
    {
        public const string ASSET_BUNDLE_LOADED_EVENT = "AssetBundleLoaded";

        private static readonly Dictionary<AssetBundlePath, AssetBundleInfo> s_bundles = new Dictionary<AssetBundlePath, AssetBundleInfo>();

        private void OnDestroy()
        {
            unloadAssets();
        }

        private void unloadAssets()
        {
            foreach (AssetBundleInfo bundle in s_bundles.Values) bundle.Unload();
            s_bundles.Clear();
        }

        private AssetBundleInfo getOrCreateAssetBundleInfo(AssetBundlePath path, bool load)
        {
            AssetBundleInfo assetBundleInfo;
            if (!s_bundles.ContainsKey(path))
            {
                assetBundleInfo = new AssetBundleInfo(path.GetPath());
                if (load) assetBundleInfo.Load();

                s_bundles.Add(path, assetBundleInfo);
            }
            else
            {
                assetBundleInfo = s_bundles[path];
            }
            return assetBundleInfo;
        }

        public T LoadAsset<T>(string bundle, string asset, string pathPrefix = null) where T : UnityEngine.Object
        {
            return getOrCreateAssetBundleInfo(GetBundlePath(bundle, pathPrefix), true).GetAsset<T>(asset);
        }

        public void LoadAssetAsync<T>(string bundle, string asset, Action<T> callback, string pathPrefix = null) where T : UnityEngine.Object
        {
            getOrCreateAssetBundleInfo(GetBundlePath(bundle, pathPrefix), true).GetAssetAsync(asset, callback);
        }

        public void LoadAssetBundle(string bundle, string pathPrefix = null)
        {
            _ = getOrCreateAssetBundleInfo(GetBundlePath(bundle, pathPrefix), true);
        }

        public void LoadAssetBundleAsync(string bundle, Action<bool> callback, string pathPrefix = null)
        {
            AssetBundleInfo bundleInfo = getOrCreateAssetBundleInfo(GetBundlePath(bundle, pathPrefix), false);
            bundleInfo.LoadAsync(callback);
        }

        public static T Load<T>(string bundle, string asset, string pathPrefix = null) where T : UnityEngine.Object
        {
            return Instance.LoadAsset<T>(bundle, asset, pathPrefix);
        }

        public static void LoadAsync<T>(string bundle, string asset, Action<T> callback, string pathPrefix = null) where T : UnityEngine.Object
        {
            Instance.LoadAssetAsync(bundle, asset, callback, pathPrefix);
        }

        public static void LoadBundle(string bundle, string pathPrefix = null)
        {
            Instance.LoadAssetBundle(bundle, pathPrefix);
        }

        public static void LoadBundleAsync(string bundle, Action<bool> callback, string pathPrefix = null)
        {
            Instance.LoadAssetBundleAsync(bundle, callback, pathPrefix);
        }

        public static AssetBundlePath GetBundlePath(string bundle, string pathPrefix = null)
        {
            return pathPrefix == null ? new AssetBundlePath(AssetBundleLocation.ModAssets, bundle) : new AssetBundlePath(AssetBundleLocation.External, Path.Combine(pathPrefix, bundle));
        }

        public static bool IsAssetBundleNotLoadedOrBeingLoaded(string bundle, string pathPrefix = null)
        {
            AssetBundleInfo assetBundleInfo = GetAssetBundleInfo(bundle, pathPrefix);
            return assetBundleInfo == null || assetBundleInfo.LoadingState != AssetLoadingState.Loaded;
        }

        public static AssetBundlePath GetAssetBundleInfoKey(AssetBundleInfo assetBundleInfo)
        {
            if (assetBundleInfo == null || !s_bundles.ContainsValue(assetBundleInfo))
                return new AssetBundlePath();

            foreach (KeyValuePair<AssetBundlePath, AssetBundleInfo> kv in s_bundles)
                if (kv.Value == assetBundleInfo)
                    return kv.Key;

            return new AssetBundlePath();
        }

        public static GameObject Prefab(string bundle, string asset, string pathPrefix = null)
        {
            return Load<GameObject>(bundle, asset, pathPrefix);
        }

        public static TextAsset TextAsset(string bundle, string asset, string pathPrefix = null)
        {
            return Load<TextAsset>(bundle, asset, pathPrefix);
        }

        public static Font Font(string bundle, string asset, string pathPrefix = null)
        {
            return Load<Font>(bundle, asset, pathPrefix);
        }

        public static AudioClip AudioClip(string bundle, string asset, string pathPrefix = null)
        {
            return Load<AudioClip>(bundle, asset, pathPrefix);
        }

        public static Texture2D Texture2D(string bundle, string asset, string pathPrefix = null)
        {
            return Load<Texture2D>(bundle, asset, pathPrefix);
        }

        public static Sprite Sprite(string bundle, string asset, string pathPrefix = null)
        {
            return Load<Sprite>(bundle, asset, pathPrefix);
        }

        public static Shader Shader(string bundle, string asset, string pathPrefix = null)
        {
            return Load<Shader>(bundle, asset, pathPrefix);
        }

        public static AssetBundle LoadAndGetAssetBundle(string bundle, string pathPrefix = null)
        {
            return Instance.getOrCreateAssetBundleInfo(GetBundlePath(bundle, pathPrefix), true).GetBundle();
        }

        public static AssetBundle AssetBundle(string bundle, string pathPrefix = null)
        {
            return GetAssetBundleInfo(bundle, pathPrefix).GetBundle();
        }

        public static AssetBundleInfo GetAssetBundleInfo(string bundle, string pathPrefix = null)
        {
            AssetBundlePath path = GetBundlePath(bundle, pathPrefix);
            if (s_bundles.ContainsKey(path)) return s_bundles[path];
            return null;
        }

        public static Font EditUndoFont()
        {
            return Load<Font>(AssetBundleConstants.UI, "Edit-Undo-BRK");
        }

        public static Font PiksieliProstoFont()
        {
            return Load<Font>(AssetBundleConstants.UI, "Piksieli-Prosto");
        }

        public static Font VSROSDMonoFont()
        {
            return Load<Font>(AssetBundleConstants.UI, "VCR-OSD-Mono");
        }

        public static Font FontByIndex(int index)
        {
            string path = null;
            bool hasAddon = AddonManager.Instance.HasInstalledAddon(AddonManager.EXTRAS_ADDON_ID, out path);

            if (index > 5 && (!hasAddon || IsAssetBundleNotLoadedOrBeingLoaded(AssetBundleConstants.UI_EXTRA, path)))
                return Font(AssetBundleConstants.UI, "OpenSans-Regular");

            switch (index)
            {
                case 1:
                    return VSROSDMonoFont();
                case 2:
                    return PiksieliProstoFont();
                case 3:
                    return EditUndoFont();
                case 5:
                    return Font(AssetBundleConstants.UI, "OpenSans-ExtraBold");
                case 6:
                    return Font(AssetBundleConstants.UI_EXTRA, "NotoSansCJKtc-Regular", path);
                case 7:
                    return Font(AssetBundleConstants.UI_EXTRA, "NotoSansCJKtc-Bold", path);
                case 8:
                    return Font(AssetBundleConstants.UI_EXTRA, "NotoSansCJKtc-Black", path);
                default:
                    return Font(AssetBundleConstants.UI, "OpenSans-Regular");
            }
        }

        public static string GetBundleName(AssetBundlePath assetBundlePath)
        {
            return Path.GetFileNameWithoutExtension(assetBundlePath.FileName);
        }

        public enum AssetLoadingState
        {
            NotLoaded,

            Loading,

            Loaded
        }

        public enum AssetBundleLocation
        {
            /// <summary>
            /// Located under <see cref="ModCore.AssetsFolder"/> Folder
            /// </summary>
            ModAssets,

            /// <summary>
            /// Located outside the mod Folder
            /// </summary>
            External,
        }

        public struct AssetBundlePath
        {
            public AssetBundleLocation Location;

            public string FileName;

            public AssetBundlePath(AssetBundleLocation location, string fileName)
            {
                Location = location;
                FileName = fileName;
            }

            public string GetPath()
            {
                if (Location == AssetBundleLocation.ModAssets)
                {
                    return Path.Combine(ModDirectories.AssetsFolder, FileName);
                }
                else if (Location == AssetBundleLocation.External)
                {
                    return FileName;
                }
                return string.Empty;
            }

            public bool IsInvalid() => FileName.IsNullOrEmpty();

            public override string ToString() => $"Asset bundle path {Location} - {FileName}";
        }

        public class AssetBundleInfo
        {
            public readonly string FileLocation;

            public AssetLoadingState LoadingState;

            private float _loadProgress;


            private AssetBundle _bundle;

            private readonly Dictionary<string, UnityEngine.Object> _cachedAssets;


            private readonly Dictionary<string, float> _assetsBeingLoaded;

            public AssetBundleInfo(string fileLocation)
            {
                FileLocation = fileLocation;
                LoadingState = AssetLoadingState.NotLoaded;
                _bundle = null;
                _cachedAssets = new Dictionary<string, UnityEngine.Object>();
                _assetsBeingLoaded = new Dictionary<string, float>();
                _loadProgress = 0f;
            }

            public AssetBundle GetBundle()
            {
                return _bundle;
            }

            public float GetBundleLoadProgress()
            {
                if (LoadingState == AssetLoadingState.NotLoaded)
                    return 0f;

                return _loadProgress;
            }

            public float GetAssetLoadProgress(string name)
            {
                if (_cachedAssets.ContainsKey(name))
                    return 1f;

                return _assetsBeingLoaded.ContainsKey(name) ? _assetsBeingLoaded[name] : 0f;
            }

            public T GetAsset<T>(string name) where T : UnityEngine.Object
            {
                if (LoadingState != AssetLoadingState.Loaded)
                    return null;

                if (_cachedAssets.TryGetValue(name, out UnityEngine.Object obj))
                    return (T)obj;

                obj = _bundle.LoadAsset<T>(name);
                if (obj)
                {
                    _cachedAssets.Add(name, obj);
                }
                return (T)obj;
            }

            public void GetAssetAsync<T>(string name, Action<T> callback) where T : UnityEngine.Object
            {
                if (LoadingState != AssetLoadingState.Loaded)
                {
                    callback?.Invoke(null);
                    return;
                }

                if (_assetsBeingLoaded.ContainsKey(name))
                {
                    _ = waitForAssetLoadCoroutine(name, callback).Run(true);
                    return;
                }

                if (_cachedAssets.TryGetValue(name, out UnityEngine.Object obj))
                {
                    callback?.Invoke((T)obj);
                    return;
                }

                _ = getAssetAsync(name, callback).Run(true);
            }

            private IEnumerator getAssetAsync<T>(string name, Action<T> callback) where T : UnityEngine.Object
            {
                _assetsBeingLoaded.Add(name, 0f);

                AssetBundleRequest assetRequest = _bundle.LoadAssetAsync<T>(name);
                while (!assetRequest.isDone)
                {
                    _assetsBeingLoaded[name] = assetRequest.progress;
                    yield return null;
                }

                _cachedAssets.Add(name, assetRequest.asset);

                _ = _assetsBeingLoaded.Remove(name);
                callback?.Invoke((T)assetRequest.asset);
                yield break;
            }

            private IEnumerator waitForAssetLoadCoroutine<T>(string name, Action<T> callback) where T : UnityEngine.Object
            {
                while (_assetsBeingLoaded.ContainsKey(name))
                    yield return null;

                if (_cachedAssets.TryGetValue(name, out UnityEngine.Object obj))
                {
                    callback?.Invoke((T)obj);
                }
                else
                {
                    callback?.Invoke(null);
                }
                yield break;
            }

            public void Load()
            {
                if (LoadingState != AssetLoadingState.NotLoaded)
                    return;

                if (!File.Exists(FileLocation))
                    return;

                LoadingState = AssetLoadingState.Loading;
                _bundle = UnityEngine.AssetBundle.LoadFromFile(FileLocation);
                _loadProgress = 1f;
                LoadingState = AssetLoadingState.Loaded;

                GlobalEventManager.Instance.Dispatch(ASSET_BUNDLE_LOADED_EVENT, ModResources.GetAssetBundleInfoKey(this));
            }

            public void LoadAsync(Action<bool> callback)
            {
                switch (LoadingState)
                {
                    case AssetLoadingState.NotLoaded:
                        if (!File.Exists(FileLocation))
                        {
                            callback?.Invoke(false);
                            return;
                        }

                        _ = loadAsyncCoroutine(callback).Run(true);
                        return;
                    case AssetLoadingState.Loading:
                        _ = waitForLoadCoroutine(callback).Run(true);
                        return;
                    case AssetLoadingState.Loaded:
                        callback?.Invoke(true);
                        return;
                }
            }

            private IEnumerator loadAsyncCoroutine(Action<bool> callback)
            {
                LoadingState = AssetLoadingState.Loading;
                AssetBundleCreateRequest createRequest = UnityEngine.AssetBundle.LoadFromFileAsync(FileLocation);
                while (!createRequest.isDone)
                {
                    _loadProgress = createRequest.progress;
                    yield return null;
                }

                if (!createRequest.assetBundle)
                {
                    _loadProgress = 0f;
                    LoadingState = AssetLoadingState.NotLoaded;
                    callback?.Invoke(false);
                    yield break;
                }

                _bundle = createRequest.assetBundle;
                _loadProgress = 1f;

                LoadingState = AssetLoadingState.Loaded;
                callback?.Invoke(true);
                GlobalEventManager.Instance.Dispatch(ASSET_BUNDLE_LOADED_EVENT, ModResources.GetAssetBundleInfoKey(this));
                yield break;
            }

            private IEnumerator waitForLoadCoroutine(Action<bool> callback)
            {
                while (LoadingState == AssetLoadingState.Loading)
                    yield return null;

                callback?.Invoke(LoadingState == AssetLoadingState.Loaded);
                yield break;
            }

            public void Unload()
            {
                if (LoadingState != AssetLoadingState.Loaded)
                    return;

                _assetsBeingLoaded.Clear();
                _cachedAssets.Clear();
                _bundle.Unload(false);
                _bundle = null;

                LoadingState = AssetLoadingState.NotLoaded;
                _loadProgress = 0f;
            }
        }
    }
}
