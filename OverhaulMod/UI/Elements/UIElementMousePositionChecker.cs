using UnityEngine.EventSystems;

namespace OverhaulMod.UI
{
    public class UIElementMousePositionChecker : OverhaulUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        public bool isMouseOverElement
        {
            get;
            private set;
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
            isMouseOverElement = false;
        }
    }
}
