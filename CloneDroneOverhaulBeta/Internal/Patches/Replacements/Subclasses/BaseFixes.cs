using Pathfinding.Util;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Patches
{
    public class BaseFixes : ReplacementBase
    {
        [SettingInforms(1)]
        [OverhaulSetting("Mod.Vanilla additions.\"Piksieli Prst\" font", true, false, "This font makes Overhaul's UI less differ from game UI")]
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
            ProjectileManager.Instance.ArrowPool.Prefab.GetComponent<Projectile>().VelocityMagnitude = 75f;

            if (m_OgUIFont == null) m_OgUIFont = LocalizationManager.Instance.SupportedLanguages[0].UIFont;
            if (m_OgSubtitlesFont == null) m_OgSubtitlesFont = LocalizationManager.Instance.SupportedLanguages[0].SubtitlesFont;
            if (m_OgFontScale == -1f) m_OgFontScale = LocalizationManager.Instance.SupportedLanguages[0].UIFontScale;
            SetEnglishFont(PixelsSimpleFont);

            foreach (Image image in Singleton<GameUIRoot>.Instance.GetComponentsInChildren<Image>(true))
            {
                if (image != null && image.sprite != null)
                {
                    if (image.sprite.name.Equals("UISprite") || image.sprite.name.Equals("Knob"))
                    {
                        image.sprite = OverhaulAssetsController.GetAsset<Sprite>("CanvasDark-Small2-16x16", OverhaulAssetPart.Part1);
                    }
                    else if (image.sprite.name.Equals("Checkmark"))
                    {
                        image.sprite = OverhaulAssetsController.GetAsset<Sprite>("CheckmarkSmall", OverhaulAssetPart.Part1);
                        image.color = Color.black;
                    }
                    else if (image.sprite.name.Equals("Background"))
                    {
                        image.sprite = OverhaulAssetsController.GetAsset<Sprite>("CanvasBright-Small-16x16", OverhaulAssetPart.Part1);
                    }
                }
            }

            SuccessfullyPatched = true;
        }

        public static void SetEnglishFont(bool piksielyPrst)
        {
            if (LocalizationManager.Instance == null || LocalizationManager.Instance.SupportedLanguages.IsNullOrEmpty())
                return;

            if (!piksielyPrst)
            {
                _ = LocalizationManager.Instance.SupportedLanguages[0].UIFont != m_OgUIFont;
                LocalizationManager.Instance.SupportedLanguages[0].UIFont = m_OgUIFont;
                LocalizationManager.Instance.SupportedLanguages[0].SubtitlesFont = m_OgSubtitlesFont;
                LocalizationManager.Instance.SupportedLanguages[0].UIFontScale = m_OgFontScale;
            }
            else
            {
                _ = LocalizationManager.Instance.SupportedLanguages[0].UIFont != LocalizationManager.Instance.SupportedLanguages[7].UIFont;
                LocalizationManager.Instance.SupportedLanguages[0].UIFont = LocalizationManager.Instance.SupportedLanguages[7].UIFont;
                LocalizationManager.Instance.SupportedLanguages[0].SubtitlesFont = LocalizationManager.Instance.SupportedLanguages[7].SubtitlesFont;
                LocalizationManager.Instance.SupportedLanguages[0].UIFontScale = 0.675f;
            }
        }
    }
}
