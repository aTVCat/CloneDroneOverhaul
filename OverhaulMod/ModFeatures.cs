using OverhaulMod.Content;

namespace OverhaulMod
{
    public static class ModFeatures
    {
        public static bool IsEnabled(FeatureType feature)
        {
            bool unknownFeature = false;
            bool result;
            switch (feature)
            {
                case FeatureType.EndlessModeMenu:
                    result = true;
                    break;
                case FeatureType.NewWeapons:
                    result = true;
                    break;
                case FeatureType.WeaponBag:
                    result = true;
                    break;
                case FeatureType.ChapterSelectMenuRework:
                    result = true;
                    break;
                case FeatureType.LoadingScreenRework:
                    result = true;
                    break;
                case FeatureType.PauseMenuRework:
                    result = true;
                    break;
                case FeatureType.TitleScreenRework:
                    result = true;
                    break;
                case FeatureType.ArrowModelRefresh:
                    result = true;
                    break;
                case FeatureType.WeatherSystem:
                    result = true;
                    break;
                case FeatureType.WorkshopBrowserRework:
                    result = true;
                    break;
                case FeatureType.NightmariumDifficultyTier:
                    result = ModBuildInfo.enableV5;
                    break;
                case FeatureType.GameModeSelectionScreensRework:
                    result = true;
                    break;
                case FeatureType.AllGameplayContent:
                    result = ModBuildInfo.enableV5;
                    break;
                case FeatureType.AllNewEnemies:
                    result = ModBuildInfo.enableV5;
                    break;
                case FeatureType.DuelInviteMenuRework:
                    result = true;
                    break;
                default:
                    result = false;
                    unknownFeature = true;
                    break;
            }

            if (!unknownFeature && !result)
            {
                ExclusiveContentManager modExclusiveContentManager = ExclusiveContentManager.Instance;
                if (modExclusiveContentManager && modExclusiveContentManager.IsFeatureUnlocked(feature))
                    result = true;
            }

            return result;
        }

        public enum FeatureType
        {
            EndlessModeMenu,

            ChapterSelectMenuRework,

            LoadingScreenRework,

            PauseMenuRework,

            TitleScreenRework,

            ArrowModelRefresh,

            NewWeapons,

            WeaponBag,

            WeatherSystem,

            WorkshopBrowserRework,

            NightmariumDifficultyTier,

            GameModeSelectionScreensRework,

            AllGameplayContent,

            AllNewEnemies,

            DuelInviteMenuRework,
        }
    }
}
