using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Patches
{
    public class BaseFixes : ReplacementBase
    {
        [SettingInforms(1)]
        [OverhaulSetting("Mod.Vanilla additions.\"Piksieli Prst\" font", false, false, "This font makes Overhaul's UI less differ from game UI")]
        public static bool PixelsSimpleFont;
        private static Font m_OgUIFont;
        private static Font m_OgSubtitlesFont;
        private static float m_OgFontScale = -1f;

        public override void Replace()
        {
            base.Replace();

            // Fix stripes on environment
            DirectionalLightManager.Instance.DirectionalLight.shadowNormalBias = 1.1f;
            DirectionalLightManager.Instance.DirectionalLight.shadowBias = 1f;

            // This may reduce RAM usage & improve performance a bit
            UnityEngine.Physics.reuseCollisionCallbacks = true;

            GameUIRoot.Instance.EmoteSelectionUI.GetComponent<Image>().enabled = false;

            if (m_OgUIFont == null) m_OgUIFont = LocalizationManager.Instance.SupportedLanguages[0].UIFont;
            if (m_OgSubtitlesFont == null) m_OgSubtitlesFont = LocalizationManager.Instance.SupportedLanguages[0].SubtitlesFont;
            if (m_OgFontScale == -1f) m_OgFontScale = LocalizationManager.Instance.SupportedLanguages[0].UIFontScale;
            SetEnglishFont(PixelsSimpleFont);

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

        public static void SetEnglishFont(bool piksielyPrst)
        {
            if (LocalizationManager.Instance == null || LocalizationManager.Instance.SupportedLanguages.IsNullOrEmpty())
            {
                return;
            }

            bool shouldRefresh = false;
            if (!piksielyPrst)
            {
                shouldRefresh = LocalizationManager.Instance.SupportedLanguages[0].UIFont != m_OgUIFont;
                LocalizationManager.Instance.SupportedLanguages[0].UIFont = m_OgUIFont;
                LocalizationManager.Instance.SupportedLanguages[0].SubtitlesFont = m_OgSubtitlesFont;
                LocalizationManager.Instance.SupportedLanguages[0].UIFontScale = m_OgFontScale;
            }
            else
            {
                shouldRefresh = LocalizationManager.Instance.SupportedLanguages[0].UIFont != LocalizationManager.Instance.SupportedLanguages[7].UIFont;
                LocalizationManager.Instance.SupportedLanguages[0].UIFont = LocalizationManager.Instance.SupportedLanguages[7].UIFont;
                LocalizationManager.Instance.SupportedLanguages[0].SubtitlesFont = LocalizationManager.Instance.SupportedLanguages[7].SubtitlesFont;
                LocalizationManager.Instance.SupportedLanguages[0].UIFontScale = 0.675f;
            }
        }
    }
}
