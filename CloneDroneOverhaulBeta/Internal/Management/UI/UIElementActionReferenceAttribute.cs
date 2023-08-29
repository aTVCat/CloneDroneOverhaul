using System;

namespace CDOverhaul
{
    public class UIElementActionReferenceAttribute : Attribute
    {
        public string[] MethodNames;

        public UIElementActionReferenceAttribute(string methodName)
        {
            MethodNames = new string[] { methodName };
        }

        public UIElementActionReferenceAttribute(string[] methodNames)
        {
            MethodNames = methodNames;
        }
    }
}
