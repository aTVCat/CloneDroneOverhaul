using System;

namespace CDOverhaul
{
    public class OverhaulSettingWithEvent
    {
        public static readonly OverhaulSettingWithEvent Type = new OverhaulSettingWithEvent();

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
            OverhaulEventsController.DispatchEvent(EventString);
        }
    }
}
