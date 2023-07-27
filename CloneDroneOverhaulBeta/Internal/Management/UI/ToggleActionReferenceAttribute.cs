using System;

namespace CDOverhaul
{
    public class ToggleActionReferenceAttribute : Attribute
    {
        public string MethodName;

        public ToggleActionReferenceAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}
