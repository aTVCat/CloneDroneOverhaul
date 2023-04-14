using System;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.All)]
    public class SettingInforms : Attribute
    {
        public byte Type;

        public SettingInforms(byte type)
        {
            Type = type;
        }
    }
}
