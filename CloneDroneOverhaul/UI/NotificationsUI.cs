using UnityEngine.UI;
using UnityEngine;
using CloneDroneOverhaul;
using System.Collections.Generic;

namespace CloneDroneOverhaul.UI.Notifications
{
    public class NotificationsUI : ModGUIBase
    {
        public static NotificationsUI Instance;
        private List<Notification> Messages;

        public override void OnAdded()
        {
            MyModdedObject = base.GetComponent<ModdedObject>();
            Messages = new List<Notification>();
            MyModdedObject.GetObjectFromList<RectTransform>(0).gameObject.SetActive(false);
            Instance = this;
        }

        public override void OnNewFrame()
        {
            MyModdedObject.GetObjectFromList<RectTransform>(1).gameObject.SetActive(!PhotoManager.Instance.IsInPhotoMode() && !CutSceneManager.Instance.IsInCutscene() && !Modules.CinematicGameManager.IsUIHidden);
            for (int i = Messages.Count - 1; i > -1; i--)
            {
                if (Time.unscaledTime >= Messages[i].TimeToDestroy)
                {
                    HideMessage(Messages[i]);
                }
            }
        }

        internal RectTransform RecieveMessage(Notification notification)
        {
            RectTransform transform = Instantiate<RectTransform>(MyModdedObject.GetObjectFromList<RectTransform>(0), MyModdedObject.GetObjectFromList<RectTransform>(1));
            transform.sizeDelta = notification.SizeDelta;
            transform.GetComponent<Image>().color = notification.Color;
            transform.SetAsFirstSibling();
            Messages.Add(notification);
            transform.gameObject.SetActive(true);
            ModdedObject mObj = transform.GetComponent<ModdedObject>();
            mObj.GetObjectFromList<Text>(0).text = notification.Title;
            mObj.GetObjectFromList<Text>(1).text = notification.Description;
            mObj.GetObjectFromList<RectTransform>(3).gameObject.SetActive(false);
            foreach (Notification.NotificationButton button in notification.Buttons)
            {
                mObj.GetObjectFromList<Text>(4).text = button.Text;
                RectTransform transform2 = Instantiate<RectTransform>(mObj.GetObjectFromList<RectTransform>(3), mObj.GetObjectFromList<RectTransform>(2));
                transform2.GetComponent<Button>().onClick.AddListener(button.Action);
                transform2.gameObject.SetActive(true);
                transform.GetComponent<Animator>().Play("NotificationAppear");
            }
            return transform;
        }

        internal void HideMessage(Notification notification)
        {
            UnityEngine.GameObject.Destroy(notification.InstanceRectTransform.gameObject);
            Messages.Remove(notification);
        }

    }

    public class Notification
    {
        public string Title { private set; get; }
        public string Description { private set; get; }
        public Vector2 SizeDelta { private set; get; }
        public Color Color { private set; get; }
        public NotificationButton[] Buttons { private set; get; }
        public float TimeToDestroy { private set; get; }
        public RectTransform InstanceRectTransform { private set; get; }

        public Notification SetUp(string title, string description, float unscaledTimeToLive, Vector2 sizeDelta, Color color, NotificationButton[] buttons)
        {
            Title = title;
            Description = description;
            SizeDelta = sizeDelta != Vector2.zero ? sizeDelta : new Vector2(450, 150);
            Color = color != Color.clear ? color : new Color(0.132743f, 0.1559941f, 0.1792453f, 0.8509804f);
            Buttons = buttons;
            TimeToDestroy = Time.unscaledTime + Mathf.Clamp(unscaledTimeToLive, 2.00f, 20.00f);
            InstanceRectTransform = NotificationsUI.Instance.RecieveMessage(this);
            return this;
        }

        public Notification()
        {

        }

        public void HideThis()
        {
            NotificationsUI.Instance.HideMessage(this);
        }

        public class NotificationButton
        {
            public string Text;
            public UnityEngine.Events.UnityAction Action;
        }
    }
}
