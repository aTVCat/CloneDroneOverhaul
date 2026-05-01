namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationAssetsInfo
    {
        public int AssetVersionNumber = -1;

        public CategoryCounter WeaponSkins, Accessories, Pets;

        public void RefreshCounters(PersonalizationItemList itemList)
        {
            WeaponSkins.TotalCount = 0;
            WeaponSkins.VerifiedCount = 0;
            Accessories.TotalCount = 0;
            Accessories.VerifiedCount = 0;
            Pets.TotalCount = 0;
            Pets.VerifiedCount = 0;

            foreach (PersonalizationItemInfo item in itemList.Items)
            {
                if (item.IsPersistentAsset) continue;

                switch (item.Category)
                {
                    case PersonalizationCategory.WeaponSkins:
                        WeaponSkins.TotalCount++;
                        if (item.IsVerified) WeaponSkins.VerifiedCount++;
                        break;
                    case PersonalizationCategory.Accessories:
                        Accessories.TotalCount++;
                        if (item.IsVerified) Accessories.VerifiedCount++;
                        break;
                    case PersonalizationCategory.Pets:
                        Pets.TotalCount++;
                        if (item.IsVerified) Pets.VerifiedCount++;
                        break;
                }
            }
        }

        public int GetTotalItems()
        {
            int totalCount = WeaponSkins.TotalCount;
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.Accessories)) totalCount += Accessories.TotalCount;
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.Pets)) totalCount += Pets.TotalCount;

            return totalCount;
        }

        public int GetTotalVerifiedItems()
        {
            int totalCount = WeaponSkins.VerifiedCount;
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.Accessories)) totalCount += Accessories.VerifiedCount;
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.Pets)) totalCount += Pets.VerifiedCount;

            return totalCount;
        }

        public bool IsSuitableForComparison()
        {
            return WeaponSkins.VerifiedCount != 0 || (Accessories.VerifiedCount != 0 && ModFeatures.IsEnabled(ModFeatures.FeatureType.Accessories)) || (Pets.VerifiedCount != 0 && ModFeatures.IsEnabled(ModFeatures.FeatureType.Pets));
        }

        public struct CategoryCounter
        {
            public int TotalCount;

            public int VerifiedCount;
        }
    }
}
