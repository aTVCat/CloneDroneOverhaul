using System;
using System.Collections.Generic;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationUserInfo
    {
        public List<string> DiscoveredItems;

        public List<string> FavoriteItems;

        [NonSerialized]
        public bool m_isDirty;

        public void FixValues()
        {
            if (DiscoveredItems == null)
                DiscoveredItems = new List<string>();

            if (FavoriteItems == null)
                FavoriteItems = new List<string>();
        }

        public void SetIsDirty(bool value = true)
        {
            m_isDirty = value;
        }

        public bool IsDirty()
        {
            return m_isDirty;
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
            foreach (PersonalizationItemInfo item in PersonalizationManager.Instance.itemList.Items)
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
    }
}
