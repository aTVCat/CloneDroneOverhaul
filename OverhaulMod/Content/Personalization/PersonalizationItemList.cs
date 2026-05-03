using Newtonsoft.Json.Linq;
using OverhaulMod.Content.Personalization.Objects;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationItemList
    {
        public List<PersonalizationItemInfo> Items;

        public List<PersonalizationItemInfo> DuplicateItems;

        [NonSerialized]
        public Dictionary<string, Exception> ItemLoadErrors;

        [NonSerialized]
        public Exception LoadError;

        public void Load()
        {
            Dictionary<string, Exception> errors = new Dictionary<string, Exception>();
            List<PersonalizationItemInfo> list = Items ?? new List<PersonalizationItemInfo>();
            list.Clear();

            if (DuplicateItems == null)
                DuplicateItems = new List<PersonalizationItemInfo>();
            List<string> directories;

            int userItemsStartIndex;
            try
            {
                directories = new List<string>();
                directories.AddRange(Directory.GetDirectories(ModDirectories.CustomizationFolder));
                userItemsStartIndex = directories.Count;
                directories.AddRange(Directory.GetDirectories(ModDirectories.CustomizationPersistentFolder));
            }
            catch (Exception exc)
            {
                LoadError = exc;
                Items = list;
                ItemLoadErrors = errors;
                return;
            }

            Items = list;
            ItemLoadErrors = errors;

            if (directories.IsNullOrEmpty())
                return;

            int index = -1;
            foreach (string directory in directories)
            {
                index++;

                try
                {
                    PersonalizationItemInfo personalizationItemInfo = LoadItemInfo(directory);
                    personalizationItemInfo.HideInBrowser = userItemsStartIndex != -1 && index < userItemsStartIndex && !personalizationItemInfo.IsVerified;
                    if (getItem(personalizationItemInfo.ItemID, list) == null)
                        list.Add(personalizationItemInfo);
                    else
                        DuplicateItems.Add(personalizationItemInfo);
                }
                catch (Exception exc)
                {
                    if (errors.ContainsKey(directory))
                        continue;

                    errors.Add(directory, exc);
                }
            }

            PersonalizationCacheManager.Instance.CacheFiles(list);
        }

        public PersonalizationItemInfo LoadItemInfo(string directory)
        {
            string rootDirectory = Directory.GetParent(directory).FullName;
            string rootDirectoryName = ModFileUtils.GetDirectoryName(rootDirectory);

            string infoFilePath = Path.Combine(directory, PersonalizationEditorDataManager.ITEM_INFO_FILE);
            string metaDataFilePath = Path.Combine(directory, PersonalizationEditorDataManager.ITEM_META_DATA_FILE);
            string objectsFilePath = Path.Combine(directory, PersonalizationEditorDataManager.ITEM_OBJECTS_FILE);
            string accessoryOffsetsFilePath = Path.Combine(directory, PersonalizationEditorDataManager.ITEM_ACCESSORY_OFFSETS_FILE);

            bool updateInfoFile = false;
            bool serializeInfoFile = false;
            bool updateMetaDataFile = false;

            PersonalizationItemMetaData personalizationItemMetaData;
            if (File.Exists(metaDataFilePath))
            {
                personalizationItemMetaData = ModJsonUtils.DeserializeStream<PersonalizationItemMetaData>(metaDataFilePath);
            }
            else
            {
                updateMetaDataFile = true;
                personalizationItemMetaData = new PersonalizationItemMetaData() // the first Version of customization system (0) didn't have meta data files
                {
                    CustomizationSystemVersion = 0,
                };
            }

            PersonalizationItemInfo personalizationItemInfo;
            if (File.Exists(infoFilePath))
            {
                string filesDirectory = Path.Combine(directory, "files");
                if (!Directory.Exists(filesDirectory))
                    _ = Directory.CreateDirectory(filesDirectory);

                string rawData = ModFileUtils.ReadText(infoFilePath);
                if (personalizationItemMetaData.CustomizationSystemVersion < 1) // meta data files update, renamed OverhaulMod.Content.Personalization.PersonalizationEditorObjectShowConditions to OverhaulMod.Engine.WeaponVariant
                {
                    updateMetaDataFile = true;
                    updateInfoFile = true;

                    rawData = rawData.Replace("OverhaulMod.Content.Personalization.PersonalizationEditorObjectShowConditions", "OverhaulMod.Engine.WeaponVariant");
                }
                if (personalizationItemMetaData.CustomizationSystemVersion < 2) // removed "Is" from every value name in WeaponVariant2 enum
                {
                    updateMetaDataFile = true;
                    updateInfoFile = true;

                    rawData = rawData.Replace("\"IsNormal\"", "\"Normal\"");
                    rawData = rawData.Replace("\"IsOnFire\"", "\"OnFire\"");
                    rawData = rawData.Replace("\"IsNormalMultiplayer\"", "\"NormalMultiplayer\"");
                    rawData = rawData.Replace("\"IsOnFireMultiplayer\"", "\"OnFireMultiplayer\"");
                }
                if (personalizationItemMetaData.CustomizationSystemVersion < 3) // renamed WeaponVariant to WeaponVariant2, because the game now has enum with the same name
                {
                    updateMetaDataFile = true;
                    updateInfoFile = true;

                    rawData = rawData.Replace("OverhaulMod.Engine.WeaponVariant", "OverhaulMod.Engine.WeaponVariant2");
                }
                if (personalizationItemMetaData.CustomizationSystemVersion < 4) // moved root object info to a separate file
                {
                    updateMetaDataFile = true;
                    updateInfoFile = false;
                    serializeInfoFile = true;

                    rawData = rawData.Replace("OverhaulMod.Content.Personalization.PersonalizationEditorObjectInfo", "OverhaulMod.Content.Personalization.Objects.PersonalizationEditorObjectInfo");
                    JObject data = ModJsonUtils.Deserialize<JObject>(rawData);
                    PersonalizationEditorObjectInfo rootObject = data["RootObject"].ToObject<PersonalizationEditorObjectInfo>();
                    fixCastsRecursive(rootObject);
                    ModJsonUtils.WriteStream(objectsFilePath, rootObject);
                }
                if (personalizationItemMetaData.CustomizationSystemVersion < 5) // reworked how properties are stored, renamed classes
                {
                    updateMetaDataFile = true;
                    updateInfoFile = false;
                    serializeInfoFile = true;

                    string objectsRawData = ModFileUtils.ReadText(objectsFilePath);
                    objectsRawData = objectsRawData.Replace("OverhaulMod.Content.Personalization.PersonalizationEditorObjectInfo", "OverhaulMod.Content.Personalization.Objects.PersonalizationEditorObjectInfo");
                    PersonalizationEditorObjectInfo rootObject = ModJsonUtils.Deserialize<PersonalizationEditorObjectInfo>(objectsRawData);
                    fixPropertiesRecursive(rootObject);
                    ModJsonUtils.WriteStream(objectsFilePath, rootObject);
                }
                personalizationItemMetaData.CustomizationSystemVersion = PersonalizationItemMetaData.CurrentCustomizationSystemVersion;

                personalizationItemInfo = ModJsonUtils.Deserialize<PersonalizationItemInfo>(rawData);
                personalizationItemInfo.FolderPath = directory;
                personalizationItemInfo.RootFolderPath = rootDirectory;
                personalizationItemInfo.IsPersistentAsset = rootDirectoryName == ModDirectories.CUSTOMIZATION_PERSISTENT_FOLDER_NAME;
                personalizationItemInfo.MetaData = personalizationItemMetaData;
                personalizationItemInfo.FixValues();
                if (serializeInfoFile) ModJsonUtils.WriteStream(infoFilePath, personalizationItemInfo);

                if (updateInfoFile)
                    ModFileUtils.WriteText(rawData, infoFilePath);
            }
            else
            {
                personalizationItemInfo = null;
            }

            if (updateMetaDataFile)
                ModJsonUtils.WriteStream(metaDataFilePath, personalizationItemMetaData);

            if (personalizationItemInfo != null)
            {
                if (personalizationItemInfo.Category == PersonalizationCategory.Accessories)
                {
                    AccessoryOffsetsList accessoryOffsetsList;
                    try
                    {
                        accessoryOffsetsList = ModJsonUtils.DeserializeStream<AccessoryOffsetsList>(accessoryOffsetsFilePath);
                    }
                    catch (Exception)
                    {
                        accessoryOffsetsList = new AccessoryOffsetsList();
                    }
                    accessoryOffsetsList.InitializeList();
                    personalizationItemInfo.AccessoryOffsets = accessoryOffsetsList;
                }
            }

            return personalizationItemInfo;
        }

        private void fixCastsRecursive(PersonalizationEditorObjectInfo objectInfo)
        {
            if (objectInfo.PropertyValues != null && objectInfo.PropertyValues.Count != 0)
            {
                Dictionary<string, object> modifiedValues = new Dictionary<string, object>();
                foreach (KeyValuePair<string, object> keyValue in objectInfo.PropertyValues)
                {
                    if (keyValue.Key == "volumeSettingPresets")
                    {
                        Dictionary<WeaponVariant2, VolumeSettingsPreset> fixedValue = (keyValue.Value as JObject).ToObject<Dictionary<WeaponVariant2, VolumeSettingsPreset>>();
                        modifiedValues.Add(keyValue.Key, fixedValue);
                    }
                    else if (keyValue.Key == "presets")
                    {
                        Dictionary<WeaponVariant2, CVMModelPreset> fixedValue = (keyValue.Value as JObject).ToObject<Dictionary<WeaponVariant2, CVMModelPreset>>();
                        modifiedValues.Add(keyValue.Key, fixedValue);
                    }
                }

                foreach (KeyValuePair<string, object> keyValue in modifiedValues)
                {
                    objectInfo.PropertyValues[keyValue.Key] = keyValue.Value;
                }
            }

            if (objectInfo.Children != null && objectInfo.Children.Count != 0)
            {
                foreach (PersonalizationEditorObjectInfo child in objectInfo.Children)
                {
                    fixCastsRecursive(child);
                }
            }
        }

        private void fixPropertiesRecursive(PersonalizationEditorObjectInfo objectInfo)
        {
            if (objectInfo.PropertyValues != null && objectInfo.PropertyValues.Count != 0)
            {
                foreach (KeyValuePair<string, object> keyValue in new Dictionary<string, object>(objectInfo.PropertyValues))
                {
                    // capitalize property names
                    string key = keyValue.Key;
                    string newKey = key[0].ToString().ToUpper() + key.Substring(1);

                    // add class names
                    bool isEnableIfWeaponVariant = newKey == nameof(PersonalizationEditorVisibilityToggler.EnableIfWeaponVariant);
                    if (isEnableIfWeaponVariant) newKey = "PersonalizationEditorVisibilityToggler." + newKey;
                    if (objectInfo.Path == "Volume")
                    {
                        if (newKey == nameof(PersonalizationEditorVoxModel.VolumeSettingPresets)) newKey = "PersonalizationEditorVoxModel." + newKey;
                        else if (newKey == nameof(PersonalizationEditorVoxModel.HideIfNoPreset)) newKey = "PersonalizationEditorVoxModel." + newKey;
                    }
                    else if (objectInfo.Path == "CvmModel")
                    {
                        if (newKey == nameof(PersonalizationEditorCVMModel.Presets)) newKey = "PersonalizationEditorCVMModel." + newKey;
                        else if (newKey == nameof(PersonalizationEditorCVMModel.HideIfNoPreset)) newKey = "PersonalizationEditorCVMModel." + newKey;
                    }
                    else if (objectInfo.Path.StartsWith("FireParticles") && !isEnableIfWeaponVariant)
                    {
                        newKey = "PersonalizationEditorFireParticles." + newKey;
                    }

                    objectInfo.PropertyValues.Remove(key);
                    objectInfo.PropertyValues.Add(newKey, keyValue.Value);
                }
            }

            if (objectInfo.Children != null && objectInfo.Children.Count != 0)
            {
                foreach (PersonalizationEditorObjectInfo child in objectInfo.Children)
                {
                    fixPropertiesRecursive(child);
                }
            }
        }

        private PersonalizationItemInfo getItem(string id, List<PersonalizationItemInfo> list)
        {
            foreach (PersonalizationItemInfo item in list)
            {
                if (item.ItemID == id)
                    return item;
            }
            return null;
        }

        public PersonalizationItemInfo GetItem(string id)
        {
            return getItem(id, GetItems());
        }

        public List<PersonalizationItemInfo> GetItems()
        {
            return Items;
        }

        public List<PersonalizationItemInfo> GetItems(PersonalizationCategory personalizationCategory)
        {
            List<PersonalizationItemInfo> list = new List<PersonalizationItemInfo>();
            foreach (PersonalizationItemInfo item in GetItems())
            {
                if (item.Category == personalizationCategory)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public List<PersonalizationItemInfo> GetItems(PersonalizationCategory personalizationCategory, PersonalizationItemsSortType sort)
        {
            return sortItems(GetItems(personalizationCategory), sort);
        }

        public List<PersonalizationItemInfo> GetWeaponSkins(WeaponType weaponType)
        {
            List<PersonalizationItemInfo> list = new List<PersonalizationItemInfo>();
            foreach (PersonalizationItemInfo item in GetItems())
            {
                if (item.Category == PersonalizationCategory.WeaponSkins && item.Weapon == weaponType)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public List<PersonalizationItemInfo> GetWeaponSkins(WeaponType weaponType, PersonalizationItemsSortType sort)
        {
            return sortItems(GetWeaponSkins(weaponType), sort);
        }

        private List<PersonalizationItemInfo> sortItems(List<PersonalizationItemInfo> list, PersonalizationItemsSortType sort)
        {
            List<PersonalizationItemInfo> result;
            switch (sort)
            {
                case PersonalizationItemsSortType.Alphabet:
                    result = list.OrderBy(f => f.Name).ToList();
                    break;
                case PersonalizationItemsSortType.AlphabetReverse:
                    result = list.OrderBy(f => f.Name).Reverse().ToList();
                    break;
                case PersonalizationItemsSortType.Exclusivity:
                    result = list.OrderBy(f => f.Name).OrderBy(f => !f.IsExclusive()).ToList();
                    break;
                case PersonalizationItemsSortType.ExclusivityReverse:
                    result = list.OrderBy(f => f.Name).OrderBy(f => f.IsExclusive()).ToList();
                    break;
                default:
                    result = list;
                    break;
            }
            return result;
        }
    }
}