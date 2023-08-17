using UnityEngine;

namespace CDOverhaul.HUD
{
    public class OverhaulUIAnchoredPanelSlider : OverhaulBehaviour
    {
        public float Multiplier = 15f;
        public Vector2 StartPosition = Vector2.zero - new Vector2(25f, 0f);
        public Vector2 TargetPosition;

        public int StopForFrames;
        private int m_StopFramesLeft;

        private void LateUpdate()
        {
            if (m_StopFramesLeft > 0)
            {
                m_StopFramesLeft--;
                return;
            }

            RectTransform rectTransform = base.transform as RectTransform;

            float deltaTime = Time.unscaledDeltaTime * Multiplier;
            Vector2 position = rectTransform.anchoredPosition;
            position.x = Mathf.Lerp(position.x, TargetPosition.x, deltaTime);
            position.y = Mathf.Lerp(position.y, TargetPosition.y, deltaTime);
            rectTransform.anchoredPosition = position;
        }

        public override void OnEnable()
        {
            if (StopForFrames != 0)
            {
                m_StopFramesLeft = StopForFrames;
            }

            (base.transform as RectTransform).anchoredPosition = StartPosition;
        }
    }
}
