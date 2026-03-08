using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace OverhaulMod.UI
{
    public class UIElementMouseEventsComponent : OverhaulUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerClickHandler
    {
        private float _timeForDoubleClick;

        public bool isMouseOverElement
        {
            get;
            private set;
        }

        public UnityAction<bool> pointerEnterStateCallback
        {
            get;
            set;
        }

        public UnityAction doubleClickCallback
        {
            get;
            set;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Time.unscaledTime < _timeForDoubleClick)
            {
                doubleClickCallback?.Invoke();
                return;
            }
            _timeForDoubleClick = Time.unscaledTime + 0.25f;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isMouseOverElement = true;
            pointerEnterStateCallback?.Invoke(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isMouseOverElement = false;
            pointerEnterStateCallback?.Invoke(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isMouseOverElement = false;
            pointerEnterStateCallback?.Invoke(false);
        }
    }
}
