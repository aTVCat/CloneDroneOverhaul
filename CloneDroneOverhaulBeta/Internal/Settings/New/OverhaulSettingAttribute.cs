using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class OverhaulSettingAttribute : Attribute
    {
        public string Path;

        public bool IsHidden;

        public OverhaulSettingAttribute(string path, bool hidden = false)
        {
            Path = path;
            IsHidden = hidden;
        }

        public OverhaulSettingAttribute(string category, string section, string name, bool hidden = false)
        {
            Path = category + "." + section + "." + name;
            IsHidden = hidden;
        }
    }
}
