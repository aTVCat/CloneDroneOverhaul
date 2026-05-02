using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace OverhaulMod.UI
{
    public class UIElementMouseEventsComponent : OverhaulUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerClickHandler
    {
        private float _timeForDoubleClick;

        public UnityAction<bool> PointerEnterStateCallback;

        public UnityAction DoubleClickCallback;

        public UnityAction ClickCallback;

        public bool IsMouseOverElement
        {
            get;
            private set;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (ClickCallback != null) ClickCallback();

            if (Time.unscaledTime < _timeForDoubleClick)
            {
                if (DoubleClickCallback != null) DoubleClickCallback();
                return;
            }
            _timeForDoubleClick = Time.unscaledTime + 0.25f;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsMouseOverElement = true;
            if (PointerEnterStateCallback != null) PointerEnterStateCallback(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsMouseOverElement = false;
            if (PointerEnterStateCallback != null) PointerEnterStateCallback(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsMouseOverElement = false;
            if (PointerEnterStateCallback != null) PointerEnterStateCallback(false);
        }
    }
}