using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
