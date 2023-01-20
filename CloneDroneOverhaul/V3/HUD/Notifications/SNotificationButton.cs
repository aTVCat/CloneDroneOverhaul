using UnityEngine;


namespace CloneDroneOverhaul.V3.Notifications
{
    public struct SNotificationButton
    {
        public string Text { get; set; }
        public Sprite Icon { get; set; }
        public UnityEngine.Events.UnityAction Action { get; set; }

        public SNotification MyNotification { get; set; }

        public SNotificationButton(in string text, in Sprite sprite)
        {
            Text = text;
            Icon = sprite;
            MyNotification = default(SNotification);
            Action = null;
        }

        public void SetAction(in System.Action action)
        {
            Action = new UnityEngine.Events.UnityAction(action);
        }

        public void Action_Close()
        {
            MyNotification.Close();
        }
    }
}
