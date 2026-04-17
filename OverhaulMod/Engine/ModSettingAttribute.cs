using System;

namespace OverhaulMod.Engine
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ModSettingAttribute : Attribute
    {
        public string Name;
        public object DefaultValue;
        public ModSetting.Tags Tag;

        public ModSettingAttribute(string name, object defaultValue, ModSetting.Tags tag = ModSetting.Tags.None)
        {
            Name = name;
            DefaultValue = defaultValue;
            Tag = tag;
        }
    }
}
