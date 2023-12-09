using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod.UI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UIAttribute : Attribute
    {
        public bool FixOutlines;

        public UIAttribute(bool fixOutlines)
        {
            FixOutlines = fixOutlines;
        }
    }
}
