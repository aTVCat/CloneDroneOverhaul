using System;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.All)]
    public class OverhaulSettingRequiredValue : Attribute
    {
        public object TargetValue;

        public OverhaulSettingRequiredValue(object targetValue)
        {
            TargetValue = targetValue;
        }
    }
}
