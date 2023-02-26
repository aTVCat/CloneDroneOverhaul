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


            foreach (Image image in Singleton<GameUIRoot>.Instance.GetComponentsInChildren<Image>(true))
            {
                if (image != null && image.sprite != null)
                {
                    if (image.sprite.name == "UISprite" || image.sprite.name == "Knob")
                    {
                        image.sprite = AssetController.GetAsset<Sprite>("CanvasDark-Small2-16x16", OverhaulAssetsPart.Part1);
                    }
                    if (image.sprite.name == "Checkmark")
                    {
                        image.sprite = AssetController.GetAsset<Sprite>("CheckmarkSmall", OverhaulAssetsPart.Part1);
                        image.color = Color.black;
                    }
                    if (image.sprite.name == "Background")
                    {
                        image.sprite = AssetController.GetAsset<Sprite>("CanvasBright-Small-16x16", OverhaulAssetsPart.Part1);
                    }
                    /*
                    Outline component = image.GetComponent<Outline>();
                    if (component != null)
                    {
                        component.enabled = false;
                    }*/
                }
            }

            SuccessfullyPatched = true;
        }

        public override void Cancel()
        {
            base.Cancel();
        }
    }
}
