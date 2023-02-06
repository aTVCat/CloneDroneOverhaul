using UnityEngine;
using UnityEngine.EventSystems;

namespace OverhaulAPI.SharedMonoBehaviours
{
    /// <summary>
    /// Make UI panel draggable
    /// </summary>
    public class OverhaulDraggableUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private RectTransform _myRectTransform;
        private bool _hasInitialized;

        /// <summary>
        /// Check if we are dragging the UI element
        /// </summary>
        public bool IsDragging { get; private set; }
        private Vector2 _dragOffset;

        private void Start()
        {
            _myRectTransform = base.GetComponent<RectTransform>();
            _hasInitialized = _myRectTransform != null;
        }

        private void Update()
        {
            DragIfPossible();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_hasInitialized)
            {
                return;
            }
            IsDragging = true;
            Vector2 uirootAnchoredPositionFromMousePosition = UIManager.Instance.GetUIRootAnchoredPositionFromMousePosition();
            _dragOffset = uirootAnchoredPositionFromMousePosition - _myRectTransform.anchoredPosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_hasInitialized)
            {
                return;
            }
            IsDragging = false;
        }

        public void DragIfPossible()
        {
            if (_hasInitialized && IsDragging)
            {
                Vector2 uirootAnchoredPositionFromMousePosition = UIManager.Instance.GetUIRootAnchoredPositionFromMousePosition();
                _myRectTransform.anchoredPosition = uirootAnchoredPositionFromMousePosition - _dragOffset;
            }
        }
    }
}