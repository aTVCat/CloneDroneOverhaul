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
                    result = false;
                    break;
                case FeatureType.NewDifficultyTiers:
                    result = ModBuildInfo.enableV5;
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
            /// <summary>
            /// <see cref="UI.UIEndlessModeMenu"/>
            /// </summary>
            EndlessModeMenu,

            /// <summary>
            /// <see cref="UI.UIChapterSelectMenuRework"/>
            /// </summary>
            ChapterSelectMenuRework,

            /// <summary>
            /// <see cref="UI.UILoadingScreenRework"/>
            /// </summary>
            LoadingScreenRework,

            /// <summary>
            /// <see cref="UI.UIPauseMenuRework"/>
            /// </summary>
            PauseMenuRework,

            /// <summary>
            /// <see cref="UI.UITitleScreenRework"/>
            /// </summary>
            TitleScreenRework,

            /// <summary>
            /// <see cref="Visuals.ArrowModelRefresher"/>
            /// </summary>
            ArrowModelRefresh,

            /// <summary>
            /// <see cref="Combat.ModWeaponsManager"/>
            /// </summary>
            NewWeapons,

            /// <summary>
            /// <see cref="Visuals.RobotWeaponBag"/>
            /// </summary>
            WeaponBag,

            /// <summary>
            /// <see cref="Visuals.Environment.WeatherManager"/>
            /// </summary>
            WeatherSystem,

            /// <summary>
            /// <see cref="UI.UIWorkshopBrowser"/>
            /// </summary>
            WorkshopBrowserRework,

            /// <summary>
            /// <see cref="Engine.DifficultyTierManager"/>
            /// </summary>
            NewDifficultyTiers,
        }
    }
}
