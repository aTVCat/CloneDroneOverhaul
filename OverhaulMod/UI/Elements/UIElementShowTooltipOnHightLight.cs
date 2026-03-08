using OverhaulMod.Utils;
using UnityEngine.EventSystems;

namespace OverhaulMod.UI
{
    public class UIElementShowTooltipOnHightLight : OverhaulUIBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        private bool _isMouseIn;

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
            if (_isMouseIn)
                ModUIUtils.Tooltip(textIsLocalizationId ? LocalizationManager.Instance.GetTranslatedString(tooltipText) : tooltipText, tooltipShowDuration);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            _isMouseIn = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isMouseIn = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isMouseIn = false;
        }

        public void OnSelect(BaseEventData eventData)
        {
            _isMouseIn = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isMouseIn = false;
        }
    }
}
