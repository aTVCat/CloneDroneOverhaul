using System;

namespace CDOverhaul.DevTools
{
    public class DebugActionAttribute : Attribute
    {
        public string DisplayName;

        public DebugActionAttribute(string name)
        {
            DisplayName = name;
        }
    }
}
