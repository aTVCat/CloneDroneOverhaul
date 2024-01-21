using System;
using System.Reflection;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MethodReferencingAttribute : Attribute
    {
        public MethodInfo MethodReference;
    }
}
