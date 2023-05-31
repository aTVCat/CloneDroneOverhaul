using System;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.All)]
    public class SettingSliderParameters : Attribute
    {
        public bool UseWholeNumbers;
        public float Min;
        public float Max;

        public SettingSliderParameters(bool isInt, float min, float max)
        {
            UseWholeNumbers = isInt;
            Min = min;
            Max = max;
        }
    }
}
