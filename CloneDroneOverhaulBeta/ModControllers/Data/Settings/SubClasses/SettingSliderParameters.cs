using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.All)]
    public class SettingSliderParameters : Attribute
    {
        public bool IsInt
        {
            get;
            set;
        }

        public float Min
        {
            get;
            set;
        }

        public float Max
        {
            get;
            set;
        }

        public SettingSliderParameters(bool isInt, float min, float max)
        {
            IsInt = isInt;
            Min = min;
            Max = max;
        }
    }
}
