using UnityEngine;


namespace CloneDroneOverhaul.V3Tests.Notifications
{
    public struct SNotification
    {
        public string Header { get; set; }
        public string Description { get; set; }
        public float Lifetime { get; set; }
        public Vector2 Size { get; set; }
        public SNotificationButton[] Buttons { get; set; }

        public UISpawnedNotification SpawnedNotification { get; set; }

        public SNotification(in string header, in string description, in float lifetime, in Vector2 size, SNotificationButton[] buttons = null)
        {
            Header = header;
            Description = description;
            Lifetime = Mathf.Clamp(lifetime, 2f, 20f);
            Size = size;
            Buttons = buttons;
            SpawnedNotification = null;

            if(Buttons != null)
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].MyNotification = this;
            }
        }

        public void Send()
        {
            if (SpawnedNotification != null)
            {
                return;
            }
            UINotifications.GetInstance<UINotifications>().ReceieveNotification(this);
        }

        public void Close()
        {
            if (SpawnedNotification != null)
            {
                SpawnedNotification.CloseNotification();
            }
        }
    }
}
