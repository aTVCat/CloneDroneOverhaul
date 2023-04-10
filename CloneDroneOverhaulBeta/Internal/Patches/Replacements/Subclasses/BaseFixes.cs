using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Patches
{
    public class BaseFixes : ReplacementBase
    {
        public override void Replace()
        {
            base.Replace();

            // Fix stripes on environment
            DirectionalLightManager.Instance.DirectionalLight.shadowNormalBias = 1.1f;
            DirectionalLightManager.Instance.DirectionalLight.shadowBias = 1f;

            // This may reduce RAM usage & improve performance a bit
            UnityEngine.Physics.reuseCollisionCallbacks = true;

            GameUIRoot.Instance.EmoteSelectionUI.GetComponent<Image>().enabled = false;

            LocalizationManager.Instance.SupportedLanguages[0].UIFont = LocalizationManager.Instance.SupportedLanguages[7].UIFont;
            LocalizationManager.Instance.SupportedLanguages[0].SubtitlesFont = LocalizationManager.Instance.SupportedLanguages[7].SubtitlesFont;
            LocalizationManager.Instance.SupportedLanguages[0].UIFontScale = 0.675f;

            if (!OverhaulVersion.Upd2Hotfix) HumanFactsManager.Instance.AddColor("Prototype", Color.white);

            foreach (Image image in Singleton<GameUIRoot>.Instance.GetComponentsInChildren<Image>(true))
            {
                if (image != null && image.sprite != null)
                {
                    if (image.sprite.name.Equals("UISprite") || image.sprite.name.Equals("Knob"))
                    {
                        image.sprite = AssetsController.GetAsset<Sprite>("CanvasDark-Small2-16x16", OverhaulAssetsPart.Part1);
                    }
                    else if (image.sprite.name.Equals("Checkmark"))
                    {
                        image.sprite = AssetsController.GetAsset<Sprite>("CheckmarkSmall", OverhaulAssetsPart.Part1);
                        image.color = Color.black;
                    }
                    else if (image.sprite.name.Equals("Background"))
                    {
                        image.sprite = AssetsController.GetAsset<Sprite>("CanvasBright-Small-16x16", OverhaulAssetsPart.Part1);
                    }
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
