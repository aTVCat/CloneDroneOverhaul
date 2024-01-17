using System;

namespace OverhaulMod.UI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UIElementAttribute : Attribute
    {
        public string Name;

        public int Index = -1;

        public bool? DefaultActiveState;

        public Type ComponentToAdd;

        public bool HasIndex() => Index != -1;

        public UIElementAttribute(string name, bool enable)
        {
            Name = name;
            DefaultActiveState = enable;
        }

        public UIElementAttribute(string name)
        {
            Name = name;
        }

        public UIElementAttribute(string name, Type componentType)
        {
            Name = name;
            ComponentToAdd = componentType;
        }

        public UIElementAttribute(string name, Type componentType, bool enable)
        {
            Name = name;
            ComponentToAdd = componentType;
            DefaultActiveState = enable;
        }

        public UIElementAttribute(int index, bool enable)
        {
            Index = index;
            DefaultActiveState = enable;
        }

        public UIElementAttribute(int index)
        {
            Index = index;
        }
    }
}
