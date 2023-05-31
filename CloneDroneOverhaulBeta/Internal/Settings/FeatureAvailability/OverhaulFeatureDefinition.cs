namespace CDOverhaul
{
    public class OverhaulFeatureDefinition
    {
        public OverhaulFeatureID FeatureID;
        public virtual bool IsAvailable() => false;

        public class AbilityToManageSkins : OverhaulFeatureDefinition
        {
            public override bool IsAvailable() => OverhaulVersion.IsDebugBuild || ExclusivityController.IsDeveloper() || ExclusivityController.GetDiscordID() == 606767773473046529;
        }
    }
}
