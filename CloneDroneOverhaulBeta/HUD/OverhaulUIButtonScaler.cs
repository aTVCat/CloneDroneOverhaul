using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulUIButtonScaler : OverhaulBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        public Vector3 IdleScale = Vector3.one;
        public Vector3 HighlightedScale = new Vector3(1.02f, 1.02f, 1.02f);
        public Vector3 PressedScale = new Vector3(0.95f, 0.95f, 0.95f);

        public float Multiplier = 30f;

        public Button LocalButton;
        private Transform m_TargetTransform;

        private bool m_Highlight;
        private bool m_Click;

        private void Update()
        {
            if (!LocalButton)
            {
                LocalButton = GetComponent<Button>();
                if (!LocalButton)
                    return;
            }

            if (!m_TargetTransform)
                m_TargetTransform = LocalButton.transform;

            m_Click = m_Highlight && Input.GetMouseButton(0);

            if (m_Click)
                updateScale(PressedScale);
            else if (m_Highlight)
                updateScale(HighlightedScale);
            else
                updateScale(IdleScale);

            m_TargetTransform.localScale = new Vector3(limitCoord(0), limitCoord(1), limitCoord(2));
        }

        protected override void OnDisposed()
        {
            OverhaulDisposable.AssignNullToAllVars(this);
        }

        public override void OnDisable()
        {
            m_Click = false;
            m_Highlight = false;
            if (m_TargetTransform) m_TargetTransform.localScale = IdleScale;
        }

        private void updateScale(Vector3 target)
        {
            m_TargetTransform.localScale += (target - m_TargetTransform.localScale) * Multiplier * Time.unscaledDeltaTime;
        }

        private float limitCoord(byte number)
        {
            return Mathf.Clamp(m_TargetTransform.localScale[number], PressedScale[number], HighlightedScale[number]);
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
