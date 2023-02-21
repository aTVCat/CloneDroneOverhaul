using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Patches
{
    public class BaseFixes : ReplacementBase
    {
        private StoryChapter3AllyFix _allyFix;

        public override void Replace()
        {
            base.Replace();

            // Fix lines on environment
            DirectionalLightManager.Instance.DirectionalLight.shadowNormalBias = 1.1f;
            DirectionalLightManager.Instance.DirectionalLight.shadowBias = 1f;

            // This may reduce RAM usage & improve performacne a bit
            UnityEngine.Physics.reuseCollisionCallbacks = true;

            GameUIRoot.Instance.EmoteSelectionUI.GetComponent<Image>().enabled = false;

            GameObject obj = new GameObject("Story3_AllyFix");
            obj.transform.SetParent(OverhaulMod.Core.transform);
            _allyFix = obj.AddComponent<StoryChapter3AllyFix>();

            SuccessfullyPatched = true;
        }

        public StoryChapter3AllyFix GetAllyFix()
        {
            return _allyFix;
        }

        public override void Cancel()
        {
            base.Cancel();
        }
    }
}
