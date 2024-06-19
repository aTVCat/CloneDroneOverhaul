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

        [NonSerialized]
        private int m_version;

        public void Load()
        {
            m_version++;
            Dictionary<string, Exception> errors = new Dictionary<string, Exception>();
            List<PersonalizationItemInfo> list = Items ?? new List<PersonalizationItemInfo>();
            list.Clear();

            if (DuplicateItems == null)
                DuplicateItems = new List<PersonalizationItemInfo>();

            List<string> directories;
            try
            {
                directories = new List<string>();
                directories.AddRange(Directory.GetDirectories(ModCore.customizationFolder));
                directories.AddRange(Directory.GetDirectories(ModCore.customizationPersistentFolder));
            }
            catch (Exception exc)
            {
                LoadError = exc;
                Items = list;
                ItemLoadErrors = errors;
                return;
            }

            if (!directories.IsNullOrEmpty())
            {
                foreach (string directory in directories)
                {
                    string rootDirectory = Directory.GetParent(directory).FullPath;
                    string rootDirectoryName = new DirectoryInfo(rootDirectory).Name;

                    string dirName = Path.GetDirectoryName(directory);
                    string infoFile = Path.Combine(directory, PersonalizationEditorManager.ITEM_INFO_FILE);
                    if (File.Exists(infoFile))
                    {
                        try
                        {
                            string filesDirectory = Path.Combine(directory, "files");
                            if (!Directory.Exists(filesDirectory))
                                _ = Directory.CreateDirectory(filesDirectory);

                            PersonalizationItemInfo personalizationItemInfo = ModJsonUtils.DeserializeStream<PersonalizationItemInfo>(infoFile);
                            personalizationItemInfo.FolderPath = directory;
                            personalizationItemInfo.RootFolderPath = rootDirectory;
                            personalizationItemInfo.RootFolderName = rootDirectoryName;
                            personalizationItemInfo.IsPersistentAsset = rootDirectoryName == ModCore.CUSTOMIZATION_PERSISTENT_FOLDER_NAME;
                            personalizationItemInfo.FixValues();

                            if (getItem(personalizationItemInfo.ItemID, list) == null)
                                list.Add(personalizationItemInfo);
                            else
                                DuplicateItems.Add(personalizationItemInfo);
                        }
                        catch (Exception exc)
                        {
                            if (errors.ContainsKey(dirName))
                                continue;

                            errors.Add(dirName, exc);
                        }
                    }
                }
            }

            Items = list;
            ItemLoadErrors = errors;
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
            // todo: cache this using versions
            List<PersonalizationItemInfo> result;
            List<PersonalizationItemInfo> list = GetItems(personalizationCategory);
            switch (sort)
            {
                case PersonalizationItemsSortType.Alphabet:
                    result = GetItems(personalizationCategory).OrderBy(f => f.Name).ToList();
                    break;
                case PersonalizationItemsSortType.AlphabetReverse:
                    result = GetItems(personalizationCategory).OrderBy(f => f.Name).Reverse().ToList();
                    break;
                case PersonalizationItemsSortType.Exclusivity:
                    result = GetItems(personalizationCategory).OrderBy(f => f.Name).OrderBy(f => !f.IsExclusive()).ToList();
                    break;
                case PersonalizationItemsSortType.ExclusivityReverse:
                    result = GetItems(personalizationCategory).OrderBy(f => f.Name).OrderBy(f => f.IsExclusive()).ToList();
                    break;
                default:
                    result = list;
                    break;
            }
            return result;
        }
    }
}
