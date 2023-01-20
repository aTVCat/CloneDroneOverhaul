using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneDroneOverhaul.V3.Base
{
    public struct SModSettingSliderSettings
    {
        public MinMaxRange Range { get; set; }
        public bool Int { get; set; }

        /// <summary>
        /// Must-use method to set float or int values up
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="useInt"></param>
        public SModSettingSliderSettings(in float minValue, in float maxValue, in bool useInt)
        {
            MinMaxRange range = new MinMaxRange();
            range.Max = maxValue;
            range.Min = minValue;
            Range = range;
            Int = useInt;
        }
    }
}
