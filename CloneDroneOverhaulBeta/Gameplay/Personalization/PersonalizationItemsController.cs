using CDOverhaul.Gameplay.Outfits;
using CDOverhaul.Gameplay.Pets;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CDOverhaul.Gameplay
{
    public abstract class PersonalizationItemsController : OverhaulGameplayController
    {
        private static readonly List<PersonalizationItem> s_EmptyList = new List<PersonalizationItem>();

        public PersonalizationItemsData ItemsData
        {
            get;
            set;
        }

        public List<PersonalizationItem> Items => ItemsData == null ? s_EmptyList : ItemsData.Items;

        public override void Initialize()
        {
            base.Initialize();
            ReloadItems();
        }

        public abstract string GetRepositoryFolder();
        public string GetItemsDataFolder()
        {
            OverhaulRepositoryController repositoryController = GetController<OverhaulRepositoryController>();
            return !repositoryController ? string.Empty : repositoryController.GetFolder(GetRepositoryFolder());
        }
        public string GetItemsDataFile() => GetItemsDataFolder() + "ItemsData.json";

        public void ReloadItems()
        {
            if (ItemsData != null)
                ItemsData.Dispose();
            ItemsData = null;

            string fileName = GetItemsDataFile();
            if (!File.Exists(fileName))
            {
                ItemsData = new PersonalizationItemsData() { Items = new List<PersonalizationItem>() };
                using (StreamWriter w = File.CreateText(fileName))
                {
                    w.WriteLine(JsonConvert.SerializeObject(ItemsData, DataRepository.Instance.GetSettings()));
                    w.Close();
                    return;
                }
            }

            string content = OverhaulCore.ReadText(fileName);
            PersonalizationItemsData data = JsonConvert.DeserializeObject<PersonalizationItemsData>(content, DataRepository.Instance.GetSettings());
            if (data.Items == null)
                data.Items = new List<PersonalizationItem>();

            foreach (PersonalizationItem item in data.Items)
            {
                item.OnDeserialized();
            }
            ItemsData = data;
        }

        public virtual PersonalizationItem GetItem(string name)
        {
            List<PersonalizationItem> list = Items;
            if (list.IsNullOrEmpty())
                return null;

            PersonalizationItem result = null;
            int i = 0;
            do
            {
                PersonalizationItem item = list[i];
                if (item == null || string.IsNullOrEmpty(item.Name))
                {
                    i++;
                    continue;
                }

                if (item.Name == name)
                {
                    result = item;
                    break;
                }
                i++;
            } while (i < list.Count);
            return result;
        }

        public virtual List<PersonalizationItem> GetItemsWithSaveString(string saveString)
        {
            List<PersonalizationItem> list = Items;
            if (list.IsNullOrEmpty())
                return s_EmptyList;

            string[] split = saveString.Split(',');
            if (split.IsNullOrEmpty())
                return s_EmptyList;

            List<PersonalizationItem> result = new List<PersonalizationItem>();
            int i = 0;
            do
            {
                PersonalizationItem item = list[i];
                if (item == null)
                {
                    i++;
                    continue;
                }

                if (split.Contains(item.GetID()))
                {
                    result.Add(item);
                }
                i++;
            } while (i < list.Count);
            return result;
        }

        public virtual List<PersonalizationItem> SearchItems(string searchText, bool searchByAuthor)
        {
            List<PersonalizationItem> list = Items;
            if (list.IsNullOrEmpty())
                return s_EmptyList;

            List<PersonalizationItem> result = null;
            int i = 0;
            do
            {
                PersonalizationItem item = list[i];
                if (item == null)
                {
                    i++;
                    continue;
                }

                string checkString = (searchByAuthor ? item.Author : item.Name).ToLower();
                if (checkString.Contains(searchText.ToLower()))
                {
                    result.Add(item);
                }
                i++;
            } while (i < list.Count);
            return result;
        }

        public static void SavePreferences()
        {
            SettingInfo info = OverhaulSettingsController.GetSetting("Player.Outfits.Equipped", true);
            if (info != null)
                SettingInfo.SavePref(info, OutfitsController.EquippedAccessories);

            SettingInfo info2 = OverhaulSettingsController.GetSetting("Player.Pets.Equipped", true);
            if (info2 != null)
                SettingInfo.SavePref(info2, PetsController.EquippedPets);
        }
    }
}
