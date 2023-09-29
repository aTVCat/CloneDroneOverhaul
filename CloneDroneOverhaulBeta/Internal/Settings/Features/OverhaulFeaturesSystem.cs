//#define AllowLevelDataPatches

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CDOverhaul
{
    public static class OverhaulFeaturesSystem
    {
        public const bool IS_DEVELOPER_ALLOWED_TO_USE_LOCKED_STUFF = true;

        public const string NEW_CURSOR_HIDING_METHOD = "NewCursorHidingMethod";

        private static readonly ReadOnlyDictionary<EBuildFeatures, bool> s_BuildFeatures = new ReadOnlyDictionary<EBuildFeatures, bool>(new Dictionary<EBuildFeatures, bool>()
        {
            { EBuildFeatures.Cursor_Disabling_Through_ModBot, true },
            { EBuildFeatures.TitleScreen_Overhaul, true }
        });

        private static readonly ReadOnlyCollection<OverhaulFeatureDefinition> s_UnlockableFeatures = new ReadOnlyCollection<OverhaulFeatureDefinition>(new List<OverhaulFeatureDefinition>()
        {
            createNew(EUnlockableFeatures.PermissionToManageSkins),
            createNew(EUnlockableFeatures.PermissionToCopyUserInfos)
        });

        private static OverhaulFeatureDefinition createNew(EUnlockableFeatures id)
        {
            OverhaulFeatureDefinition def;
            switch (id)
            {
                case EUnlockableFeatures.PermissionToManageSkins:
                    def = new OverhaulFeatureDefinition.AbilityToManageSkins();
                    break;
                case EUnlockableFeatures.PermissionToCopyUserInfos:
                    def = new OverhaulFeatureDefinition.AbilityToCopyUserInfos();
                    break;
                default:
                    def = new OverhaulFeatureDefinition();
                    break;
            }
            def.FeatureID = id;
            return def;
        }

        private static OverhaulFeatureDefinition getFeatureDefinition(in EUnlockableFeatures featureID)
        {
            if (s_UnlockableFeatures.IsNullOrEmpty())
                return null;

            int i = 0;
            do
            {
                OverhaulFeatureDefinition def = s_UnlockableFeatures[i];
                if (def != null && def.FeatureID == featureID)
                    return def;
                i++;
            } while (i < s_UnlockableFeatures.Count);
            return null;
        }

        public static bool IsFeatureUnlocked(in EUnlockableFeatures featureID)
        {
            if (OverhaulVersion.IsDebugBuild)
                return true;

            OverhaulFeatureDefinition def = getFeatureDefinition(featureID);
            return def != null && def.IsAvailable();
        }

        public static bool IsFeatureImplemented(EBuildFeatures buildFeature)
        {
            return !s_BuildFeatures.ContainsKey(buildFeature) ? false : s_BuildFeatures[buildFeature];
        }
    }
}
