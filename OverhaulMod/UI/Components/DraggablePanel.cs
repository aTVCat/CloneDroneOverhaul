using UnityEngine;
using UnityEngine.EventSystems;

namespace OverhaulMod.UI
{
    public class DraggablePanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private RectTransform m_RectTransform;

        private Vector2 m_DragOffset;

        private float m_width, m_height;

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

        public bool IsDragging
        {
            get;
            private set;
        }

        private void Start()
        {
            RectTransform rectTransform = base.GetComponent<RectTransform>();
            m_RectTransform = rectTransform;
            m_width = rectTransform.rect.width;
            m_height = rectTransform.rect.height;
        }

        private void Update()
        {
            updateDrag();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInitialized)
                return;

            m_DragOffset = UIManagerReference.GetUIRootAnchoredPositionFromMousePosition() - m_RectTransform.anchoredPosition;
            IsDragging = true;
        }
        public void OnPointerUp(PointerEventData eventData) => IsDragging = false;

        private void updateDrag()
        {
            if (!IsInitialized || !IsDragging)
                return;

            m_RectTransform.anchoredPosition = UIManagerReference.GetUIRootAnchoredPositionFromMousePosition() - m_DragOffset;
        }
    }
}
