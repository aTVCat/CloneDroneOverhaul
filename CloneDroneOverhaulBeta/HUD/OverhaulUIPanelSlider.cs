using UnityEngine;

namespace CDOverhaul.HUD
{
    public class OverhaulUIPanelSlider : OverhaulBehaviour
    {
        public float Multiplier = 15f;
        public Vector3 StartPosition = Vector3.zero + new Vector3(25f, 0f);
        public Vector3 TargetPosition = Vector3.zero;

        public int StopForFrames;
        private int m_StopFramesLeft;

        private void Update()
        {
            if (m_StopFramesLeft > 0)
            {
                m_StopFramesLeft--;
                return;
            }

            float deltaTime = Time.unscaledDeltaTime * Multiplier;
            Vector3 localPosition = base.transform.localPosition;
            localPosition.x = Mathf.Lerp(localPosition.x, TargetPosition.x, deltaTime);
            localPosition.y = Mathf.Lerp(localPosition.y, TargetPosition.y, deltaTime);
            localPosition.z = Mathf.Lerp(localPosition.z, TargetPosition.z, deltaTime);
            base.transform.localPosition = localPosition;
        }

        public override void OnEnable()
        {
            if (StopForFrames != 0)
            {
                m_StopFramesLeft = StopForFrames;
            }

            base.transform.localPosition = StartPosition;
        }
    }
}
