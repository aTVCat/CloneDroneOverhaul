using UnityEngine;

namespace CDOverhaul.HUD
{
    public class OverhaulUIPanelScaler : OverhaulBehaviour
    {
        public float Multiplier = 10f;
        public Vector3 StartScale = Vector3.one * 1.05f;
        public Vector3 TargetScale = Vector3.one;

        private void Update()
        {
            float deltaTime = Time.unscaledDeltaTime * Multiplier;
            Vector3 scale = base.transform.localScale;
            scale.x = Mathf.Lerp(scale.x, TargetScale.x, deltaTime);
            scale.y = Mathf.Lerp(scale.y, TargetScale.y, deltaTime);
            scale.z = Mathf.Lerp(scale.z, TargetScale.z, deltaTime);
            base.transform.localScale = scale;
        }

        public override void OnEnable()
        {
            base.transform.localScale = StartScale;
        }
    }
}
