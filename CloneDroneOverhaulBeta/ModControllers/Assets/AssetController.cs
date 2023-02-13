﻿using CDOverhaul.Enumerators;
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

        /// <summary>
        /// Get an asset from bundle
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="assetBundlePart"></param>
        /// <returns></returns>
        public static GameObject GetAsset(in string assetName, in EModAssetBundlePart assetBundlePart)
        {
            GameObject result = null;
            result = GetAsset<GameObject>(assetName, assetBundlePart);
            return result;
        }

        public static T GetAsset<T>(in string assetName, in EModAssetBundlePart assetBundlePart) where T : UnityEngine.Object
        {
            T result = null;
            switch (assetBundlePart)
            {
                case EModAssetBundlePart.Part1:
                    result = AssetLoader.GetObjectFromFile<T>(ModAssetBundle_Part1, assetName);
                    break;
                case EModAssetBundlePart.Part2:
                    result = AssetLoader.GetObjectFromFile<T>(ModAssetBundle_Part2, assetName);
                    break;
                case EModAssetBundlePart.WeaponSkins:
                    result = AssetLoader.GetObjectFromFile<T>(ModAssetBundle_Skins, assetName);
                    break;
                case EModAssetBundlePart.Objects:
                    result = AssetLoader.GetObjectFromFile<T>(ModAssetBundle_Objects, assetName);
                    break;
                case EModAssetBundlePart.Accessories:
                    result = AssetLoader.GetObjectFromFile<T>(ModAssetBundle_Accessouries, assetName);
                    break;
                case EModAssetBundlePart.Sounds:
                    result = AssetLoader.GetObjectFromFile<T>(ModAssetBundle_Sounds, assetName);
                    break;
            }
            return result;
        }

        public static void PreloadAsset<T>(in string assetName, in EModAssetBundlePart assetBundlePart) where T : UnityEngine.Object
        {
            GetAsset<T>(assetName, assetBundlePart);
        }
    }
}
