using System;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.All)]
    public class OverhaulSettingSliderParameters : OverhaulSettingBaseAttribute
    {
        public bool UseWholeNumbers;
        public float Min;
        public float Max;

        public OverhaulSettingSliderParameters(bool isInt, float min, float max)
        {
            UseWholeNumbers = isInt;
            Min = min;
            Max = max;
        }
    }
}
