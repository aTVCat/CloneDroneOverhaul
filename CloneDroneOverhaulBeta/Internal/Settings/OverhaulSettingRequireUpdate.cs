using System;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.All)]
    public class OverhaulSettingRequireUpdate : Attribute
    {
        public OverhaulVersion.Updates MinimumUpdateRequired;

        public bool ShouldBeVisible() => OverhaulVersion.IsUpdate(MinimumUpdateRequired);

        public OverhaulSettingRequireUpdate(OverhaulVersion.Updates update)
        {
            MinimumUpdateRequired = update;
        }
    }
}
