using System;

namespace OverhaulMod.UI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UIElementAttribute : Attribute
    {
        public string Name;

        public bool? DefaultActiveState;

        public Type ComponentToAdd;

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
    }
}
