using System;

namespace CDOverhaul
{
    public class UIElementComponentsAttribute : Attribute
    {
        public Type[] Components;

        public UIElementComponentsAttribute(Type[] components)
        {
            Components = components;
        }
    }
}
