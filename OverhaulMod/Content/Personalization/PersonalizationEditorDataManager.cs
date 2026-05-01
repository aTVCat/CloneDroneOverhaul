using ICSharpCode.SharpZipLib.Zip;
using OverhaulMod.Engine;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorDataManager : Singleton<PersonalizationEditorDataManager>
    {
        public const int IMPORT_VERSION = 1;

        public const string ITEMS_ARCHIVE_FILE = "CustomizationAssets.zip";

        public const string ITEMS_ARCHIVE_FILE_OLD = "customization.zip";

        public const string ITEM_INFO_FILE = "itemInfo.json";

        public const string ITEM_META_DATA_FILE = "metaData.json";

        public const string ITEM_OBJECTS_FILE = "objects.json";

        public const string ITEM_ACCESSORY_OFFSETS_FILE = "accessoryOffsets.json";

        public PersonalizationItemCreationResult CreateItem(PersonalizationItemCreationArgs args)
        {
            string rootDirectory = args.UsePersistentFolder ? ModDirectories.CustomizationPersistentFolder : ModDirectories.CustomizationFolder;
            string directoryPath = Path.Combine(rootDirectory, args.DirectoryName);
            string filesDirectoryPath = Path.Combine(directoryPath, "files");

            PersonalizationItemInfo createdItemInfo = null;
            if (Directory.Exists(directoryPath))
                return new PersonalizationItemCreationResult("Item with the same folder name is already created");

            _ = Directory.CreateDirectory(directoryPath);
            _ = Directory.CreateDirectory(filesDirectoryPath);

            bool useGeneratedItemInfo = true;
            if (args.Template != null)
            {
                try
                {
                    createdItemInfo = ModJsonUtils.Deserialize<PersonalizationItemInfo>(ModJsonUtils.Serialize(args.Template)); // create a copy of the template

                    createdItemInfo.Name = name;
                    createdItemInfo.ItemID = args.UniqueID;
                    createdItemInfo.Category = args.ItemCategory;
                    createdItemInfo.Description = "No description provided.";
                    createdItemInfo.EditorID = PersonalizationEditorManager.Instance.EditorID;
                    createdItemInfo.FolderPath = directoryPath;
                    createdItemInfo.RootFolderPath = rootDirectory;
                    createdItemInfo.IsPersistentAsset = args.UsePersistentFolder;
                    createdItemInfo.MetaData = new PersonalizationItemMetaData()
                    {
                        CustomizationSystemVersion = PersonalizationItemMetaData.CurrentCustomizationSystemVersion,
                    };
                }
                catch (Exception)
                {
                    useGeneratedItemInfo = false;
                }
            }
            else
                useGeneratedItemInfo = false;

            if (!useGeneratedItemInfo)
            {
                createdItemInfo = new PersonalizationItemInfo()
                {
                    Name = name,
                    ItemID = args.UniqueID,
                    Category = args.ItemCategory,
                    Description = "No description provided.",
                    EditorID = PersonalizationEditorManager.Instance.EditorID,
                    FolderPath = directoryPath,
                    RootFolderPath = rootDirectory,
                    IsPersistentAsset = args.UsePersistentFolder,
                    MetaData = new PersonalizationItemMetaData()
                    {
                        CustomizationSystemVersion = PersonalizationItemMetaData.CurrentCustomizationSystemVersion,
                    }
                };
            }

            createdItemInfo.FixValues();
            createdItemInfo.SetAuthor(SteamFriends.GetPersonaName());

            bool isAccessory = args.ItemCategory == PersonalizationCategory.Accessories;
            if (isAccessory)
            {
                createdItemInfo.AccessoryOffsets = new AccessoryOffsetsList();
                createdItemInfo.AccessoryOffsets.InitializeList();
            }

            PersonalizationManager.Instance.ItemList.Items.Add(createdItemInfo);

            PersonalizationManager.Instance.UserInfo.SetIsItemUnverified(createdItemInfo, true);
            PersonalizationManager.Instance.SaveUserInfo();

            WriteItemToFiles(createdItemInfo);

            return new PersonalizationItemCreationResult(createdItemInfo);
        }

        public PersonalizationItemSaveResult SaveItem(PersonalizationItemInfo itemInfo)
        {
            string folder = itemInfo.FolderPath;
            if (folder.IsNullOrEmpty()) return new PersonalizationItemSaveResult("Item has no folder assigned!");
            if (!Directory.Exists(folder)) return new PersonalizationItemSaveResult("Item folder was deleted or moved.");

            bool isAccessory = itemInfo.Category == PersonalizationCategory.Accessories;
            if (isAccessory && itemInfo.AccessoryOffsets == null) return new PersonalizationItemSaveResult("No accessory offsets found!");

            PersonalizationItemMetaData personalizationItemMetaData = itemInfo.MetaData;
            if (personalizationItemMetaData == null)
            {
                personalizationItemMetaData = new PersonalizationItemMetaData
                {
                    CustomizationSystemVersion = PersonalizationItemMetaData.CurrentCustomizationSystemVersion
                };
                itemInfo.MetaData = personalizationItemMetaData;
            }

            try
            {
                WriteItemToFiles(itemInfo);
            }
            catch (Exception exc)
            {
                return new PersonalizationItemSaveResult(exc.ToString());
            }
            return new PersonalizationItemSaveResult();
        }

        public void WriteItemToFiles(PersonalizationItemInfo itemInfo)
        {
            string directory = itemInfo.FolderPath;
            ModJsonUtils.WriteStream(Path.Combine(directory, ITEM_INFO_FILE), itemInfo);
            ModJsonUtils.WriteStream(Path.Combine(directory, ITEM_META_DATA_FILE), itemInfo.MetaData);
            if (itemInfo.RootObject != null) ModJsonUtils.WriteStream(Path.Combine(directory, ITEM_OBJECTS_FILE), itemInfo.RootObject);
            if (itemInfo.Category == PersonalizationCategory.WeaponSkins) ModJsonUtils.WriteStream(Path.Combine(directory, ITEM_ACCESSORY_OFFSETS_FILE), itemInfo.AccessoryOffsets);
        }

        public void DeleteItemFolder(string directory)
        {
            Directory.Delete(directory, true);
        }

        public void DeleteItem(PersonalizationItemInfo personalizationItem)
        {
            PersonalizationItemList itemList = PersonalizationManager.Instance.ItemList;
            itemList.Items.Remove(personalizationItem);
            DeleteItemFolder(personalizationItem.FolderPath);
        }

        public PersonalizationItemImportResult ImportItem(string path, bool editItem = false)
        {
            int importVersion = IMPORT_VERSION;

            string folderName = Path.GetFileNameWithoutExtension(path);
            if (folderName.StartsWith("PersonalizationItem_"))
            {
                importVersion = 0;
                folderName = folderName.Replace("PersonalizationItem_", string.Empty).Remove(8);
            }

            return ImportItem(path, folderName, importVersion, true);
        }

        public PersonalizationItemImportResult ImportItem(string path, string itemFolderName, int importVersion, bool editItem)
        {
            string folderPath = Path.Combine(ModDirectories.CustomizationFolder, itemFolderName);

            if (Directory.Exists(folderPath)) return new PersonalizationItemImportResult("Item with the same folder name is already imported");

            _ = Directory.CreateDirectory(folderPath);

            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(path, folderPath, null);

            PersonalizationItemList itemList = PersonalizationManager.Instance.ItemList;
            PersonalizationItemInfo info;
            try
            {
                info = itemList.LoadItemInfo(folderPath);
            }
            catch (Exception exc)
            {
                return new PersonalizationItemImportResult(exc.ToString());
            }

            if (importVersion == 0)
            {
                foreach (PersonalizationEditorObjectInfo child in info.RootObject.Children)
                {
                    if (child.Path == "Volume")
                    {
                        if (child.PropertyValues.TryGetValue(nameof(PersonalizationEditorObjectVolume.volumeSettingPresets), out object obj) && obj is Dictionary<WeaponVariant2, VolumeSettingsPreset> dictionary && !dictionary.IsNullOrEmpty())
                        {
                            foreach (VolumeSettingsPreset value in dictionary.Values)
                            {
                                string voxFilePath = value.VoxFilePath;
                                if (!voxFilePath.IsNullOrEmpty() && !voxFilePath.StartsWith(itemFolderName))
                                {
                                    string sub = voxFilePath.Substring(voxFilePath.IndexOf(Path.DirectorySeparatorChar) + 1);
                                    voxFilePath = $"{itemFolderName}{Path.DirectorySeparatorChar}{sub}";
                                    value.VoxFilePath = voxFilePath;
                                }
                            }
                        }
                    }
                    else if (child.Path == "CvmModel")
                    {
                        if (child.PropertyValues.TryGetValue(nameof(PersonalizationEditorObjectCVMModel.presets), out object obj) && obj is Dictionary<WeaponVariant2, CVMModelPreset> dictionary && !dictionary.IsNullOrEmpty())
                        {
                            foreach (CVMModelPreset value in dictionary.Values)
                            {
                                string cvmFilePath = value.CvmFilePath;
                                if (!cvmFilePath.IsNullOrEmpty() && !cvmFilePath.StartsWith(itemFolderName))
                                {
                                    string sub = cvmFilePath.Substring(cvmFilePath.IndexOf(Path.DirectorySeparatorChar) + 1);
                                    cvmFilePath = $"{itemFolderName}{Path.DirectorySeparatorChar}{sub}";
                                    value.CvmFilePath = cvmFilePath;
                                }
                            }
                        }
                    }
                }
            }

            itemList.Items.Add(info);

            if (editItem)
            {
                UIPersonalizationEditor.Instance.ShowEverything();
                PersonalizationEditorManager.Instance.EditItem(info);
            }

            return new PersonalizationItemImportResult(PersonalizationItemImportResult.ImportResult.Success);
        }

        public bool HasImportedItem(string exportedItemPath, out PersonalizationItemInfo itemInfo)
        {
            string rawItemId = Path.GetFileNameWithoutExtension(exportedItemPath);
            if (rawItemId.StartsWith("PersonalizationItem_"))
            {
                rawItemId = rawItemId.Replace("PersonalizationItem_", string.Empty).Remove(8);
            }
            else if (rawItemId.Length > 8)
            {
                rawItemId = rawItemId.Remove(8);
            }

            string rawItemIdLower = rawItemId.ToLower();
            List<PersonalizationItemInfo> itemList = PersonalizationManager.Instance.ItemList.Items;
            foreach (PersonalizationItemInfo item in itemList)
            {
                if (item != null && !(item.ItemID.IsNullOrEmpty() || item.ItemID.IsNullOrWhiteSpace()) && item.ItemID.ToLower().StartsWith(rawItemIdLower))
                {
                    itemInfo = item;
                    ModDebug.Log($"{rawItemId} is already imported");
                    return true;
                }
            }

            ModDebug.Log($"{rawItemId} is not imported");

            itemInfo = null;
            return false;
        }

        public void ImportOrUpdateItem(string path, Action<PersonalizationItemImportResult> callback, bool editItem = false)
        {
            if (HasImportedItem(path, out PersonalizationItemInfo itemInfo))
            {
                ModUIUtils.MessagePopup(true, $"Update {itemInfo.Name}?", "Replace the old version with the new one?", 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "Ok", "Yes", "No", null, delegate
                {
                    DeleteItem(itemInfo);

                    PersonalizationItemImportResult result = ImportItem(path, editItem);
                    if (result.Result != PersonalizationItemImportResult.ImportResult.Success)
                    {
                        if (callback != null) callback(result);
                        return;
                    }

                    if (callback != null) callback(new PersonalizationItemImportResult(PersonalizationItemImportResult.ImportResult.Updated));
                }, delegate
                {
                    if (callback != null) callback(new PersonalizationItemImportResult(PersonalizationItemImportResult.ImportResult.Cancelled));
                });
            }
            else
            {
                PersonalizationItemImportResult result = ImportItem(path, editItem);
                if (result.Result != PersonalizationItemImportResult.ImportResult.Success)
                {
                    if (callback != null) callback(result);
                    return;
                }

                if (callback != null) callback(new PersonalizationItemImportResult(PersonalizationItemImportResult.ImportResult.Success));
            }
        }

        public string GetExportedItemFileName(PersonalizationItemInfo personalizationItemInfo)
        {
            return $"{ModFileUtils.GetDirectoryName(personalizationItemInfo.FolderPath)}.zip";
        }

        public void ExportItem(PersonalizationItemInfo personalizationItemInfo, out string destination, string overrideDirectoryPath = null, string overrideFn = null)
        {
            string fn = overrideFn.IsNullOrEmpty() ? GetExportedItemFileName(personalizationItemInfo) : overrideFn;
            string folder = overrideDirectoryPath.IsNullOrEmpty() ? ModDirectories.SavesFolder : overrideDirectoryPath;
            destination = Path.Combine(folder, fn);

            if (File.Exists(destination))
                File.Delete(destination);

            FastZip fastZip = new FastZip();
            fastZip.CreateZip(destination, personalizationItemInfo.FolderPath, true, null);
        }
    }
}