using TMPro;
using UnityEngine;

namespace CDOverhaul.HUD
{
    public class OverhaulUIPanelScaler : OverhaulBehaviour
    {
        public float Multiplier = 10f;
        public Vector3 StartScale = Vector3.one * 1.05f;
        public Vector3 TargetScale = Vector3.one;

        public int StopForFrames;
        private int m_StopFramesLeft;

        public void Initialize(Vector3 startScale, Vector3 targetScale, float multiplier = 15f, int stopFrames = 0)
        {
            StartScale = startScale;
            TargetScale = targetScale;
            Multiplier = multiplier;
            StopForFrames = stopFrames;
            m_StopFramesLeft = stopFrames;
        }

        private void LateUpdate()
        {
            if (m_StopFramesLeft > 0)
            {
                m_StopFramesLeft--;
                return;
            }

            float deltaTime = Time.unscaledDeltaTime * Multiplier;
            Vector3 scale = base.transform.localScale;
            scale.x = Mathf.Lerp(scale.x, TargetScale.x, deltaTime);
            scale.y = Mathf.Lerp(scale.y, TargetScale.y, deltaTime);
            scale.z = Mathf.Lerp(scale.z, TargetScale.z, deltaTime);
            base.transform.localScale = scale;
        }

        public override void OnEnable()
        {
            if (StopForFrames != 0)
            {
                m_StopFramesLeft = StopForFrames;
            }

            base.transform.localScale = StartScale;
        }
    }
}
