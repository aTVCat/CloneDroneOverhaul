using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulUISelectionOutline : OverhaulBehaviour, IPointerEnterHandler, IPointerExitHandler//, IPointerUpHandler
    {
        public float Multiplier = 20f;

        private Graphic m_Graphic;

        private bool m_Highlight;
        private bool m_Click;

        public void SetGraphic(Graphic graphic)
        {
            m_Graphic = graphic;
        }

        private void Update()
        {
            if (!m_Graphic)
            {
                m_Graphic = base.GetComponent<Image>();
                if (!m_Graphic)
                    return;
            }

            m_Click = m_Highlight && Input.GetMouseButton(0);

            Color currentColor = m_Graphic.color;
            currentColor.a = Mathf.Lerp(currentColor.a, m_Highlight ? (m_Click ? 0.35f : 0.75f) : 0f, Time.unscaledDeltaTime * Multiplier);
            m_Graphic.color = currentColor;
        }

        public override void OnDisable()
        {
            m_Click = false;
            m_Highlight = false;
        }

        protected override void OnDisposed() => OverhaulDisposable.AssignNullToAllVars(this);

        public void OnPointerEnter(PointerEventData eventData) => m_Highlight = true;
        public void OnPointerExit(PointerEventData eventData) => m_Highlight = false;
        //public void OnPointerUp(PointerEventData eventData) => OnPointerExit(null);
    }
}
