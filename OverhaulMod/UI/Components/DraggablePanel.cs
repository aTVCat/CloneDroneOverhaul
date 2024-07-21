using UnityEngine;
using UnityEngine.EventSystems;

namespace OverhaulMod.UI
{
    public class DraggablePanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private RectTransform m_rectTransform;

        private Vector2 m_DragOffset;

        private bool m_goToFront;

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

        public bool IsInitialized => UIManagerReference && m_rectTransform;

        public bool IsDragging
        {
            get;
            private set;
        }

        private void Start()
        {
            if (!m_rectTransform)
                m_rectTransform = base.GetComponent<RectTransform>();
        }

        private void Update()
        {
            updateDrag();
        }

        public void SetTransform(RectTransform rectTransform)
        {
            m_rectTransform = rectTransform;
        }

        public void SetGoToFront(bool value)
        {
            m_goToFront = value;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInitialized)
                return;

            m_DragOffset = UIManagerReference.GetUIRootAnchoredPositionFromMousePosition() - m_rectTransform.anchoredPosition;
            IsDragging = true;

            if (m_goToFront)
                m_rectTransform.SetAsLastSibling();
        }
        public void OnPointerUp(PointerEventData eventData) => IsDragging = false;

        private void updateDrag()
        {
            if (!IsInitialized || !IsDragging)
                return;

            m_rectTransform.anchoredPosition = UIManagerReference.GetUIRootAnchoredPositionFromMousePosition() - m_DragOffset;
        }
    }
}
