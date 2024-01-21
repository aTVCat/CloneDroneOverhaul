using System;

namespace CDOverhaul
{
    public class ObjectReferenceAttribute : Attribute
    {
        public string ObjectName;

        public ObjectReferenceAttribute(string referenceName)
        {
            ObjectName = referenceName;
        }
    }
}
