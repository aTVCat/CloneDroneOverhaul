using System;

namespace OverhaulMod.UI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UIElementAttribute : Attribute
    {
        public string Name;

        public int Index = -1;

        public bool HasIndex() => Index != -1;

        public UIElementAttribute(string name)
        {
            Name = name;
        }

        public UIElementAttribute(int index)
        {
            Index = index;
        }
    }
}
