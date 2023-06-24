using System.Linq;

namespace CDOverhaul
{
    public class OverhaulFeatureDefinition
    {
        public OverhaulFeatureID FeatureID;
        public virtual bool IsAvailable() => false;

        public class AbilityToManageSkins : OverhaulFeatureDefinition
        {
            public override bool IsAvailable() => OverhaulVersion.IsDebugBuild || PlayFabDataController.IsDeveloper();
        }
    }
}
