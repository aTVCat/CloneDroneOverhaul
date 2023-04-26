using ModLibrary;
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

        #region Manually controller asset bundles

        public static bool LoadingAssetBundle => m_CurrentCreateRequest != null;
        public static float LoadingProgress => m_CurrentCreateRequest == null ? 0f : m_CurrentCreateRequest.progress;
        private static AssetBundleCreateRequest m_CurrentCreateRequest;

        private static readonly Dictionary<string, AssetBundle> m_LoadedAssetBundles = new Dictionary<string, AssetBundle>();

        public static void TryLoadAssetBundle(in string pathUnderModFolder)
        {
            try
            {
                if (!checkPath(pathUnderModFolder, out string assetBundleFilename, out string assetBundlePath))
                {
                    return;
                }

                AssetBundle loadedAssetBundle = AssetBundle.LoadFromFile(assetBundlePath);
                m_LoadedAssetBundles.Add(assetBundleFilename, loadedAssetBundle);
            }
            catch
            {
                // todo: catch
            }
        }

        public static void LoadAssetBundleAsync(in string pathUnderModFolder, Action<AssetBundle> onComplete)
        {
            if (LoadingAssetBundle || !checkPath(pathUnderModFolder, out string assetBundleFilename, out string assetBundlePath))
            {
                return;
            }

            StaticCoroutineRunner.StartStaticCoroutine(loadAssetBundleCoroutine(assetBundlePath, assetBundleFilename, onComplete));
        }

        private static IEnumerator loadAssetBundleCoroutine(string path, string fileName, Action<AssetBundle> onComplete)
        {
            m_CurrentCreateRequest = AssetBundle.LoadFromFileAsync(path);

            yield return m_CurrentCreateRequest;
            while (!m_CurrentCreateRequest.isDone)
            {
                yield return null;
            }

            AssetBundle loadedAssetBundle = m_CurrentCreateRequest.assetBundle;
            m_LoadedAssetBundles.Add(fileName, loadedAssetBundle);
            onComplete?.Invoke(loadedAssetBundle);
            m_CurrentCreateRequest = null;
            yield break;
        }

        public static void TryUnloadAssetBundle(in string pathUnderModFolder, in bool unloadObjects = false)
        {
            try
            {
                string filename = getAssetBundleFilename(pathUnderModFolder);
                if (!HasLoadedAssetBundle(filename))
                {
                    return;
                }

                AssetBundle bundleToUnload = m_LoadedAssetBundles[filename];
                bundleToUnload.Unload(unloadObjects);
                m_LoadedAssetBundles.Remove(filename);
            }
            catch
            {
                // todo: catch
            }
        }

        public static bool HasLoadedAssetBundle(in string assetBundleName)
        {
            return m_LoadedAssetBundles.ContainsKey(assetBundleName);
        }

        private static bool checkPath(in string pathUnderModFolder, out string assetBundleFilename, out string assetBundlePath)
        {
            if (string.IsNullOrEmpty(pathUnderModFolder))
            {
                assetBundleFilename = string.Empty;
                assetBundlePath = string.Empty;
                return false;
            }

            assetBundlePath = OverhaulMod.Core.ModDirectory + pathUnderModFolder;
            assetBundleFilename = getAssetBundleFilename(assetBundlePath);
            return !HasLoadedAssetBundle(assetBundleFilename) && File.Exists(assetBundlePath);
        }

        private static string getAssetBundleFilename(in string path)
        {
            return !string.IsNullOrEmpty(path) && !path.Contains("/") && !path.Contains("\\") ? path : path.Substring(path.LastIndexOf('/') + 1);
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

        public static bool TryGetAsset<T>(in string assetName, in OverhaulAssetsPart assetBundlePart, out T asset) where T : UnityEngine.Object
        {
            try
            {
                asset = GetAsset<T>(assetName, assetBundlePart);
                return true;
            }
            catch
            {
                asset = null;
                return false;
            }

            return false;
        }

        public static T GetAsset<T>(in string assetName, in OverhaulAssetsPart assetBundlePart) where T : UnityEngine.Object
        {
            string assetBundle = null;
            string m = GetManuallyControlledAssetBundleFileName(assetBundlePart);
            bool isManuallyControlled = IsManuallyControlledAssetBundle(assetBundlePart);
            if (isManuallyControlled && !HasLoadedAssetBundle(m))
            {
                TryLoadAssetBundle(m);
            }

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
            T result = null;
            if (!isManuallyControlled)
            {
                result = AssetLoader.GetObjectFromFile<T>(assetBundle, assetName);
            }
            else
            {
                result = m_LoadedAssetBundles[m].LoadAsset<T>(assetName);
            }
            return result;
        }

        public static T GetAsset<T>(in string assetName, in string assetBundleFileName) where T : UnityEngine.Object
        {
            bool isManuallyControlled = IsManuallyControlledAssetBundle(assetBundleFileName);
            if (isManuallyControlled && !HasLoadedAssetBundle(assetBundleFileName))
            {
                TryLoadAssetBundle(assetBundleFileName);
            }

            T result = null;
            if (!isManuallyControlled)
            {
                result = AssetLoader.GetObjectFromFile<T>(assetBundleFileName, assetName);
            }
            else
            {
                result = m_LoadedAssetBundles[assetBundleFileName].LoadAsset<T>(assetName);
            }
            return result;
        }

        /// <summary>
        /// Use for testing purposes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <param name="assetBundlePart"></param>
        public static void PreloadAsset<T>(in string assetName, in OverhaulAssetsPart assetBundlePart) where T : UnityEngine.Object
        {
            _ = GetAsset<T>(assetName, assetBundlePart);
        }

        public static void PreloadAsset<T>(in string assetName, in string assetBundleFileName) where T : UnityEngine.Object
        {
            _ = GetAsset<T>(assetName, assetBundleFileName);
        }

        /// <summary>
        /// Check if user has specific asset bundle (or even file) on disk
        /// </summary>
        /// <param name="assetBundleFileName"></param>
        /// <returns></returns>
        public static bool HasAssetBundle(in string assetBundleFileName)
        {
            string path = OverhaulMod.Core.ModDirectory + assetBundleFileName;
            return File.Exists(path);
        }

        public static bool IsManuallyControlledAssetBundle(in OverhaulAssetsPart assetBundlePart)
        {
            switch (assetBundlePart)
            {
                case OverhaulAssetsPart.Skyboxes:
                    return true;
                case OverhaulAssetsPart.WeaponSkins:
                    return true;
            }
            return false;
        }

        public static bool IsManuallyControlledAssetBundle(in string assetBundlePart)
        {
            switch (assetBundlePart)
            {
                case ModAssetBundle_Skyboxes:
                    return true;
                case ModAssetBundle_Skins:
                    return true;
            }
            return false;
        }

        public static string GetManuallyControlledAssetBundleFileName(in OverhaulAssetsPart assetBundlePart)
        {
            switch (assetBundlePart)
            {
                case OverhaulAssetsPart.Skyboxes:
                    return ModAssetBundle_Skyboxes;
                case OverhaulAssetsPart.WeaponSkins:
                    return ModAssetBundle_Skins;
            }
            return string.Empty;
        }
    }
}
