using CDOverhaul.HUD;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace CDOverhaul.Visuals
{
    public class BloomOverhaulImageEffect : OverhaulCameraEffectBehaviour
    {
        [OverhaulSettingAttribute_Old("Graphics.Post effects.Set vanilla settings", OverhaulSettingsManager_Old.SettingEventDispatcherFlag, false, null, null)]
        public static OverhaulSettingWithEvent SetBloomVanilla = new OverhaulSettingWithEvent();

        [OverhaulSettingAttribute_Old("Graphics.Post effects.Enable bloom", true, false, "Make every light shine better", "Bloom.png")]
        public static bool BloomEnabled;

        [OverhaulSettingSliderParameters(true, 1, 10)]
        [OverhaulSettingAttribute_Old("Graphics.Post effects.Bloom iterations", 10, false, "How many times bloom effect should be applied?\n(Very low performance impact)", "Graphics.Post effects.Enable bloom")]
        public static int BloomIterations;

        [OverhaulSettingSliderParameters(false, 0.1f, 2f)]
        [OverhaulSettingAttribute_Old("Graphics.Post effects.Bloom intensity", 0.7f, false, null, "Graphics.Post effects.Enable bloom")]
        public static float BloomIntensity;

        [OverhaulSettingSliderParameters(false, 0.85f, 2f)]
        [OverhaulSettingAttribute_Old("Graphics.Post effects.Bloom Threshold", 1.25f, false, null, "Graphics.Post effects.Enable bloom")]
        public static float BloomThreshold;

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
