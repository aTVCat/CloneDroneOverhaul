using System;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.All)]
    public class OverhaulSettingRequiredValue : OverhaulSettingBaseAttribute
    {
        public object TargetValue;

        public OverhaulSettingRequiredValue(object targetValue)
        {
            TargetValue = targetValue;
        }
    }
}
