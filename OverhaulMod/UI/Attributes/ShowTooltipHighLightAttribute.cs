using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod.UI.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ShowTooltipHighLightAttribute : Attribute
    {
        public string Text;

        public float Duration;

        public ShowTooltipHighLightAttribute(string text, float duration)
        {
            Text = text;
            Duration = duration;
        }

        public ShowTooltipHighLightAttribute(string text)
        {
            Text = text;
            Duration = 2f;
        }
    }
}
