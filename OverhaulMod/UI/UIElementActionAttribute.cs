using System;

namespace OverhaulMod.UI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UIElementActionAttribute : Attribute
    {
        public string Name;

        public UIElementActionAttribute(string name)
        {
            Name = name;
        }
    }
}
