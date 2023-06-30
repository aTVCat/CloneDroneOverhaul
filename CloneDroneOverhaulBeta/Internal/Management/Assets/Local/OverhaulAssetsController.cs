using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulAssetsController
    {
        #region File names
        public const string ModAssetBundle_Part1 = "overhaulassets_p1";

        public const string ModAssetBundle_Part2 = "overhaulassets_p2";

        public const string ModAssetBundle_Skins = "overhaulassets_skins";

        public const string ModAssetBundle_Objects = "overhaulassets_objects";

        public const string ModAssetBundle_Accessouries = "overhaulassets_accessories";

        public const string ModAssetBundle_Sounds = "overhaulassets_sounds";

        public const string ModAssetBundle_Main = "overhaulassets_main";

        public const string ModAssetBundle_CombatUpdate = "overhaulassets_combatupdatestuff";

        public const string ModAssetBundle_Preload = "overhaulassets_preload";

        public const string ModAssetBundle_Experiments = "overhaulassets_experiments";

        public const string ModAssetBundle_Pets = "overhaulassets_pets";
        #endregion

        /// <summary>
        /// All loaded assets into memory
        /// </summary>
        private static readonly Dictionary<string, AssetBundle> m_LoadedAssetBundles = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// The list of loading asset bundles at the moment
        /// </summary>
        private static readonly List<string> m_LoadingAssetBundles = new List<string>();

        /// <summary>
        /// Try load an asset bundle if one exists
        /// </summary>
        /// <param name="pathUnderModFolder"></param>
        /// <returns><b>True</b> if asset bundle has been successfully loaded</returns>
        public static bool TryLoadAssetBundle(in string pathUnderModFolder)
        {
            if (HasLoadedAssetBundle(pathUnderModFolder) || !DoesAssetBundleExist(pathUnderModFolder))
                return false;

            AssetBundle loadedAssetBundle;
            try
            {
                loadedAssetBundle = AssetBundle.LoadFromFile(OverhaulCore.ModDirectoryStatic + pathUnderModFolder);
            }
            catch
            {
                return false;
            }
            m_LoadedAssetBundles.Add(pathUnderModFolder, loadedAssetBundle);
            return true;
        }

        /// <summary>
        /// Load asset bundle async
        /// </summary>
        /// <param name="pathUnderModFolder"></param>
        /// <param name="doneCallback"></param>
        /// <returns><see cref="AssetLoadHandler"/> containing progress variable</returns>
        public static AssetBundleLoadHandler LoadAssetBundleAsync(in string pathUnderModFolder, Action<AssetBundleLoadHandler> doneCallback)
        {
            if (doneCallback == null || IsLoadingAssetBundle(pathUnderModFolder) || !DoesAssetBundleExist(pathUnderModFolder))
                return null;

            AssetBundleLoadHandler handler = new AssetBundleLoadHandler(doneCallback, pathUnderModFolder);
            if (HasLoadedAssetBundle(pathUnderModFolder))
                return handler;

            _ = StaticCoroutineRunner.StartStaticCoroutine(loadAssetBundleAsyncCoroutine(pathUnderModFolder, handler));
            return handler;
        }

        private static IEnumerator loadAssetBundleAsyncCoroutine(string pathUnderModFolder, AssetBundleLoadHandler handler)
        {
            m_LoadingAssetBundles.Add(pathUnderModFolder);
            AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(OverhaulCore.ModDirectoryStatic + pathUnderModFolder);
            handler.AsignRequest(assetBundleCreateRequest);

            while (!assetBundleCreateRequest.isDone)
                yield return null;

            handler.OnDoneLoading();
            yield break;
        }

        /// <summary>
        /// Unload an asset bundle from memory
        /// </summary>
        /// <param name="pathUnderModFolder"></param>
        /// <param name="unloadObjects">Destroy loaded objects to free memory</param>
        /// <returns><b>True</b> if asset bundle has been successfully unloaded</returns>
        public static bool TryUnloadAssetBundle(in string pathUnderModFolder, in bool unloadObjects = false)
        {
            if (!HasLoadedAssetBundle(pathUnderModFolder))
                return false;

            try
            {
                AssetBundle bundleToUnload = m_LoadedAssetBundles[pathUnderModFolder];
                bundleToUnload.Unload(unloadObjects);
            }
            catch
            {
                return false;
            }
            _ = m_LoadedAssetBundles.Remove(pathUnderModFolder);
            return true;
        }

        /// <summary>
        /// Try load an asset bundle if one isn't loaded
        /// </summary>
        /// <param name="pathUnderModFolder"></param>
        /// <returns><b>True</b> if asset bundle has been loaded</returns>
        public static bool LoadAssetBundleIfNotLoaded(in string pathUnderModFolder) => DoesAssetBundleExist(pathUnderModFolder) && (HasLoadedAssetBundle(pathUnderModFolder) || TryLoadAssetBundle(pathUnderModFolder));

        /// <summary>
        /// Check if there's specific asset bundle on disk under mod directory
        /// </summary>
        /// <param name="assetBundleUnderModDirectory"></param>
        /// <returns></returns>
        public static bool DoesAssetBundleExist(in string assetBundleUnderModDirectory)
        {
            string path = OverhaulCore.ModDirectoryStatic + assetBundleUnderModDirectory;
            return File.Exists(path);
        }

        /// <summary>
        /// Check if specified asset bundle is already loaded into memory and can be used
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        public static bool HasLoadedAssetBundle(in string assetBundleName) => m_LoadedAssetBundles.ContainsKey(assetBundleName);

        /// <summary>
        /// Check if specified asset bundle is being loaded async-ly
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        public static bool IsLoadingAssetBundle(in string assetBundleName) => m_LoadingAssetBundles.Contains(assetBundleName);

        public static T GetAsset<T>(in string assetName, in OverhaulAssetPart assetBundlePart) where T : UnityEngine.Object
        {
            string assetBundle = null;
            switch (assetBundlePart)
            {
                case OverhaulAssetPart.Part1:
                    assetBundle = ModAssetBundle_Part1;
                    break;
                case OverhaulAssetPart.Part2:
                    assetBundle = ModAssetBundle_Part2;
                    break;
                case OverhaulAssetPart.WeaponSkins:
                    assetBundle = ModAssetBundle_Skins;
                    break;
                case OverhaulAssetPart.Objects:
                    assetBundle = ModAssetBundle_Objects;
                    break;
                case OverhaulAssetPart.Accessories:
                    assetBundle = ModAssetBundle_Accessouries;
                    break;
                case OverhaulAssetPart.Sounds:
                    assetBundle = ModAssetBundle_Sounds;
                    break;
                case OverhaulAssetPart.Main:
                    assetBundle = ModAssetBundle_Main;
                    break;
                case OverhaulAssetPart.Combat_Update:
                    assetBundle = ModAssetBundle_CombatUpdate;
                    break;
                case OverhaulAssetPart.Preload:
                    assetBundle = ModAssetBundle_Preload;
                    break;
                case OverhaulAssetPart.Experiments:
                    assetBundle = ModAssetBundle_Experiments;
                    break;
                case OverhaulAssetPart.Pets:
                    assetBundle = ModAssetBundle_Pets;
                    break;
            }

            if (!LoadAssetBundleIfNotLoaded(assetBundle)) return null;
            T result = m_LoadedAssetBundles[assetBundle].LoadAsset<T>(assetName);
            return result;
        }

        public static T GetAsset<T>(in string assetName, in string assetBundleFileName) where T : UnityEngine.Object
        {
            if (!LoadAssetBundleIfNotLoaded(assetBundleFileName)) return null;

            T result = m_LoadedAssetBundles[assetBundleFileName].LoadAsset<T>(assetName);
            return result;
        }

        public static GameObject GetAsset(in string assetName, in OverhaulAssetPart assetBundlePart) => GetAsset<GameObject>(assetName, assetBundlePart);

        public static bool TryGetAsset<T>(in string assetName, in string assetBundle, out T asset) where T : UnityEngine.Object
        {
            try
            {
                asset = GetAsset<T>(assetName, assetBundle);
                return true;
            }
            catch
            {
                asset = null;
            }
            return false;
        }

        public static AssetLoadHandler GetAssetAsync(in string assetBundle, in string assetName, Action<AssetLoadHandler> doneCallback)
        {
            if (doneCallback == null || IsLoadingAssetBundle(assetBundle) || !HasLoadedAssetBundle(assetBundle))
                return null;

            AssetBundle bundle = m_LoadedAssetBundles[assetBundle];
            AssetLoadHandler handler = new AssetLoadHandler(doneCallback, bundle);
            _ = StaticCoroutineRunner.StartStaticCoroutine(getAssetAsyncCoroutine(bundle, assetName, handler));
            return handler;
        }

        private static IEnumerator getAssetAsyncCoroutine(AssetBundle assetBundle, string assetName, AssetLoadHandler handler)
        {
            AssetBundleRequest assetBundleCreateRequest = assetBundle.LoadAssetAsync(assetName);
            handler.AsignRequest(assetBundleCreateRequest);

            while (!assetBundleCreateRequest.isDone)
                yield return null;

            handler.OnDoneLoading();
            yield break;
        }

        public static void PreloadAsset<T>(in string assetName, in string assetBundleFileName) where T : UnityEngine.Object => _ = GetAsset<T>(assetName, assetBundleFileName);

        public class AssetLoadHandler : OverhaulDisposable
        {
            public AsyncOperation Request;
            public Action<AssetLoadHandler> DoneAction;

            public AssetBundle AssetBundle;

            public AssetLoadHandler(Action<AssetLoadHandler> onDone, AssetBundle bundle)
            {
                DoneAction = onDone;
                AssetBundle = bundle;
            }

            protected override void OnDisposed()
            {
                Request = null;
                DoneAction = null;
                AssetBundle = null;
            }

            public void AsignRequest(AssetBundleRequest request)
            {
                if (IsDisposed)
                    return;

                Request = request;
            }

            public void OnDoneLoading()
            {
                DoneAction(this);
                Dispose();
            }
        }

        public class AssetBundleLoadHandler : OverhaulDisposable
        {
            public AsyncOperation Request;
            public Action<AssetBundleLoadHandler> DoneAction;

            public string AssetBundlePath;

            public AssetBundleLoadHandler(Action<AssetBundleLoadHandler> onDone, string bundleName)
            {
                DoneAction = onDone;
                AssetBundlePath = bundleName;
            }

            protected override void OnDisposed()
            {
                Request = null;
                DoneAction = null;
                AssetBundlePath = null;
            }

            public void AsignRequest(AssetBundleCreateRequest request)
            {
                if (IsDisposed)
                    return;

                Request = request;
            }

            public void OnDoneLoading()
            {
                _ = m_LoadingAssetBundles.Remove(AssetBundlePath);
                m_LoadedAssetBundles.Add(AssetBundlePath, (Request as AssetBundleCreateRequest).assetBundle);
                DoneAction(this);
                Dispose();
            }
        }
    }
}
