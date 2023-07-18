using UnityEngine;

namespace CDOverhaul.HUD
{
    public class OverhaulUIAnchoredPanelSlider : OverhaulBehaviour
    {
        public float Multiplier = 15f;
        public Vector3 StartPosition = Vector3.zero - new Vector3(25f, 0f);
        public Vector3 TargetPosition = Vector3.zero;

        private void Update()
        {
            RectTransform rectTransform = base.transform as RectTransform;

            float deltaTime = Time.unscaledDeltaTime * Multiplier;
            Vector3 position = rectTransform.anchoredPosition;
            position.x = Mathf.Lerp(position.x, TargetPosition.x, deltaTime);
            position.y = Mathf.Lerp(position.y, TargetPosition.y, deltaTime);
            position.z = Mathf.Lerp(position.z, TargetPosition.z, deltaTime);
            rectTransform.anchoredPosition = position;
        }

        public override void OnEnable()
        {
            (base.transform as RectTransform).anchoredPosition = StartPosition;
        }
    }
}
