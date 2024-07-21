using System;

namespace OverhaulMod.UI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ShowTooltipOnHighLightAttribute : Attribute
    {
        public string Text;

        public float Duration;

        public bool TextIsLocalizationID;

        public ShowTooltipOnHighLightAttribute(string text, float duration, bool textIsLocalizationId = false)
        {
            Text = text;
            Duration = duration;
            TextIsLocalizationID = textIsLocalizationId;
        }

        public ShowTooltipOnHighLightAttribute(string text, bool textIsLocalizationId = false)
        {
            Text = text;
            Duration = 2f;
            TextIsLocalizationID = textIsLocalizationId;
        }
    }
}
