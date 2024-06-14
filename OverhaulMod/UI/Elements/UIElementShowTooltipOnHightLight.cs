using OverhaulMod.Utils;
using UnityEngine.EventSystems;

namespace OverhaulMod.UI
{
    public class UIElementShowTooltipOnHightLight : OverhaulUIBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        private bool m_isMouseIn;

        public string tooltipText
        {
            get;
            set;
        }

        public float tooltipShowDuration
        {
            get;
            set;
        } = 2f;

        public bool textIsLocalizationId
        {
            get;
            set;
        }

        public override void Update()
        {
            if (m_isMouseIn)
                ModUIUtils.Tooltip(textIsLocalizationId ? LocalizationManager.Instance.GetTranslatedString(tooltipText) : tooltipText, tooltipShowDuration);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            m_isMouseIn = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_isMouseIn = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_isMouseIn = false;
        }

        public void OnSelect(BaseEventData eventData)
        {
            m_isMouseIn = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_isMouseIn = false;
        }
    }
}
