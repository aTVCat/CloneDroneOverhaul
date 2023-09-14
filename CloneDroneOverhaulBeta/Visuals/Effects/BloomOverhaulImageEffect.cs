using CDOverhaul.HUD;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace CDOverhaul.Visuals
{
    public class BloomOverhaulImageEffect : OverhaulCameraEffectBehaviour
    {
        [OverhaulSettingAttribute_Old("Graphics.Post effects.Set vanilla settings", OverhaulSettingsManager_Old.SettingEventDispatcherFlag, false, null, null)]
        public static OverhaulSettingWithEvent SetBloomVanilla = new OverhaulSettingWithEvent();

        [OverhaulSetting(OverhaulSettingConstants.Categories.GRAPHICS, OverhaulSettingConstants.Sections.BLOOM, "Enable Bloom")]
        public static bool BloomEnabled = true;

        [OverhaulSettingSliderParameters(true, 1, 10)]
        [OverhaulSetting(OverhaulSettingConstants.Categories.GRAPHICS, OverhaulSettingConstants.Sections.BLOOM, "Bloom iterations")]
        public static int BloomIterations = 10;

        [OverhaulSettingSliderParameters(false, 0.1f, 2f)]
        [OverhaulSetting(OverhaulSettingConstants.Categories.GRAPHICS, OverhaulSettingConstants.Sections.BLOOM, "Bloom intensity")]
        public static float BloomIntensity = 0.7f;

        [OverhaulSettingSliderParameters(false, 0.85f, 2f)]
        [OverhaulSetting(OverhaulSettingConstants.Categories.GRAPHICS, OverhaulSettingConstants.Sections.BLOOM, "Bloom threshold")]
        public static float BloomThreshold = 1.25f;

        public override void Start()
        {
            base.Start();
            SetBloomVanilla.EventAction = delegate
            {
                OverhaulSettingInfo_Old.SavePref(OverhaulSettingsManager_Old.GetSetting("Graphics.Post effects.Enable bloom", true), true);
                OverhaulSettingInfo_Old.SavePref(OverhaulSettingsManager_Old.GetSetting("Graphics.Post effects.Bloom iterations", true), 2);
                OverhaulSettingInfo_Old.SavePref(OverhaulSettingsManager_Old.GetSetting("Graphics.Post effects.Bloom intensity", true), 0.5f);
                OverhaulSettingInfo_Old.SavePref(OverhaulSettingsManager_Old.GetSetting("Graphics.Post effects.Bloom Threshold", true), 0.9f);

                ParametersMenu menu = OverhaulController.Get<ParametersMenu>();
                if (menu && menu.gameObject.activeSelf)
                    menu.PopulateCategory(menu.SelectedCategory, true);
            };
        }

        public override void PatchCamera(Camera camera)
        {
            base.PatchCamera(camera);
            if (!camera)
                return;

            Bloom bloom = camera.GetComponent<Bloom>();
            if (!bloom)
                return;

            bloom.bloomBlurIterations = BloomEnabled ? BloomIterations : 1;
            bloom.bloomIntensity = BloomEnabled ? BloomIntensity : 0f;
            bloom.bloomThreshold = BloomThreshold;
        }
    }
}
