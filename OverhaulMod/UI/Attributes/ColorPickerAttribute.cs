using System;

namespace OverhaulMod.UI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ColorPickerAttribute : Attribute
    {
        public bool UseAlpha;

        public ColorPickerAttribute(bool useAlpha)
        {
            UseAlpha = useAlpha;
        }
    }
}
