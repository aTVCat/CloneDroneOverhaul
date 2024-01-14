using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod.Engine
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ModSettingAttribute : Attribute
    {
        public string Name;
        public object DefaultValue;
        public ModSetting.Tag Tag;

        public ModSettingAttribute(string name, object defaultValue, ModSetting.Tag tag = ModSetting.Tag.None)
        {
            Name = name;
            DefaultValue = defaultValue;
            Tag = tag;
        }
    }
}
