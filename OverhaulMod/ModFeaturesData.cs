using System.Collections.Generic;

namespace OverhaulMod
{
    public class ModFeaturesData
    {
        public Dictionary<OverhaulMod.ModFeatures.FeatureType, bool> FeatureStates;

        public void FixValues()
        {
            if (FeatureStates == null)
                FeatureStates = new Dictionary<OverhaulMod.ModFeatures.FeatureType, bool>();
        }
    }
}
