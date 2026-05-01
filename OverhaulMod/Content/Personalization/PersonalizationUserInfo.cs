using OverhaulMod.Engine;
using OverhaulMod.Gameplay;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationUserInfo
    {
        [ModSetting(ModSettingIDs.SWORD_SKIN, null)]
        public static string SwordSkin;

        [ModSetting(ModSettingIDs.BOW_SKIN, null)]
        public static string BowSkin;

        [ModSetting(ModSettingIDs.HAMMER_SKIN, null)]
        public static string HammerSkin;

        [ModSetting(ModSettingIDs.SPEAR_SKIN, null)]
        public static string SpearSkin;

        [ModSetting(ModSettingIDs.SHIELD_SKIN, null)]
        public static string ShieldSkin;

        [ModSetting(ModSettingIDs.SCYTHE_SKIN, null)]
        public static string ScytheSkin;

        [ModSetting(ModSettingIDs.ACCESSORIES, "")]
        public static string Accessories;

        [ModSetting(ModSettingIDs.PETS, "")]
        public static string Pets;

        [ModSetting(ModSettingIDs.ALLOW_ENEMIES_USE_WEAPON_SKINS, true)]
        public static bool AllowEnemiesUseSkins;

        public List<string> DiscoveredItems;

        public List<string> FavoriteItems;

        public List<string> UnverifiedItems;

        public Dictionary<string, int> ItemVersions;

        [NonSerialized]
        public bool _isDirty;

        public void FixValues()
        {
            if (DiscoveredItems == null) DiscoveredItems = new List<string>();

            if (FavoriteItems == null) FavoriteItems = new List<string>();

            if (UnverifiedItems == null) UnverifiedItems = new List<string>();

            if (ItemVersions == null) ItemVersions = new Dictionary<string, int>();

            if (DiscoveredItems.Count == 0) DiscoverAllItems();

            RefreshAllItemsVerification();

            if (ItemVersions.Count == 0) RefreshAllItemVersions();
        }

        public void SetIsDirty(bool value = true)
        {
            _isDirty = value;
        }

        public bool IsDirty()
        {
            return _isDirty;
        }

        public void SaveIfDirty()
        {
            if (IsDirty())
            {
                SetIsDirty(false);
                PersonalizationManager.Instance.SaveUserInfo();
            }
        }

        public void DiscoverAllItems()
        {
            foreach (PersonalizationItemInfo item in PersonalizationManager.Instance.ItemList.Items)
                SetIsItemDiscovered(item);
        }

        public void SetIsItemDiscovered(PersonalizationItemInfo itemInfo)
        {
            SetIsItemDiscovered(itemInfo.ItemID);
        }

        public void SetIsItemDiscovered(string itemId)
        {
            if (!DiscoveredItems.Contains(itemId))
                DiscoveredItems.Add(itemId);

            SetIsDirty(true);
        }

        public bool IsItemDiscovered(PersonalizationItemInfo itemInfo)
        {
            return IsItemDiscovered(itemInfo.ItemID);
        }

        public bool IsItemDiscovered(string itemId)
        {
            return DiscoveredItems.Contains(itemId);
        }

        public void SetIsItemFavorite(PersonalizationItemInfo itemInfo, bool value)
        {
            SetIsItemFavorite(itemInfo.ItemID, value);
        }

        public void SetIsItemFavorite(string itemId, bool value)
        {
            List<string> list = FavoriteItems;

            if (list.Contains(itemId) && !value)
                _ = list.Remove(itemId);
            else if (!list.Contains(itemId) && value)
                list.Add(itemId);

            SetIsDirty(true);
        }

        public bool IsItemFavorite(PersonalizationItemInfo itemInfo)
        {
            return IsItemFavorite(itemInfo.ItemID);
        }

        public bool IsItemFavorite(string itemId)
        {
            return FavoriteItems.Contains(itemId);
        }

        public void RefreshAllItemsVerification()
        {
            foreach (PersonalizationItemInfo item in PersonalizationManager.Instance.ItemList.Items)
                if (!item.IsVerified)
                    SetIsItemUnverified(item, true);
        }

        public void SetIsItemUnverified(PersonalizationItemInfo itemInfo, bool value)
        {
            SetIsItemUnverified(itemInfo.ItemID, value);
        }

        public void SetIsItemUnverified(string itemId, bool value)
        {
            if (value && !UnverifiedItems.Contains(itemId))
                UnverifiedItems.Add(itemId);
            else if (!value)
                _ = UnverifiedItems.Remove(itemId);

            SetIsDirty(true);
        }

        public bool IsItemUnverified(PersonalizationItemInfo itemInfo)
        {
            return IsItemUnverified(itemInfo.ItemID);
        }

        public bool IsItemUnverified(string itemId)
        {
            return UnverifiedItems.Contains(itemId);
        }

        public void RefreshAllItemVersions()
        {
            foreach (PersonalizationItemInfo item in PersonalizationManager.Instance.ItemList.Items)
                SetItemVersion(item, item.Version);
        }

        public void SetItemVersion(PersonalizationItemInfo itemInfo, int value)
        {
            SetItemVersion(itemInfo.ItemID, value);
        }

        public void SetItemVersion(string itemId, int value)
        {
            if (value != 0)
            {
                if (ItemVersions.ContainsKey(itemId))
                    ItemVersions[itemId] = value;
                else
                    ItemVersions.Add(itemId, value);
            }
            else
            {
                _ = ItemVersions.Remove(itemId);
            }

            SetIsDirty(true);
        }

        public int GetItemVersion(PersonalizationItemInfo itemInfo)
        {
            return GetItemVersion(itemInfo.ItemID);
        }

        public int GetItemVersion(string itemId)
        {
            if (ItemVersions.ContainsKey(itemId))
                return ItemVersions[itemId];

            return 0;
        }

        public static void SetWeaponSkin(WeaponType weaponType, string itemId)
        {
            switch (weaponType)
            {
                case WeaponType.Sword:
                    ModSettingsManager.SetStringValue(ModSettingIDs.SWORD_SKIN, itemId);
                    break;
                case WeaponType.Bow:
                    ModSettingsManager.SetStringValue(ModSettingIDs.BOW_SKIN, itemId);
                    break;
                case WeaponType.Hammer:
                    ModSettingsManager.SetStringValue(ModSettingIDs.HAMMER_SKIN, itemId);
                    break;
                case WeaponType.Spear:
                    ModSettingsManager.SetStringValue(ModSettingIDs.SPEAR_SKIN, itemId);
                    break;
                case WeaponType.Shield:
                    ModSettingsManager.SetStringValue(ModSettingIDs.SHIELD_SKIN, itemId);
                    break;
                case ModWeaponsManager.SCYTHE_TYPE:
                    ModSettingsManager.SetStringValue(ModSettingIDs.SCYTHE_SKIN, itemId);
                    break;
            }
        }

        public static string GetWeaponSkin(WeaponType weaponType)
        {
            switch (weaponType)
            {
                case WeaponType.Sword:
                    return SwordSkin;
                case WeaponType.Bow:
                    return BowSkin;
                case WeaponType.Hammer:
                    return HammerSkin;
                case WeaponType.Spear:
                    return SpearSkin;
                case WeaponType.Shield:
                    return ShieldSkin;
                case ModWeaponsManager.SCYTHE_TYPE:
                    return ScytheSkin;
            }
            return null;
        }

        public static bool IsWeaponSkinEquipped(WeaponType weaponType, string itemId)
        {
            return GetWeaponSkin(weaponType) == itemId;
        }

        public static void SetAccessoryEquipped(string itemId, bool value)
        {
            SetItemEquipped(PersonalizationCategory.Accessories, itemId, value);
        }

        public static void SetPetEquipped(string itemId, bool value)
        {
            SetItemEquipped(PersonalizationCategory.Pets, itemId, value);
        }

        public static void SetItemEquipped(PersonalizationCategory category, string itemId, bool value)
        {
            if (category != PersonalizationCategory.Accessories && category != PersonalizationCategory.Pets)
                throw new ArgumentException($"Must be either {nameof(PersonalizationCategory.Accessories)} or {nameof(PersonalizationCategory.Pets)}", nameof(category));

            // get current value
            string equippedItemsRawList;
            if (category == PersonalizationCategory.Accessories)
            {
                equippedItemsRawList = Accessories;
            }
            else if (category == PersonalizationCategory.Pets)
            {
                equippedItemsRawList = Pets;
            }
            else
            {
                equippedItemsRawList = null;
            }
            if (equippedItemsRawList == null) equippedItemsRawList = string.Empty;

            // update value
            string formattedValue = $"{itemId},";
            if (value && !equippedItemsRawList.Contains(itemId))
            {
                equippedItemsRawList += formattedValue;
            }
            else if (!value && equippedItemsRawList.Contains(formattedValue))
            {
                equippedItemsRawList = equippedItemsRawList.Replace(formattedValue, string.Empty);
            }

            // set value
            string settingId;
            if (category == PersonalizationCategory.Accessories)
            {
                settingId = ModSettingIDs.ACCESSORIES;
            }
            else if (category == PersonalizationCategory.Pets)
            {
                settingId = ModSettingIDs.PETS;
            }
            else
            {
                settingId = null;
            }
            ModSettingsManager.SetStringValue(settingId, equippedItemsRawList);
        }

        public static bool IsAccessoryEquipped(string itemId)
        {
            return IsItemEquipped(PersonalizationCategory.Accessories, itemId);
        }

        public static bool IsPetEquipped(string itemId)
        {
            return IsItemEquipped(PersonalizationCategory.Pets, itemId);
        }

        public static bool IsItemEquipped(PersonalizationCategory category, string itemId)
        {
            if (category != PersonalizationCategory.Accessories && category != PersonalizationCategory.Pets)
                throw new ArgumentException($"Must be either {nameof(PersonalizationCategory.Accessories)} or {nameof(PersonalizationCategory.Pets)}", nameof(category));

            string equippedItemsRawList;
            if (category == PersonalizationCategory.Accessories)
            {
                equippedItemsRawList = Accessories;
            }
            else if (category == PersonalizationCategory.Pets)
            {
                equippedItemsRawList = Pets;
            }
            else
            {
                equippedItemsRawList = null;
            }

            return !equippedItemsRawList.IsNullOrEmpty() && equippedItemsRawList.Contains(itemId);
        }

        public static List<string> GetEquippedAccessoriesList()
        {
            return GetItemList(Accessories);
        }

        public static List<string> GetEquippedPetsList()
        {
            return GetItemList(Pets);
        }

        public static List<string> GetItemList(string rawData)
        {
            if (rawData.IsNullOrEmpty() || rawData == "_")
                return null;

            return new List<string>(StringUtils.GetNonEmptySplitOfCommaSeparatedString(rawData));
        }
    }
}