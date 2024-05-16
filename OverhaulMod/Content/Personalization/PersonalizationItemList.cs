using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationItemList
    {
        public List<PersonalizationItemInfo> Items;

        [NonSerialized]
        public Dictionary<string, Exception> ItemLoadErrors;

        [NonSerialized]
        public Exception LoadError;

        public void Load()
        {
            Dictionary<string, Exception> errors = new Dictionary<string, Exception>();
            List<PersonalizationItemInfo> list = Items ?? new List<PersonalizationItemInfo>();
            list.Clear();

            string[] directories;
            try
            {
                directories = Directory.GetDirectories(ModCore.customizationFolder);
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
                    string dirName = Path.GetDirectoryName(directory);
                    string d = directory + "/";
                    string infoFile = d + PersonalizationManager.ITEM_INFO_FILE;
                    if (File.Exists(infoFile))
                    {
                        try
                        {
                            PersonalizationItemInfo personalizationItemInfo = ModJsonUtils.DeserializeStream<PersonalizationItemInfo>(infoFile);
                            personalizationItemInfo.FolderPath = d;
                            personalizationItemInfo.FixValues();
                            list.Add(personalizationItemInfo);
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

        public List<PersonalizationItemInfo> GetItems()
        {
            return Items;
        }

        public List<PersonalizationItemInfo> GetItems(PersonalizationCategory personalizationCategory)
        {
            List<PersonalizationItemInfo> list = new List<PersonalizationItemInfo>();
            foreach(var item in GetItems())
            {
                if(item.Category == personalizationCategory)
                {
                    list.Add(item);
                }
            }
            return list;
        }
    }
}
