using System;

namespace CDOverhaul
{
    public class ButtonActionReferenceAttribute : Attribute
    {
        public string MethodName;

        public ButtonActionReferenceAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}
