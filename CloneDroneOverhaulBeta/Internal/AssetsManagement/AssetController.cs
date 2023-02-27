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

        public const string ModAssetBundle_CombatUpdate = "overhaulassets_combatupdatestuff";

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
            }
            T result = AssetLoader.GetObjectFromFile<T>(assetBundle, assetName);
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
    }
}
