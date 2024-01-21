using System;
using System.Reflection;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FieldReferencingAttribute : Attribute
    {
        public FieldInfo FieldReference;
    }
}
