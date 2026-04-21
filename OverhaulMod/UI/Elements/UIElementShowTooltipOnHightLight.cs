using OverhaulMod.Utils;
using UnityEngine.EventSystems;

namespace OverhaulMod.UI
{
    public class UIElementShowTooltipOnHightLight : OverhaulUIBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        private bool _isMouseIn;

        public string TooltipText
        {
            get;
            set;
        }

        public float TooltipShowDuration
        {
            get;
            set;
        } = 2f;

        public bool TextIsLocalizationId
        {
            get;
            set;
        }

        public override void Update()
        {
            if (_isMouseIn)
                ModUIUtils.Tooltip(TextIsLocalizationId ? LocalizationManager.Instance.GetTranslatedString(TooltipText) : TooltipText, TooltipShowDuration);
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
