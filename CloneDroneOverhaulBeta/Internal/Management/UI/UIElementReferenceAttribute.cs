using System;

namespace CDOverhaul
{
    public class UIElementReferenceAttribute : Attribute
    {
        public string ObjectName;
        public int ObjectIndex;

        public bool UsesIndexes;

        public UIElementReferenceAttribute(string referenceName)
        {
            ObjectName = referenceName;
        }

        public UIElementReferenceAttribute(int index)
        {
            ObjectIndex = index;
            UsesIndexes = true;
        }
    }
}
