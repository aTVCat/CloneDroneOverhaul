using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MethodReferencingAttribute : Attribute
    {
        public MethodInfo MethodReference;
    }
}
