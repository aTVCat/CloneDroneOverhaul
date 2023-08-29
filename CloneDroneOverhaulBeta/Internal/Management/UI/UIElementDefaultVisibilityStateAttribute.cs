using System;

namespace CDOverhaul
{
    public class UIElementDefaultVisibilityStateAttribute : Attribute
    {
        public bool ShouldBeActive;

        public UIElementDefaultVisibilityStateAttribute(bool value)
        {
            ShouldBeActive = value;
        }
    }
}
