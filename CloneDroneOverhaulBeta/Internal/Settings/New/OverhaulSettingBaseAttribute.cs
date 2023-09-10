using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class OverhaulSettingBaseAttribute : Attribute
    {
    }
}
