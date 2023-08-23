using System;
using System.IO;
using UnityEngine;

namespace CDOverhaul
{
    public static class AdditionalContentLoader
    {
        private static string s_LoadedContent = string.Empty;

        public static void LoadAllContent()
        {
            AdditionalContentController controller = OverhaulController.Get<AdditionalContentController>();
            if (!controller)
            {
                throw new NullReferenceException("AdditionalContentController is NULL!");
            }

            string[] zippedFiles = controller.GetZipFiles();
            if (!zippedFiles.IsNullOrEmpty())
            {
                unZipFiles(zippedFiles, true, controller);
            }

            string[] directories = controller.GetContentDirectories();
            if (directories.IsNullOrEmpty())
                return;

            foreach (string directory in directories)
            {
                LoadContent(directory);
            }
        }

        public static void LoadContent(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
                return;

            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

            string uniqueIdFilePath = directoryPath + "/UniqueID.txt";
            if (!File.Exists(uniqueIdFilePath))
            {
                throw new Exception(string.Format("[AdditionalContentLoader] File UniqueID.txt not found in {0} directory! Delete the directory!", directoryPath));
            }
            string uniqueIdFileContent = OverhaulCore.ReadText(uniqueIdFilePath);

            if (s_LoadedContent.Contains(uniqueIdFileContent))
                return;

            string assetsToLoadPath = directoryPath + "/AssetsToLoad.txt";
            if (!File.Exists(assetsToLoadPath))
            {
                throw new Exception(string.Format("[AdditionalContentLoader] File AssetsToLoad.txt not found in {0} directory! Delete the directory!", directoryPath));
            }
            string assetsToLoadContent = OverhaulCore.ReadText(assetsToLoadPath);
            string[] assetFiles = assetsToLoadContent.Split(',');
            foreach (string assetFile in assetFiles)
            {
                string assetFilePath = "Content/" + directoryInfo.Name + "/" + assetFile;
                Debug.Log(assetFilePath);
                if (!File.Exists(OverhaulMod.Core.ModDirectory + assetFilePath))
                    continue;

                _ = OverhaulAssetsController.LoadAssetBundleIfNotLoaded(assetFilePath);
            }

            s_LoadedContent += uniqueIdFileContent + ", ";
        }

        public static bool HasLoadedContent(string uniqueId) => s_LoadedContent.Contains(uniqueId);

        private static void unZipFiles(string[] files, bool deleteInitialFiles, AdditionalContentController controller)
        {
            int index = 0;
            do
            {
                string path = files[index];
                _ = OverhaulCore.UnZipFile(path, controller.GetContentDirectory() + path.Substring(path.LastIndexOf('/') + 1).Replace(".zip", string.Empty));
                if (deleteInitialFiles) File.Delete(path);
                index++;
            } while (index < files.Length);
        }
    }
}
