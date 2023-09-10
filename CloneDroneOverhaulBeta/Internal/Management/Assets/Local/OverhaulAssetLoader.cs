using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulAssetLoader
    {
        #region File names
        public const string ModAssetBundle_Part1 = "overhaulassets_p1";

        public const string ModAssetBundle_Part2 = "overhaulassets_p2";

        public const string ModAssetBundle_Skins = "overhaulassets_skins";

        public const string ModAssetBundle_Objects = "overhaulassets_objects";

        public const string ModAssetBundle_Accessouries = "overhaulassets_accessories";

        public const string ModAssetBundle_Sounds = "overhaulassets_sounds";

        public const string ModAssetBundle_Main = "overhaulassets_main";

        public const string ModAssetBundle_Preload = "overhaulassets_preload";

        public const string ModAssetBundle_Experiments = "overhaulassets_experiments";

        public const string ModAssetBundle_Pets = "overhaulassets_pets";

        public const string ModAssetBundle_ArenaOverhaul = "overhaulassets_arenaupdate";

        public const string ModAssetBundle_Fonts = "overhaulassets_fonts";
        #endregion

        public const string STANDARD_SHADER = "Standard";

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
        public static bool TryLoadAssetBundle(in string pathUnderModFolder, bool fixMaterials = true)
        {
            if (HasLoadedAssetBundle(pathUnderModFolder) || !DoesAssetBundleExist(pathUnderModFolder))
                return false;

            AssetBundle loadedAssetBundle;
            try
            {
                loadedAssetBundle = AssetBundle.LoadFromFile(OverhaulCore.ModDirectoryStatic + pathUnderModFolder);
                if (fixMaterials)
                {
                    Material[] materials = loadedAssetBundle.LoadAllAssets<Material>(); // No fog fix
                    foreach (Material material in materials)
                        if (material.shader.name == STANDARD_SHADER)
                            material.shader = Shader.Find(STANDARD_SHADER);
                }
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
        public static AssetBundleLoadHandler LoadAssetBundleAsync(in string pathUnderModFolder, Action<AssetBundleLoadHandler> doneCallback, bool fixMaterials = true)
        {
            if (doneCallback == null)
                return null;

            AssetBundleLoadHandler handler = new AssetBundleLoadHandler(doneCallback, pathUnderModFolder);
            if (IsLoadingAssetBundle(pathUnderModFolder) || !DoesAssetBundleExist(pathUnderModFolder) || HasLoadedAssetBundle(pathUnderModFolder))
                return handler;

            _ = StaticCoroutineRunner.StartStaticCoroutine(loadAssetBundleAsyncCoroutine(pathUnderModFolder, handler, fixMaterials));
            return handler;
        }

        private static IEnumerator loadAssetBundleAsyncCoroutine(string pathUnderModFolder, AssetBundleLoadHandler handler, bool fixMaterials)
        {
            m_LoadingAssetBundles.Add(pathUnderModFolder);
            AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(OverhaulCore.ModDirectoryStatic + pathUnderModFolder);
            handler.AsignRequest(assetBundleCreateRequest);

            while (!assetBundleCreateRequest.isDone)
                yield return null;

            if (fixMaterials)
            {
                Material[] materials = assetBundleCreateRequest.assetBundle.LoadAllAssets<Material>(); // No fog fix
                foreach (Material material in materials)
                    if (material.shader.name == STANDARD_SHADER)
                        material.shader = Shader.Find(STANDARD_SHADER);
            }

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

        public static void UnloadAllAssetBundles()
        {
            foreach (AssetBundle bundle in m_LoadedAssetBundles.Values)
            {
                bundle.Unload(true);
            }
            m_LoadedAssetBundles.Clear();
        }

        /// <summary>
        /// Try load an asset bundle if one isn't loaded
        /// </summary>
        /// <param name="pathUnderModFolder"></param>
        /// <returns><b>True</b> if asset bundle has been loaded</returns>
        public static bool LoadAssetBundleIfNotLoaded(in string pathUnderModFolder, bool fixMaterials = true) => DoesAssetBundleExist(pathUnderModFolder) && (HasLoadedAssetBundle(pathUnderModFolder) || TryLoadAssetBundle(pathUnderModFolder, fixMaterials));

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
        public static string GetLoadedAssetBundlesString()
        {
            string result = string.Empty;

            if (m_LoadedAssetBundles.IsNullOrEmpty())
                return result;

            foreach (string name in m_LoadedAssetBundles.Keys)
                result += name + "\n";

            return result;
        }

        public static float GetAllAssetBundlesLoadPercent()
        {
            if (m_LoadedAssetBundles.IsNullOrEmpty())
                return 0f;

            int all = m_LoadingAssetBundles.Count;
            float percents = 0f;
            foreach (AssetBundleLoadHandler name in AssetBundleLoadHandler.LoadingBundles.Values)
                percents += name != null && name.Request != null ? name.Request.progress : 0f;

            return percents / all;
        }

        /// <summary>
        /// Check if specified asset bundle is being loaded async-ly
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        public static bool IsLoadingAssetBundle(in string assetBundleName) => m_LoadingAssetBundles.Contains(assetBundleName);

        public static T GetAsset<T>(in string assetName, in OverhaulAssetPart assetBundlePart, bool fixMaterials = true) where T : UnityEngine.Object
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
                case OverhaulAssetPart.Preload:
                    assetBundle = ModAssetBundle_Preload;
                    break;
                case OverhaulAssetPart.Experiments:
                    assetBundle = ModAssetBundle_Experiments;
                    break;
                case OverhaulAssetPart.Pets:
                    assetBundle = ModAssetBundle_Pets;
                    break;
                case OverhaulAssetPart.ArenaOverhaul:
                    assetBundle = ModAssetBundle_ArenaOverhaul;
                    break;
                case OverhaulAssetPart.Fonts:
                    assetBundle = ModAssetBundle_Fonts;
                    break;
            }

            if (!LoadAssetBundleIfNotLoaded(assetBundle, fixMaterials)) return null;
            T result = m_LoadedAssetBundles[assetBundle].LoadAsset<T>(assetName);
            return result;
        }

        public static T GetAsset<T>(in string assetName, in string assetBundleFileName, bool fixMaterials = true) where T : UnityEngine.Object
        {
            if (!LoadAssetBundleIfNotLoaded(assetBundleFileName, fixMaterials))
                return null;

            T result = m_LoadedAssetBundles[assetBundleFileName].LoadAsset<T>(assetName);
            return result;
        }

        public static GameObject GetAsset(in string assetName, in OverhaulAssetPart assetBundlePart, bool fixMaterials = true) => GetAsset<GameObject>(assetName, assetBundlePart, fixMaterials);
        public static GameObject GetAsset(in string assetName, in string assetBundlePart, bool fixMaterials = true) => GetAsset<GameObject>(assetName, assetBundlePart, fixMaterials);

        public static bool TryGetAsset<T>(in string assetName, in string assetBundle, out T asset, bool fixMaterials = true) where T : UnityEngine.Object
        {
            try
            {
                asset = GetAsset<T>(assetName, assetBundle, fixMaterials);
                return asset;
            }
            catch
            {
                asset = null;
            }
            return false;
        }

        public static UnityEngine.Object[] GetAllObjects(in string assetBundleName)
        {
            if (!HasLoadedAssetBundle(assetBundleName))
                return Array.Empty<UnityEngine.Object>();

            AssetBundle bundle = m_LoadedAssetBundles[assetBundleName];
            return bundle.LoadAllAssets();
        }

        public static T[] GetAllObjects<T>(in string assetBundleName) where T : UnityEngine.Object
        {
            if (!HasLoadedAssetBundle(assetBundleName))
                return Array.Empty<T>();

            AssetBundle bundle = m_LoadedAssetBundles[assetBundleName];
            return bundle.LoadAllAssets<T>();
        }

        public static AssetLoadHandler GetAssetAsync(in string assetBundle, in string assetName, Action<AssetLoadHandler> doneCallback)
        {
            if (doneCallback == null || IsLoadingAssetBundle(assetBundle) || !HasLoadedAssetBundle(assetBundle))
                return null;

            AssetBundle bundle = m_LoadedAssetBundles[assetBundle];
            AssetLoadHandler handler = new AssetLoadHandler(doneCallback);
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

            public AssetLoadHandler(Action<AssetLoadHandler> onDone)
            {
                DoneAction = onDone;
            }

            protected override void OnDisposed()
            {
                DoneAction = null;
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
            public static readonly Dictionary<string, AssetBundleLoadHandler> LoadingBundles = new Dictionary<string, AssetBundleLoadHandler>();

            public AsyncOperation Request;
            public Action<AssetBundleLoadHandler> DoneAction;

            public string AssetBundlePath;

            public AssetBundleLoadHandler(Action<AssetBundleLoadHandler> onDone, string bundleName)
            {
                DoneAction = onDone;
                AssetBundlePath = bundleName;
                LoadingBundles.Add(bundleName, this);
            }

            protected override void OnDisposed()
            {
                if (LoadingBundles.ContainsKey(AssetBundlePath))
                    _ = LoadingBundles.Remove(AssetBundlePath);

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
