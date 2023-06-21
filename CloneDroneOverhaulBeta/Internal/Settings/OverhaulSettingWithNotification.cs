using System;

namespace CDOverhaul
{
    [AttributeUsage(AttributeTargets.All)]
    public class OverhaulSettingWithNotification : Attribute
    {
        public byte Type;

        public OverhaulSettingWithNotification(byte type)
        {
            Type = type;
        }
    }
}
