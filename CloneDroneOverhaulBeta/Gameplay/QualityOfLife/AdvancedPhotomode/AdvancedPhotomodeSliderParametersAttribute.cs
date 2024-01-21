using System;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class AdvancedPhotomodeSliderParametersAttribute : Attribute
    {
        public float MinValue;
        public float MaxValue;

        public AdvancedPhotomodeSliderParametersAttribute(float min, float max)
        {
            MinValue = min;
            MaxValue = max;
        }
    }
}
