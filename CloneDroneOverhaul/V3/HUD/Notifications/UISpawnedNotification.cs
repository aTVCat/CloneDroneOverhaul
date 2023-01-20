using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.V3.Notifications
{
    public class UISpawnedNotification : MonoBehaviour
    {
        private ModdedObject _myModdedObject;

        private Text _header;
        private Text _description;

        private Transform _buttonContainer;
        private ModdedObject _buttonPrefab;

        private Button _closeNotificationButton;

        private bool _isNotCloned;

        private float _timeLeft;

        internal bool Initialize(in bool isClone = false)
        {
            _isNotCloned = !isClone;
            _myModdedObject = base.GetComponent<ModdedObject>();

            if (!_myModdedObject)
            {
                return false;
            }

            _header = _myModdedObject.GetObjectFromList<Text>(0);
            _description = _myModdedObject.GetObjectFromList<Text>(1);

            _buttonContainer = _myModdedObject.GetObjectFromList<Transform>(2);
            _buttonPrefab = _myModdedObject.GetObjectFromList<ModdedObject>(3);

            _buttonPrefab.gameObject.SetActive(false);

            _closeNotificationButton = _myModdedObject.GetObjectFromList<Button>(5);

            base.gameObject.SetActive(false);

            return true;
        }

        public void Populate(SNotification notif, in Transform parent)
        {
            if (_isNotCloned)
            {
                UISpawnedNotification clone = Instantiate<UISpawnedNotification>(this, parent);
                clone.Initialize(true);
                clone._isNotCloned = false;
                notif.SpawnedNotification = clone;

                clone._header.text = notif.Header;
                clone._description.text = notif.Description;
                clone._closeNotificationButton.onClick.AddListener(clone.CloseNotification);
                clone._timeLeft = notif.Lifetime;
                clone.GetComponent<RectTransform>().sizeDelta = notif.Size;

                if (notif.Buttons != null)
                {
                    foreach (SNotificationButton buttons in notif.Buttons)
                    {
                        ModdedObject button = Instantiate<ModdedObject>(clone._buttonPrefab, clone._buttonContainer);
                        button.GetObjectFromList<Text>(0).text = buttons.Text;
                        if (buttons.Action != null)
                        {
                            button.GetComponent<Button>().onClick.AddListener(buttons.Action);
                        }

                        if (buttons.Icon != null)
                        {
                            button.GetObjectFromList<Image>(1).sprite = buttons.Icon;
                        }
                        else
                        {
                            button.GetObjectFromList<Text>(0).rectTransform.sizeDelta = Vector2.zero;
                            button.GetObjectFromList<Text>(0).rectTransform.anchoredPosition = Vector2.zero;
                            button.GetObjectFromList<Image>(1).enabled = false;
                        }

                        button.gameObject.SetActive(true);
                    }
                }

                clone.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (!_isNotCloned)
            {
                _timeLeft -= Time.deltaTime;
                if (_timeLeft <= 0f)
                {
                    CloseNotification();
                }
            }
        }

        public void CloseNotification()
        {
            if (!_isNotCloned)
            {
                Destroy(gameObject);
            }
        }
    }
}