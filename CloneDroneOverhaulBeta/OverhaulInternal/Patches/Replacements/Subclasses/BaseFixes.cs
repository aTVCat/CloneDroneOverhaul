using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Patches
{
    public class BaseFixes : ReplacementBase
    {
        public override void Replace()
        {
            base.Replace();

            // Fix lines on environment
            DirectionalLightManager.Instance.DirectionalLight.shadowNormalBias = 1.1f;
            DirectionalLightManager.Instance.DirectionalLight.shadowBias = 1f;

            // This may reduce RAM usage & improve performacne a bit
            UnityEngine.Physics.reuseCollisionCallbacks = true;

            GameUIRoot.Instance.EmoteSelectionUI.GetComponent<Image>().enabled = false;

            SuccessfullyPatched = true;
        }

        public override void Cancel()
        {
            base.Cancel();
        }
    }
}
