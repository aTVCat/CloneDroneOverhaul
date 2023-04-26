using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul
{
    public class OverhaulFeatureDefinition
    {
        public OverhaulFeatureID FeatureID
        {
            get;
            set;
        }

        public virtual bool IsAvailable()
        {
            return false;
        }

        public class AbilityToManageSkins : OverhaulFeatureDefinition
        {
            public override bool IsAvailable()
            {
                return OverhaulVersion.IsDebugBuild || ExclusivityController.IsDeveloper() || ExclusivityController.GetDiscordID() == 606767773473046529;
            }
        }
    }
}
