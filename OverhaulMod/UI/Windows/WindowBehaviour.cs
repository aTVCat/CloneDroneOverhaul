using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI.Windows
{
    public class WindowBehaviour : OverhaulUIBehaviour
    {
        [UIElement("TitleBar")]
        private readonly GameObject _titleBar;

        [UIElement("TitleBarFrame")]
        private readonly GameObject _titleBarFrame;

        [UIElement("TitleText")]
        private readonly Text _titleText;

        [UIElementAction(nameof(Close))]
        [UIElement("CloseButton")]
        private readonly Button _closeButton;

        [UIElementAction(nameof(ToggleMinimized))]
        [UIElement("HideButton")]
        private readonly Button _hideButton;

        [UIElement("Content")]
        private readonly Transform _content;

        [UIElement("Shadow")]
        private readonly GameObject _strongShadow;

        [UIElement("BG")]
        private readonly Shadow _weakShadow;

        private DraggablePanel _draggablePanel;

        private UIElementMouseEventsComponent _mouseEvents;

        private RectTransform _rectTransform;

        private float _width, _height;

        private bool _minimized;
        public bool Minimized
        {
            get
            {
                return _minimized;
            }
            set
            {
                _minimized = value;
                setMinimized(value);
            }
        }

        public WindowHandle Handle;

        public bool DestroyOnClose;

        protected override void OnInitialized()
        {
            RectTransform rectTransform = base.transform as RectTransform;
            _rectTransform = rectTransform;

            DraggablePanel draggablePanel = _titleBar.AddComponent<DraggablePanel>();
            draggablePanel.SetTransform(rectTransform);
            draggablePanel.SetGoToFront(true);
            _draggablePanel = draggablePanel;

            UIElementMouseEventsComponent mouseEventsComponent = _titleBar.AddComponent<UIElementMouseEventsComponent>();
            mouseEventsComponent.DoubleClickCallback = ToggleMinimized;
            mouseEventsComponent.ClickCallback = SelectThis;
            _mouseEvents = mouseEventsComponent;

            GlobalEventManager.Instance.AddEventListener(WindowManager.WINDOW_SELECTED_EVENT, RefreshIsSelected);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            WindowManager.Instance.RemoveWindow(Handle);
            GlobalEventManager.Instance.RemoveEventListener(WindowManager.WINDOW_SELECTED_EVENT, RefreshIsSelected);
        }

        private void setMinimized(bool value)
        {
            Vector2 size = _rectTransform.sizeDelta;
            size.y = value ? 40f : _height;
            _rectTransform.sizeDelta = size;
            _titleBarFrame.SetActive(!value);
            _content.gameObject.SetActive(!value);
        }

        public void SetTitle(string text)
        {
            _titleText.text = text;
        }

        public void SetPivot(Vector2 pivot)
        {
            _rectTransform.pivot = pivot;
        }

        public void SetRect(Rect rect, float scaleFactor = 1f, float horizontalAnchor = 0f)
        {
            _rectTransform.anchoredPosition = new Vector2(rect.x + (rect.width * (1f - scaleFactor) * (1f - horizontalAnchor)), rect.y);
            _rectTransform.sizeDelta = new Vector2(rect.width, rect.height);

            _width = rect.width;
            _height = rect.height;
        }

        public void SetScale(float scale)
        {
            _rectTransform.localScale = Vector3.one * scale;
        }

        public void SetContents(Transform transform)
        {
            transform.gameObject.SetActive(true);
            transform.SetParent(_content);
            transform.localScale = Vector3.one;
            transform.localEulerAngles = Vector3.zero;
            transform.localPosition = Vector3.zero;

            if (transform is RectTransform rectTransform)
            {
                rectTransform.anchorMax = Vector2.one;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.pivot = Vector2.one * 0.5f;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }

        public void SetIsSelected(bool selected)
        {
            /*_weakShadow.enabled = !selected;
            _strongShadow.SetActive(selected);*/
        }

        public void RefreshIsSelected()
        {
            SetIsSelected(Handle.WindowID == WindowManager.Instance.GetSelectedWindowHandle().WindowID);
        }

        public void SelectThis()
        {
            WindowManager.Instance.SelectWindow(Handle);
        }

        public void ToggleMinimized()
        {
            Minimized = !Minimized;
        }

        public void Close()
        {
            if (!DestroyOnClose)
            {
                Hide();
                return;
            }
            Destroy(base.gameObject);
        }
    }
}