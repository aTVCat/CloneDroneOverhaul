using CloneDroneOverhaul.V3Tests.Utilities;
using System.IO;
using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Base
{
    public class ModDataController : V3_ModControllerBase
    {
        private static bool _hasCreatedFolders { get; set; }
        private static int _tempFileID { get; set; }

        /// <summary>
        /// User data folder
        /// </summary>
        public static readonly string ModDataFolderPath = Application.persistentDataPath + "/OverhaulMod/";

        /// <summary>
        /// The folder, where most of assets are stored in
        /// </summary>
        public static string TemporarySavedFilesFolder => ModDataFolderPath + "Temp/";

        /// <summary>
        /// The folder, where most of assets are stored in
        /// </summary>
        public static string ModAssetsFolder => OverhaulDescription.GetModFolder() + "Assets/";

        internal static void Initialize()
        {
            string[] folders = new string[]
            {
                ModDataFolderPath,
                TemporarySavedFilesFolder
            };
            CreateFoldersIfRequired(folders);

            _hasCreatedFolders = true;
        }

        /// <summary>
        /// Load an asset from Assets directory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileUnderAssetsFolder"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static T LoadModAsset<T>(string fileUnderAssetsFolder, in ModAssetFileType fileType) where T : class
        {
            fileUnderAssetsFolder = ModAssetsFolder + fileUnderAssetsFolder;

            object result = null;

            if (fileType == ModAssetFileType.Image)
            {
                result = OverhaulUtilities.TextureAndMaterialUtils.LoadTexture(fileUnderAssetsFolder);
            }
            else if (fileType == ModAssetFileType.Json)
            {
                result = OverhaulUtilities.FileUtils.LoadString(fileUnderAssetsFolder);
            }
            else if (fileType == ModAssetFileType.None)
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
        public static void LoadModAssetAsync<T>(string fileUnderAssetsFolder, in ModAssetFileType fileType, in System.Action<T> onLoaded)
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

        /// <summary>
        /// Async load asset from temp files directory
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="fileType"></param>
        public static void LoadTempAssetAsync<T>(string filename, in ModAssetFileType fileType, System.Action<T> onLoaded)
        {
            filename = TemporarySavedFilesFolder + filename;

            object result = null;

            if (fileType == ModAssetFileType.Image)
            {
                OverhaulUtilities.TextureAndMaterialUtils.LoadTextureAsync(filename, delegate (Texture2D t)
                {
                    result = t;
                });
            }
            else if (fileType == ModAssetFileType.Json)
            {
                OverhaulUtilities.FileUtils.LoadStringAsync(filename, delegate (string t)
                {
                    result = t;
                });
            }
            else if (fileType == ModAssetFileType.None)
            {
                OverhaulUtilities.FileUtils.LoadBytesAsync(filename, delegate (byte[] t)
                {
                    result = t;
                });
            }

            onLoaded((T)result);
        }

        /// <summary>
        /// Save bytes in temp folder
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>The path of saved asset</returns>
        public static string SaveTempAssetOnDisk(in byte[] bytes)
        {
            _tempFileID++;
            OverhaulUtilities.FileUtils.SaveBytes(bytes, ModDataController.TemporarySavedFilesFolder, _tempFileID.ToString());
            return ModDataController.TemporarySavedFilesFolder + _tempFileID.ToString();
        }

        /// <summary>
        /// Save bytes in temp folder
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>The path of saved asset</returns>
        public static string SaveTempAssetOnDisk(in byte[] bytes, in string customName)
        {
            OverhaulUtilities.FileUtils.SaveBytes(bytes, ModDataController.TemporarySavedFilesFolder, customName);
            return ModDataController.TemporarySavedFilesFolder + customName;
        }

        public static bool HasTempAsset(in string fileName)
        {
            return OverhaulUtilities.FileUtils.FileExists(ModDataController.TemporarySavedFilesFolder + fileName);
        }

        /// <summary>
        /// Creates a folder if one doesn't exist
        /// </summary>
        /// <param name="path"></param>
        public static void CreateFolderIfRequired(in string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Same as method above, but for multiple folders
        /// </summary>
        /// <param name="paths"></param>
        public static void CreateFoldersIfRequired(in string[] paths)
        {
            foreach (string path in paths)
            {
                CreateFolderIfRequired(path);
            }
        }
    }
}