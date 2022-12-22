using UnityEngine;
using CloneDroneOverhaul.V3Tests.Utilities;

namespace CloneDroneOverhaul.V3Tests.Base
{
    public class ModDataController : V3_ModControllerBase
    {
        /// <summary>
        /// User data folder
        /// </summary>
        public static readonly string ModDataFolderPath = Application.persistentDataPath + "/OverhaulMod/";

        /// <summary>
        /// The folder, where most of assets are stored in
        /// </summary>
        public static string ModAssetsFolder { get { return OverhaulDescription.GetModFolder() + "Assets/"; } }

        /// <summary>
        /// Load an asset from Assets directory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileUnderAssetsFolder"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public T LoadModAsset<T>(string fileUnderAssetsFolder, in ModAssetFileType fileType) where T : class
        {
            fileUnderAssetsFolder = ModAssetsFolder + fileUnderAssetsFolder;

            object result = null;

            if(fileType == ModAssetFileType.Image)
            {
                result = OverhaulUtilities.TextureAndMaterialUtils.LoadTexture(fileUnderAssetsFolder);
            }
            else if(fileType == ModAssetFileType.Json)
            {
                result = OverhaulUtilities.FileUtils.LoadString(fileUnderAssetsFolder);
            }
            else if(fileType == ModAssetFileType.None)
            {
                result = OverhaulUtilities.FileUtils.LoadBytes(fileUnderAssetsFolder);
            }

            return (T)result;
        }

        /// <summary>
        /// Async load asset from Assets directory
        /// </summary>
        /// <param name="fileUnderAssetsFolder"></param>
        /// <param name="fileType"></param>
        public void LoadModAssetAsync<T>(string fileUnderAssetsFolder, in ModAssetFileType fileType, in System.Action<T> onLoaded)
        {
            fileUnderAssetsFolder = ModAssetsFolder + fileUnderAssetsFolder;

            object result = null;

            if (fileType == ModAssetFileType.Image)
            {
                OverhaulUtilities.TextureAndMaterialUtils.LoadTextureAsync(fileUnderAssetsFolder, delegate (Texture2D t)
                {
                    result = t;
                });
            }
            else if (fileType == ModAssetFileType.Json)
            {
                OverhaulUtilities.FileUtils.LoadStringAsync(fileUnderAssetsFolder, delegate (string t)
                {
                    result = t;
                });
            }
            else if (fileType == ModAssetFileType.None)
            {
                OverhaulUtilities.FileUtils.LoadBytesAsync(fileUnderAssetsFolder, delegate (byte[] t)
                {
                    result = t;
                });
            }

            onLoaded((T)result);
        }
    }
}