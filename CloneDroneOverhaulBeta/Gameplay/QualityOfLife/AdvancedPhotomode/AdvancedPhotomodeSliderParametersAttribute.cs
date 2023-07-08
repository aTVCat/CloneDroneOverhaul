using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
