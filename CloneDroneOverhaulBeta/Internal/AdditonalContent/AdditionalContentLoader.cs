using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul
{
    public static class AdditionalContentLoader
    {
        private static string s_LoadedContent = string.Empty;

        public static void LoadAllContent()
        {
            AdditionalContentController controller = OverhaulController.GetController<AdditionalContentController>();
            if (!controller)
            {
                throw new NullReferenceException("AdditionalContentController is NULL!");
            }

            string[] zippedFiles = controller.GetZipFiles();
            if (!zippedFiles.IsNullOrEmpty())
            {
                unZipFiles(zippedFiles, controller);
            }

            string[] directories = controller.GetContentDirectories();
            if (directories.IsNullOrEmpty())
                return;

            foreach(string directory in directories)
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

            string assetsToLoadPath = directoryPath + "/AssetsToLoad.txt";
            if (!File.Exists(assetsToLoadPath))
            {
                throw new Exception(string.Format("[AdditionalContentLoader] File AssetsToLoad.txt not found in {0} directory! Delete the directory!", directoryPath));
            }
            string assetsToLoadContent = OverhaulCore.ReadText(assetsToLoadPath);
            string[] assetFiles = assetsToLoadContent.Split(',');
            foreach(string assetFile in assetFiles)
            {
                string assetFilePath = "Content/" + directoryInfo.Name + "/" + assetFile;
                Debug.Log(assetFilePath);
                if (!File.Exists(OverhaulMod.Core.ModDirectory + assetFilePath))
                    continue;

                OverhaulAssetsController.LoadAssetBundleIfNotLoaded(assetFilePath);
            }

            s_LoadedContent += uniqueIdFileContent + ", ";
        }

        public static bool HasLoadedContent(string uniqueId) => s_LoadedContent.Contains(uniqueId);

        private static void unZipFiles(string[] files, AdditionalContentController controller)
        {
            int index = 0;
            do
            {
                string path = files[index];
                OverhaulCore.UnZipFile(path, controller.GetContentDirectory());
                index++;
            } while (index < files.Length);
        }
    }
}
