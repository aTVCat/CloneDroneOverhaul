using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CDOverhaul
{
    public static class AssetsController
    {
        /// <summary>
        /// The asset bundle is split in several parts. This is the file name of first part
        /// </summary>
        public const string ModAssetBundle_Part1 = "overhaulassets_p1";

        public const string ModAssetBundle_Part2 = "overhaulassets_p2";

        public const string ModAssetBundle_Skins = "overhaulassets_skins";

        public const string ModAssetBundle_Objects = "overhaulassets_objects";

        public const string ModAssetBundle_Accessouries = "overhaulassets_accessories";

        public const string ModAssetBundle_Sounds = "overhaulassets_sounds";

        public const string ModAssetBundle_Main = "overhaulassets_main";

        public const string ModAssetBundle_ArenaUpdate = "overhaulassets_arenaupdate";

        public const string ModAssetBundle_CombatUpdate = "overhaulassets_combatupdatestuff";

        public const string ModAssetBundle_Skyboxes = "overhaulassets_skyboxes";

        #region Manually controlled asset bundles

        public static AssetBundleCreateRequest CurrentAssetBundleCreateRequest;
        public static bool IsLoadingAssetBundle => CurrentAssetBundleCreateRequest != null;
        public static float AssetBundleLoadingProgress => IsLoadingAssetBundle ? CurrentAssetBundleCreateRequest.progress : 0f;


        private static readonly Dictionary<string, AssetBundle> m_LoadedAssetBundles = new Dictionary<string, AssetBundle>();

        public static void TryLoadAssetBundle(in string pathUnderModFolder)
        {
            if (HasLoadedAssetBundle(pathUnderModFolder))
            {
                return;
            }

            string path = OverhaulMod.Core.ModDirectory + pathUnderModFolder;
            try
            {
                if (!checkPath(path))
                {
                    return;
                }

                AssetBundle loadedAssetBundle = AssetBundle.LoadFromFile(path);
                m_LoadedAssetBundles.Add(pathUnderModFolder, loadedAssetBundle);
            }
            catch
            {
                // todo: catch
            }
        }

        public static void LoadAssetBundleAsync(in string path, Action<AssetBundle> onComplete)
        {
            if (IsLoadingAssetBundle || !checkPath(path))
            {
                return;
            }

            StaticCoroutineRunner.StartStaticCoroutine(loadAssetBundleCoroutine(path, onComplete));
        }
        private static IEnumerator loadAssetBundleCoroutine(string path, Action<AssetBundle> onComplete)
        {
            CurrentAssetBundleCreateRequest = AssetBundle.LoadFromFileAsync(path);

            yield return CurrentAssetBundleCreateRequest;
            while (!CurrentAssetBundleCreateRequest.isDone)
            {
                yield return null;
            }

            AssetBundle loadedAssetBundle = CurrentAssetBundleCreateRequest.assetBundle;
            m_LoadedAssetBundles.Add(path, loadedAssetBundle);
            onComplete?.Invoke(loadedAssetBundle);
            CurrentAssetBundleCreateRequest = null;
            yield break;
        }

        public static bool HasLoadedAssetBundle(in string assetBundleName)
        {
            return m_LoadedAssetBundles.ContainsKey(assetBundleName);
        }

        private static bool checkPath(in string pathUnderModFolder)
        {
            return !string.IsNullOrEmpty(pathUnderModFolder) && File.Exists(pathUnderModFolder);
        }

        #endregion

        /// <summary>
        /// Get an asset from bundle
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="assetBundlePart"></param>
        /// <returns></returns>
        public static GameObject GetAsset(in string assetName, in OverhaulAssetsPart assetBundlePart)
        {
            GameObject result = GetAsset<GameObject>(assetName, assetBundlePart);
            return result;
        }

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

        public static T GetAsset<T>(in string assetName, in OverhaulAssetsPart assetBundlePart) where T : UnityEngine.Object
        {
            string assetBundle = null;

            switch (assetBundlePart)
            {
                case OverhaulAssetsPart.Part1:
                    assetBundle = ModAssetBundle_Part1;
                    break;
                case OverhaulAssetsPart.Part2:
                    assetBundle = ModAssetBundle_Part2;
                    break;
                case OverhaulAssetsPart.WeaponSkins:
                    assetBundle = ModAssetBundle_Skins;
                    break;
                case OverhaulAssetsPart.Objects:
                    assetBundle = ModAssetBundle_Objects;
                    break;
                case OverhaulAssetsPart.Accessories:
                    assetBundle = ModAssetBundle_Accessouries;
                    break;
                case OverhaulAssetsPart.Sounds:
                    assetBundle = ModAssetBundle_Sounds;
                    break;
                case OverhaulAssetsPart.Main:
                    assetBundle = ModAssetBundle_Main;
                    break;
                case OverhaulAssetsPart.Arena_Update:
                    assetBundle = ModAssetBundle_ArenaUpdate;
                    break;
                case OverhaulAssetsPart.Combat_Update:
                    assetBundle = ModAssetBundle_CombatUpdate;
                    break;
                case OverhaulAssetsPart.Skyboxes:
                    assetBundle = ModAssetBundle_Skyboxes;
                    break;
            }

            if (!HasLoadedAssetBundle(assetBundle))
            {
                TryLoadAssetBundle(assetBundle);
            }

            T result = m_LoadedAssetBundles[assetBundle].LoadAsset<T>(assetName);
            return result;
        }

        public static T GetAsset<T>(in string assetName, in string assetBundleFileName) where T : UnityEngine.Object
        {
            if (!HasLoadedAssetBundle(assetBundleFileName))
            {
                TryLoadAssetBundle(assetBundleFileName);
            }

            T result = m_LoadedAssetBundles[assetBundleFileName].LoadAsset<T>(assetName);
            return result;
        }

        public static void PreloadAsset<T>(in string assetName, in string assetBundleFileName) where T : UnityEngine.Object
        {
            _ = GetAsset<T>(assetName, assetBundleFileName);
        }

        public static void TryUnloadAssetBundle(in string pathUnderModFolder, in bool unloadObjects = false)
        {
            try
            {
                if (!HasLoadedAssetBundle(pathUnderModFolder))
                {
                    return;
                }

                AssetBundle bundleToUnload = m_LoadedAssetBundles[pathUnderModFolder];
                bundleToUnload.Unload(unloadObjects);
                m_LoadedAssetBundles.Remove(pathUnderModFolder);
            }
            catch
            {
                // todo: catch
            }
        }

        /// <summary>
        /// Check if user has specific asset bundle (or even file) on disk
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <returns></returns>
        public static bool HasAssetBundle(in string assetBundlePath)
        {
            string path = OverhaulMod.Core.ModDirectory + assetBundlePath;
            return File.Exists(path);
        }
    }
}
