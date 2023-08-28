//#define AllowLevelDataPatches

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CDOverhaul
{
    public static class OverhaulFeatureAvailabilitySystem
    {
        private static readonly ReadOnlyCollection<OverhaulFeatureDefinition> s_Features = new ReadOnlyCollection<OverhaulFeatureDefinition>(new List<OverhaulFeatureDefinition>()
        {
            createNew(OverhaulFeatureID.PermissionToManageSkins),
            createNew(OverhaulFeatureID.PermissionToCopyUserInfos)
        });

        private static OverhaulFeatureDefinition createNew(OverhaulFeatureID id)
        {
            OverhaulFeatureDefinition def;
            switch (id)
            {
                case OverhaulFeatureID.PermissionToManageSkins:
                    def = new OverhaulFeatureDefinition.AbilityToManageSkins();
                    break;
                case OverhaulFeatureID.PermissionToCopyUserInfos:
                    def = new OverhaulFeatureDefinition.AbilityToCopyUserInfos();
                    break;
                default:
                    def = new OverhaulFeatureDefinition();
                    break;
            }
            def.FeatureID = id;
            return def;
        }

        public static bool IsFeatureUnlocked(in OverhaulFeatureID featureID)
        {
            if (OverhaulVersion.IsDebugBuild)
                return true;

            OverhaulFeatureDefinition def = getFeatureDefinition(featureID);
            return def != null && def.IsAvailable();
        }

        private static OverhaulFeatureDefinition getFeatureDefinition(in OverhaulFeatureID featureID)
        {
            if (s_Features.IsNullOrEmpty())
                return null;

            int i = 0;
            do
            {
                OverhaulFeatureDefinition def = s_Features[i];
                if (def != null && def.FeatureID == featureID)
                    return def;
                i++;
            } while (i < s_Features.Count);
            return null;
        }

        /// <summary>
        /// Features that doesn't require player IDs to work. This class defines what things should be included in the build
        /// </summary>
        public static class ImplementedInBuild
        {
            /// <summary>
            /// More functional photo mode
            /// </summary>
            private const bool IS_PHOTO_MODE_OVERHAUL_ENABLED = true;
            public static bool IsPhotoModeOverhaulEnabled => OverhaulVersion.IsVersion3Update && IS_PHOTO_MODE_OVERHAUL_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            private const bool IS_ACHIEVEMENTS_MENU_REDESIGN_ENABLED = true;
            public static bool IsAchievementsMenuRedesignEnabled => OverhaulVersion.IsVersion4 && IS_ACHIEVEMENTS_MENU_REDESIGN_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            private const bool IS_CONNECTION_SCREENS_REDESIGN_ENABLED = true;
            public static bool IsConnectionScreensRedesignEnabled => OverhaulVersion.IsVersion4 && IS_CONNECTION_SCREENS_REDESIGN_ENABLED;

            /// <summary>
            /// 
            /// </summary>
            private const bool ARE_NEW_PERSONALIZATION_CATEGORIES_ENABLED = true;
            public static bool AreNewPersonalizationCategoriesEnabled => OverhaulVersion.IsVersion4 && ARE_NEW_PERSONALIZATION_CATEGORIES_ENABLED;

            public const bool IS_DEVELOPER_ALLOWED_TO_USE_LOCKED_STUFF = true;
        }
    }
}
