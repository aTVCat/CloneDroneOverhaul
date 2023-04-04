using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul
{
    public class SettingEventDispatcher
    {
        public static readonly SettingEventDispatcher Type = new SettingEventDispatcher();

        public string EventString;
        public Func<bool> CanBeShown;

        public void DispatchEvent()
        {
            if (string.IsNullOrEmpty(EventString))
            {
                return;
            }
            OverhaulEventManager.DispatchEvent(EventString);
        }
    }
}
