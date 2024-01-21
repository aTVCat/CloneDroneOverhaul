using UnityEngine;
using UnityEngine.EventSystems;

namespace OverhaulAPI.SharedMonoBehaviours
{
    /// <summary>
    /// Make UI panel draggable
    /// </summary>
    public class OverhaulDraggablePanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private RectTransform m_RectTransform;

        private bool m_HasInitialized;
        private Vector2 m_DragOffset;
        private UIManager m_UIManager;

        /// <summary>
        /// Check if we are dragging the UI element
        /// </summary>
        public bool IsDragging
        {
            get;
            private set;
        }

        private void Start()
        {
            m_RectTransform = base.GetComponent<RectTransform>();
            m_UIManager = UIManager.Instance;
            m_HasInitialized = m_RectTransform != null;
        }

        private void Update()
        {
            updateDrag();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!m_HasInitialized)
                return;

            Vector2 position = m_UIManager.GetUIRootAnchoredPositionFromMousePosition();
            m_DragOffset = position - m_RectTransform.anchoredPosition;
            IsDragging = true;
        }
        public void OnPointerUp(PointerEventData eventData) => IsDragging = false;

        private void updateDrag()
        {
            if (!m_HasInitialized || !IsDragging)
                return;

            Vector2 uirootAnchoredPositionFromMousePosition = m_UIManager.GetUIRootAnchoredPositionFromMousePosition();
            m_RectTransform.anchoredPosition = uirootAnchoredPositionFromMousePosition - m_DragOffset;
        }
    }
}