using CloneDroneOverhaul.V3.HUD;
using UnityEngine;

namespace CloneDroneOverhaul.V3.Notifications
{
    public class UINotifications : V3_ModHUDBase
    {
        public static readonly Vector2 NotificationSize_Default = new Vector2(345, 160);
        public static readonly Vector2 NotificationSize_Small = new Vector2(345, 95);

        private UISpawnedNotification _prefab;
        private Transform _container;

        private bool _hasInitialized;

        private void Start()
        {
            _prefab = MyModdedObject.GetObjectFromList<ModdedObject>(0).gameObject.AddComponent<UISpawnedNotification>();
            _container = MyModdedObject.GetObjectFromList<Transform>(1);

            _hasInitialized = _prefab != null && _container != null && _prefab.Initialize();
        }

        public void ReceieveNotification(in SNotification notification)
        {
            _prefab.Populate(notification, _container);
        }
    }
}