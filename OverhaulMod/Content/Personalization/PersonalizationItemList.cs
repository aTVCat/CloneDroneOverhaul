using OverhaulMod.Utils;
using System.Collections.Generic;
using System.IO;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationItemList
    {
        public List<PersonalizationItemInfo> Items;

        public void Load()
        {
            List<PersonalizationItemInfo> list = Items ?? new List<PersonalizationItemInfo>();
            list.Clear();

            string[] directories = Directory.GetDirectories(ModCore.customizationFolder);
            if (!directories.IsNullOrEmpty())
            {
                foreach (string directory in directories)
                {
                    string d = directory + "/";
                    string infoFile = d + PersonalizationManager.ITEM_INFO_FILE;
                    if (File.Exists(infoFile))
                    {
                        try
                        {
                            PersonalizationItemInfo personalizationItemInfo = ModJsonUtils.DeserializeStream<PersonalizationItemInfo>(infoFile);
                            personalizationItemInfo.FolderPath = d;
                            list.Add(personalizationItemInfo);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }

            Items = list;
        }
    }
}
