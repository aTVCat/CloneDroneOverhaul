using UnityEngine;
using UnityEngine.EventSystems;

namespace CDOverhaul.HUD
{
    /// <summary>
    /// Make UI panel draggable
    /// </summary>
    public class DraggableUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private bool _isDragging;
        private Vector2 _dragOffset;
        private RectTransform _myRectTransform;

        private bool _hasInitialized;

        private void Start()
        {
            _myRectTransform = base.GetComponent<RectTransform>();
            _hasInitialized = true;
        }

        private void Update()
        {
            if (_isDragging)
            {
                Vector2 uirootAnchoredPositionFromMousePosition = UIManager.Instance.GetUIRootAnchoredPositionFromMousePosition();
                _myRectTransform.anchoredPosition = uirootAnchoredPositionFromMousePosition - _dragOffset;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_hasInitialized)
            {
                return;
            }
            _isDragging = true;
            Vector2 uirootAnchoredPositionFromMousePosition = UIManager.Instance.GetUIRootAnchoredPositionFromMousePosition();
            _dragOffset = uirootAnchoredPositionFromMousePosition - _myRectTransform.anchoredPosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_hasInitialized)
            {
                return;
            }
            _isDragging = false;
        }
    }
}