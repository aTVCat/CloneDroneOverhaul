using System;

namespace CDOverhaul
{
    public class ActionReferenceAttribute : Attribute
    {
        public string[] MethodNames;

        public ActionReferenceAttribute(string methodName)
        {
            MethodNames = new string[] { methodName };
        }

        public ActionReferenceAttribute(string[] methodNames)
        {
            MethodNames = methodNames;
        }
    }
}
