using OverhaulMod.Content;

namespace OverhaulMod
{
    public static class ModFeatures
    {
        public static bool IsEnabled(FeatureType feature)
        {
            ModExclusiveContentManager modExclusiveContentManager = ModExclusiveContentManager.Instance;
            if (modExclusiveContentManager)
                foreach (ExclusiveContentInfo content in modExclusiveContentManager.GetContentOfType<ExclusiveContentFeatureUnlock>())
                    if (content.Content is ExclusiveContentFeatureUnlock featureUnlock && featureUnlock.Feature == feature)
                    {
                        if (content.IsAvailableToLocalUser())
                        {
                            return true;
                        }
                        break;
                    }

            switch (feature)
            {
                case FeatureType.EndlessModeMenu:
                    return true;
                case FeatureType.NewWeapons:
                    return true;
                case FeatureType.WeaponBag:
                    return true;
                case FeatureType.ChapterSelectMenuRework:
                    return true;
                case FeatureType.LoadingScreenRework:
                    return true;
                case FeatureType.PauseMenuRework:
                    return true;
                case FeatureType.TitleScreenRework:
                    return false;
                case FeatureType.ArrowModelRefresh:
                    return true;
            }
            throw new System.Exception($"Unknown Overhaul feature: {feature}");
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
            WeaponBag
        }
    }
}
