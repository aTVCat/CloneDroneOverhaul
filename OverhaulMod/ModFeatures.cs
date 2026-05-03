using OverhaulMod.Content;
using System.Collections.Generic;

namespace OverhaulMod
{
    public static class ModFeatures
    {
        private static readonly Dictionary<FeatureType, bool> _cachedValues = new Dictionary<FeatureType, bool>();

        public static void CacheValues()
        {
            _cachedValues.Clear();
            foreach (FeatureType feature in typeof(FeatureType).GetEnumValues())
                _cachedValues.Add(feature, IsEnabled(feature, false));
        }

        public static bool IsEnabled(FeatureType feature, bool useCaching = true)
        {
            if (useCaching && _cachedValues.ContainsKey(feature))
            {
                return _cachedValues[feature];
            }

            bool result;
            switch (feature)
            {
                case FeatureType.Accessories:
                    result = ModBuild.VERSION_4_3;
                    break;
                case FeatureType.Pets:
                    result = false;// ModBuild.VERSION_4_3;
                    break;
                case FeatureType.UISounds:
                    result = ModBuild.VERSION_4_3;
                    break;
                case FeatureType.SettingsMenuReworkV2:
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
            WorkshopBrowserContextMenu,

            DisplayNewGraphicsOptionsInSettings,

            WorkshopBrowserHistoryAndCheckpoints,

            UISounds,

            HyperdomeSounds,

            Hypocrisis3Special,

            ShieldSkins,

            Accessories,

            Pets,

            SettingsMenuReworkV2,

            PauseMenuLogoAsRenderTexture
        }
    }
}