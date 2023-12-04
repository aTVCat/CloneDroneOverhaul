namespace OverhaulMod
{
    internal static class ModFeatures
    {
        public static bool IsEnabled(EModFeature feature)
        {
            switch (feature)
            {
                case EModFeature.EndlessModeMenu:
                    return true;
                case EModFeature.NewWeapons:
                    return true;
                case EModFeature.WeaponBag:
                    return true;
            }
            throw new System.Exception(string.Format("Unknown Overhaul feature: {0}", feature));
        }

        public enum EModFeature
        {
            EndlessModeMenu,
            NewWeapons,
            WeaponBag
        }
    }
}
