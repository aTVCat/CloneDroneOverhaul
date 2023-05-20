using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulFeatureAvailabilitySystem
    {
        private static ReadOnlyCollection<OverhaulFeatureDefinition> m_Features = new ReadOnlyCollection<OverhaulFeatureDefinition>(new List<OverhaulFeatureDefinition>()
        {
            CreateNew(OverhaulFeatureID.PermissionToManageSkins)
        });

        private static OverhaulFeatureDefinition CreateNew(OverhaulFeatureID id)
        {
            OverhaulFeatureDefinition def = null;
            switch (id)
            {
                case OverhaulFeatureID.PermissionToManageSkins:
                    def = new OverhaulFeatureDefinition.AbilityToManageSkins();
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
            {
                return true;
            }

            OverhaulFeatureDefinition def = GetFeatureDefinition(featureID);
            return def != null && def.IsAvailable();
        }

        public static OverhaulFeatureDefinition GetFeatureDefinition(in OverhaulFeatureID featureID)
        {
            if (m_Features.IsNullOrEmpty())
            {
                return null;
            }

            int i = 0;
            do
            {
                OverhaulFeatureDefinition def = m_Features[i];
                if (def != null && def.FeatureID == featureID)
                {
                    return def;
                }
                i++;
            } while (i < m_Features.Count);
            return null;
        }

        public static string GetStringOfFeature(in OverhaulFeatureID featureID)
        {
            switch (featureID)
            {
                case OverhaulFeatureID.PermissionToManageSkins:
                    return "Permission to manage skins"; // Todo: Translate all
                case OverhaulFeatureID.PermissionToEditLocalization:
                    return "Permission to translate the mod";
            }
            return "Unknown feature (" + featureID.ToString() + ")";
        }

        public static string GetColoredStringOfFeature(in OverhaulFeatureID featureID)
        {
            return GetStringOfFeature(featureID).AddColor(IsFeatureUnlocked(featureID) ? Color.white : Color.gray);
        }

        /// <summary>
        /// Features that doesn't require player IDs to work. This class defines what things should be included in the build
        /// </summary>
        public static class BuildImplements
        {
            /// <summary>
            /// Level editor selection outline
            /// </summary>
            private static bool IS_SELECTION_OUTLINE_ENABLED = true;
            public static bool IsSelectionOutLineEnabled => OverhaulVersion.IsUpdate4 && IS_SELECTION_OUTLINE_ENABLED;

            /// <summary>
            /// VFX that plays when switching skins
            /// </summary>
            private static bool IS_SKIN_SWITCHING_VFX_ENABLED = true;
            public static bool IsSkinSwitchingVFXEnabled => !OverhaulVersion.IsUpdate2Hotfix && IS_SKIN_SWITCHING_VFX_ENABLED;
        }
    }
}
