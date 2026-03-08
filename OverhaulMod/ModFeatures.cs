using OverhaulMod.Content;
using System.Collections.Generic;

namespace OverhaulMod
{
    public static class ModFeatures
    {
        private static readonly Dictionary<FeatureType, bool> m_cachedValues = new Dictionary<FeatureType, bool>();

        public static void CacheValues()
        {
            Dictionary<FeatureType, bool> d = m_cachedValues;
            d.Clear();

            foreach (FeatureType feature in typeof(FeatureType).GetEnumValues())
                d.Add(feature, IsEnabled(feature, false));
        }

        public static bool IsEnabled(FeatureType feature, bool useCaching = true)
        {
            if (useCaching)
            {
                Dictionary<FeatureType, bool> d = m_cachedValues;
                if (d != null && d.ContainsKey(feature))
                    return d[feature];
            }

            bool result;
            switch (feature)
            {
                case FeatureType.WeaponBag:
                    result = ModBuildInfo.VERSION_5_0;
                    break;
                case FeatureType.WeatherSystem:
                    result = ModBuildInfo.VERSION_4_3;
                    break;
                case FeatureType.NightmariumDifficultyTier:
                    result = ModBuildInfo.VERSION_5_0;
                    break;
                case FeatureType.WorkshopBrowserContextMenu:
                    result = false;
                    break;
                case FeatureType.RevertUpgrades:
                    result = ModBuildInfo.VERSION_5_0;
                    break;
                case FeatureType.DisplayNewGraphicsOptionsInSettings:
                    result = false;
                    break;
                case FeatureType.WorkshopBrowserHistoryAndCheckpoints:
                    result = ModBuildInfo.VERSION_4_3;
                    break;
                case FeatureType.UISounds:
                    result = ModBuildInfo.VERSION_4_4;
                    break;
                case FeatureType.Hypocrisis3Special:
                    result = false;
                    break;
                case FeatureType.ShieldSkins:
                    result = false;
                    break;
                case FeatureType.Accessories:
                    result = ModBuildInfo.VERSION_4_3;
                    break;
                case FeatureType.Pets:
                    result = false;
                    break;
                default:
                    return false;
            }

            if (!result)
            {
                ExclusivePerkManager exclusivePerkManager = ExclusivePerkManager.Instance;
                if (exclusivePerkManager && exclusivePerkManager.IsFeatureUnlocked(feature))
                    result = true;
            }
            return result;
        }

        public enum FeatureType
        {
            WeaponBag,

            WeatherSystem,

            NightmariumDifficultyTier,

            WorkshopBrowserContextMenu,

            RevertUpgrades,

            DisplayNewGraphicsOptionsInSettings,

            WorkshopBrowserHistoryAndCheckpoints,

            UISounds,

            Hypocrisis3Special,

            ShieldSkins,

            Accessories,

            Pets,
        }
    }
}