using System;

namespace OverhaulMod.UI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UIElementActionAttribute : Attribute
    {
        public string Name;

        public bool UsePatch;

        public UIElementActionAttribute(string name, bool usePatch = false)
        {
            Name = name;
            UsePatch = usePatch;
        }
    }
}
