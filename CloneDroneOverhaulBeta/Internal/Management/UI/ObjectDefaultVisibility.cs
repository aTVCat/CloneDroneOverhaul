using System;

namespace CDOverhaul
{
    public class ObjectDefaultVisibility : Attribute
    {
        public bool ShouldBeActive;

        public ObjectDefaultVisibility(bool value)
        {
            ShouldBeActive = value;
        }
    }
}
