using UnityEngine;
using UnityEngine.EventSystems;

namespace OverhaulMod.UI
{
    public class DraggablePanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private RectTransform _rectTransform;

        private Vector2 _DragOffset;

        private bool _goToFront;

        private UIManager _UIManager;
        private UIManager UIManagerReference
        {
            get
            {
                if (!_UIManager)
                    _UIManager = UIManager.Instance;

                return _UIManager;
            }
        }

        public bool IsInitialized => UIManagerReference && _rectTransform;

        public bool IsDragging
        {
            get;
            private set;
        }

        private void Start()
        {
            if (!_rectTransform)
                _rectTransform = base.GetComponent<RectTransform>();
        }

        private void Update()
        {
            updateDrag();
        }

        public void SetTransform(RectTransform rectTransform)
        {
            _rectTransform = rectTransform;
        }

        public void SetGoToFront(bool value)
        {
            _goToFront = value;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInitialized)
                return;

            _DragOffset = UIManagerReference.GetUIRootAnchoredPositionFromMousePosition() - _rectTransform.anchoredPosition;
            IsDragging = true;

            if (_goToFront)
                _rectTransform.SetAsLastSibling();
        }
        public void OnPointerUp(PointerEventData eventData) => IsDragging = false;

        private void updateDrag()
        {
            if (!IsInitialized || !IsDragging)
                return;

            _rectTransform.anchoredPosition = UIManagerReference.GetUIRootAnchoredPositionFromMousePosition() - _DragOffset;
        }
    }
}
