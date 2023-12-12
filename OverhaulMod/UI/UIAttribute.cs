using System;

namespace OverhaulMod.UI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UIAttribute : Attribute
    {
        public bool FixOutlines;

        public UIAttribute(bool fixOutlines)
        {
            FixOutlines = fixOutlines;
        }
    }
}
