using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod.UI.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ShowTooltipOnHighLightAttribute : Attribute
    {
        public string Text;

        public float Duration;

        public ShowTooltipOnHighLightAttribute(string text, float duration)
        {
            Text = text;
            Duration = duration;
        }

        public ShowTooltipOnHighLightAttribute(string text)
        {
            Text = text;
            Duration = 2f;
        }
    }
}
