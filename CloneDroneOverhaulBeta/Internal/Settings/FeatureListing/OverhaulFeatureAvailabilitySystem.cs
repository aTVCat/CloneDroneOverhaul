using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            OverhaulFeatureDefinition def = GetFeatureDefinition(featureID);
            if(def == null)
            {
                return false;
            }

            return def.IsAvailable();
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
                if(def != null && def.FeatureID == featureID)
                {
                    return def;
                }
                i++;
            } while (i < m_Features.Count);
            return null;
        }
    }
}
