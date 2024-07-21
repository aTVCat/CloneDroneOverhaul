using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace OverhaulMod.UI
{
    public class UIElementMouseEventsComponent : OverhaulUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerClickHandler
    {
        private float m_timeForDoubleClick;

        public bool isMouseOverElement
        {
            get;
            private set;
        }

        public UnityAction doubleClickAction
        {
            get;
            set;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Time.unscaledTime < m_timeForDoubleClick)
            {
                doubleClickAction?.Invoke();
                return;
            }
            m_timeForDoubleClick = Time.unscaledTime + 0.25f;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isMouseOverElement = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isMouseOverElement = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //isMouseOverElement = false;
        }
    }
}
