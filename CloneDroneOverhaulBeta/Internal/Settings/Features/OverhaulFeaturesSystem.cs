//#define AllowLevelDataPatches

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CDOverhaul
{
    public static class OverhaulFeaturesSystem
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
        public static class Implemented
        {
            public const bool IS_DEVELOPER_ALLOWED_TO_USE_LOCKED_STUFF = true;

            public const string NEW_CURSOR_HIDING_METHOD = "NewCursorHidingMethod";

            private static readonly Dictionary<string, bool> s_Features = new Dictionary<string, bool>()
            {
                { NEW_CURSOR_HIDING_METHOD, true }
            };

            public static bool IsImplemented(string key)
            {
                if (!s_Features.ContainsKey(key))
                    return false;

                return s_Features[key];
            }
        }
    }
}
