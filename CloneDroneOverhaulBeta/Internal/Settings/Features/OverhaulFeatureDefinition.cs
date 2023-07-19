using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace CDOverhaul
{
    public class OverhaulFeatureDefinition
    {
        public OverhaulFeatureID FeatureID;
        public virtual bool IsAvailable() => false;

        public class AbilityToManageSkins : OverhaulFeatureDefinition
        {
            public override bool IsAvailable() => OverhaulVersion.IsDebugBuild || OverhaulPlayerIdentifier.IsDeveloper();
        }

        public class AbilityToCopyUserInfos : OverhaulFeatureDefinition
        {
            public static readonly ReadOnlyCollection<string> AllowedUsers = new ReadOnlyCollection<string>(new List<string>()
            {
                "76561199014733748",
                "76561198416093982"
            });

            public override bool IsAvailable() => OverhaulVersion.IsDebugBuild || OverhaulPlayerIdentifier.IsDeveloper() || AllowedUsers.Contains(OverhaulPlayerIdentifier.GetLocalSteamID());
        }
    }
}
