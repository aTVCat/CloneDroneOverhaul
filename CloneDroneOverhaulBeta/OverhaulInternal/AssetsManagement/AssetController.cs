using CDOverhaul.Enumerators;
using ModLibrary;
using UnityEngine;

namespace CDOverhaul
{
    public static class AssetController
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

        /// <summary>
        /// Get an asset from bundle
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="assetBundlePart"></param>
        /// <returns></returns>
        public static GameObject GetAsset(in string assetName, in ModAssetBundlePart assetBundlePart)
        {
            GameObject result = GetAsset<GameObject>(assetName, assetBundlePart);
            return result;
        }

        public static T GetAsset<T>(in string assetName, in ModAssetBundlePart assetBundlePart) where T : UnityEngine.Object
        {
            T result = null;
            switch (assetBundlePart)
            {
                case ModAssetBundlePart.Part1:
                    result = AssetLoader.GetObjectFromFile<T>(ModAssetBundle_Part1, assetName);
                    break;
                case ModAssetBundlePart.Part2:
                    result = AssetLoader.GetObjectFromFile<T>(ModAssetBundle_Part2, assetName);
                    break;
                case ModAssetBundlePart.WeaponSkins:
                    result = AssetLoader.GetObjectFromFile<T>(ModAssetBundle_Skins, assetName);
                    break;
                case ModAssetBundlePart.Objects:
                    result = AssetLoader.GetObjectFromFile<T>(ModAssetBundle_Objects, assetName);
                    break;
                case ModAssetBundlePart.Accessories:
                    result = AssetLoader.GetObjectFromFile<T>(ModAssetBundle_Accessouries, assetName);
                    break;
                case ModAssetBundlePart.Sounds:
                    result = AssetLoader.GetObjectFromFile<T>(ModAssetBundle_Sounds, assetName);
                    break;
                case ModAssetBundlePart.Main:
                    result = AssetLoader.GetObjectFromFile<T>(ModAssetBundle_Main, assetName);
                    break;
                case ModAssetBundlePart.Arena_Update:
                    result = AssetLoader.GetObjectFromFile<T>(ModAssetBundle_ArenaUpdate, assetName);
                    break;
            }
            return result;
        }

        /// <summary>
        /// Use for testing purposes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <param name="assetBundlePart"></param>
        public static void PreloadAsset<T>(in string assetName, in ModAssetBundlePart assetBundlePart) where T : UnityEngine.Object
        {
            _ = GetAsset<T>(assetName, assetBundlePart);
        }
    }
}
