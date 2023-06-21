using System.Linq;

namespace CDOverhaul
{
    public class OverhaulFeatureDefinition
    {
        public OverhaulFeatureID FeatureID;
        public virtual bool IsAvailable() => false;

        public class AbilityToManageSkins : OverhaulFeatureDefinition
        {
            private static readonly long[] s_AllowedDiscordIDs = new long[]
            {
                606767773473046529
            };

            public override bool IsAvailable() => OverhaulVersion.IsDebugBuild || ExclusivityController.IsDeveloper() || s_AllowedDiscordIDs.Contains(ExclusivityController.GetDiscordID());
        }
    }
}
