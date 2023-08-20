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

        private Vector2 m_DragOffset;

        private UIManager m_UIManager;
        private UIManager UIManagerReference
        {
            get
            {
                if (!m_UIManager)
                    m_UIManager = UIManager.Instance;

                return m_UIManager;
            }
        }

        public bool IsInitialized => UIManagerReference && m_RectTransform;

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
        }

        private void Update()
        {
            updateDrag();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInitialized)
                return;

            Vector2 position = UIManagerReference.GetUIRootAnchoredPositionFromMousePosition();
            m_DragOffset = position - m_RectTransform.anchoredPosition;
            IsDragging = true;
        }
        public void OnPointerUp(PointerEventData eventData) => IsDragging = false;

        private void updateDrag()
        {
            if (!IsInitialized || !IsDragging)
                return;

            Vector2 uirootAnchoredPositionFromMousePosition = UIManagerReference.GetUIRootAnchoredPositionFromMousePosition();
            m_RectTransform.anchoredPosition = uirootAnchoredPositionFromMousePosition - m_DragOffset;
        }
    }
}