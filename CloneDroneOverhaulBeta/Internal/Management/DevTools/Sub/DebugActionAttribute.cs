using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.DevTools
{
    public class DebugActionAttribute : Attribute
    {
        public string DisplayName;

        public DebugActionAttribute(string name)
        {
            DisplayName = name;
        }
    }
}
