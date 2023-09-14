using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UIComponentButtonScaler : OverhaulBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        private bool m_Highlight;
        private bool m_Click;

        public Vector3 IdleScale = Vector3.one;
        public Vector3 HighlightedScale = new Vector3(1.02f, 1.02f, 1.02f);
        public Vector3 PressedScale = new Vector3(0.95f, 0.95f, 0.95f);

        public float Multiplier = 30f;

        private void Update()
        {
            m_Click = m_Highlight && Input.GetMouseButton(0);

            if (m_Click)
                updateScale(PressedScale);
            else if (m_Highlight)
                updateScale(HighlightedScale);
            else
                updateScale(IdleScale);
        }

        protected override void OnDisposed()
        {
            OverhaulDisposable.AssignNullToAllVars(this);
        }

        public override void OnDisable()
        {
            m_Click = false;
            m_Highlight = false;
            base.transform.localScale = IdleScale;
        }

        private void updateScale(Vector3 target)
        {
            float deltaTime = Time.unscaledDeltaTime * Multiplier;
            Vector3 scale = base.transform.localScale;
            scale.x = Mathf.Lerp(scale.x, target.x, deltaTime);
            scale.y = Mathf.Lerp(scale.y, target.y, deltaTime);
            scale.z = Mathf.Lerp(scale.z, target.z, deltaTime);
            base.transform.localScale = scale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_Highlight = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_Highlight = false;
        }

        public void OnPointerUp(PointerEventData eventData) => OnPointerExit(null);
    }
}
