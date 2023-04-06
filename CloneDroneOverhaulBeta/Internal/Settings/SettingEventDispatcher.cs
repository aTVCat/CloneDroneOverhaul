using System;

namespace CDOverhaul
{
    public class SettingEventDispatcher
    {
        public static readonly SettingEventDispatcher Type = new SettingEventDispatcher();

        public string EventString;
        public Func<bool> CanBeShown;

        public Action EventAction;

        public void DispatchEvent()
        {
            if (string.IsNullOrEmpty(EventString))
            {
                EventAction?.Invoke();
                return;
            }
            OverhaulEventManager.DispatchEvent(EventString);
        }
    }
}
