using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FieldReferencingAttribute : Attribute
    {
        public FieldInfo FieldReference;
    }
}
