namespace OverhaulMod
{
    public static class ModFeatures
    {
        public static bool IsEnabled(FeatureType feature)
        {
            switch (feature)
            {
                case FeatureType.EndlessModeMenu:
                    return true;
                case FeatureType.NewWeapons:
                    return true;
                case FeatureType.WeaponBag:
                    return true;
            }
            throw new System.Exception(string.Format("Unknown Overhaul feature: {0}", feature));
        }

        public enum FeatureType
        {
            EndlessModeMenu,
            NewWeapons,
            WeaponBag
        }
    }
}
