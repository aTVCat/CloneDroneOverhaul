using OverhaulMod.Utils;
using System.IO;
using UnityEngine;

namespace OverhaulMod
{
    public static class ModDirectories
    {
        public const string CUSTOMIZATION_FOLDER_NAME = "customization";

        public const string CUSTOMIZATION_PERSISTENT_FOLDER_NAME = "customizationPersistent";

        public static void CreateMissingDirectories()
        {
            _ = ModFileUtils.CreateDirectoryIfRequired(ModUserDataFolder);
            _ = ModFileUtils.CreateDirectoryIfRequired(ContentFolder);
            _ = ModFileUtils.CreateDirectoryIfRequired(SavesFolder);
            _ = ModFileUtils.CreateDirectoryIfRequired(AddonsFolder);
            _ = ModFileUtils.CreateDirectoryIfRequired(CustomizationFolder);
            _ = ModFileUtils.CreateDirectoryIfRequired(CustomizationPersistentFolder);
            _ = ModFileUtils.CreateDirectoryIfRequired(DeveloperFolder);
        }

        private static string s_modFolder;
        public static string ModFolder
        {
            get
            {
                ModCore modCore = ModCore.Instance;
                if (modCore == null)
                {
                    return null;
                }

                if (s_modFolder == null)
                {
                    s_modFolder = modCore.ModInfo.FolderPath;
                }
                return s_modFolder;
            }
        }

        private static string s_savesFolder;
        public static string SavesFolder
        {
            get
            {
                if (s_savesFolder == null)
                {
                    s_savesFolder = $"{Path.Combine(ModUserDataFolder, "saves")}/";
                }
                return s_savesFolder;
            }
        }

        private static string s_assetsFolder;
        public static string AssetsFolder
        {
            get
            {
                if (s_assetsFolder == null)
                {
                    s_assetsFolder = $"{Path.Combine(ModFolder, "assets")}/";
                }
                return s_assetsFolder;
            }
        }

        private static string s_dataFolder;
        public static string DataFolder
        {
            get
            {
                if (s_dataFolder == null)
                {
                    s_dataFolder = $"{Path.Combine(AssetsFolder, "data")}/";
                }
                return s_dataFolder;
            }
        }

        private static string s_modUserDataFolder;
        public static string ModUserDataFolder
        {
            get
            {
                if (s_modUserDataFolder == null)
                {
                    s_modUserDataFolder = $"{Path.Combine(Application.persistentDataPath, "OverhaulMod")}/";
                }
                return s_modUserDataFolder;
            }
        }

        private static string s_developerFolder;
        public static string DeveloperFolder
        {
            get
            {
                if (s_developerFolder == null)
                {
                    s_developerFolder = $"{Path.Combine(ModUserDataFolder, "devFolder")}/";
                }
                return s_developerFolder;
            }
        }

        private static string s_contentFolder;
        public static string ContentFolder
        {
            get
            {
                if (s_contentFolder == null)
                {
                    s_contentFolder = $"{Path.Combine(ModUserDataFolder, "content")}/";
                }
                return s_contentFolder;
            }
        }

        private static string s_addonsFolder;
        public static string AddonsFolder
        {
            get
            {
                if (s_addonsFolder == null)
                {
                    s_addonsFolder = $"{Path.Combine(ContentFolder, "addons")}/";
                }
                return s_addonsFolder;
            }
        }

        private static string s_customizationFolder;
        public static string CustomizationFolder
        {
            get
            {
                if (s_customizationFolder == null)
                {
                    s_customizationFolder = $"{Path.Combine(ContentFolder, CUSTOMIZATION_FOLDER_NAME)}/";
                }
                return s_customizationFolder;
            }
        }

        private static string s_persistentCustomizationFolder;
        public static string CustomizationPersistentFolder
        {
            get
            {
                if (s_persistentCustomizationFolder == null)
                {
                    s_persistentCustomizationFolder = $"{Path.Combine(ContentFolder, CUSTOMIZATION_PERSISTENT_FOLDER_NAME)}/";
                }
                return s_persistentCustomizationFolder;
            }
        }

        private static string s_textureFolder;
        public static string TexturesFolder
        {
            get
            {
                if (s_textureFolder == null)
                {
                    s_textureFolder = $"{Path.Combine(AssetsFolder, "textures")}/";
                }
                return s_textureFolder;
            }
        }
    }
}