using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod.Engine
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ModSettingRequireRestartAttribute : Attribute
    {
    }
}
