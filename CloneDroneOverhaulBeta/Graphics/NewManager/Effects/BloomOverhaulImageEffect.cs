using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace CDOverhaul.Graphics
{
    public class BloomOverhaulImageEffect : OverhaulImageEffectBehaviour
    {
        [OverhaulSetting("Graphics.Post effects.Set vanilla settings", OverhaulSettingsController.SettingEventDispatcherFlag, false, null, null)]
        public static OverhaulSettingWithEvent SetBloomVanilla = new OverhaulSettingWithEvent();

        [OverhaulSetting("Graphics.Post effects.Enable bloom", true, false, "Make every light shine better", "Bloom.png")]
        public static bool BloomEnabled;

        [OverhaulSettingSliderParameters(true, 1, 10)]
        [OverhaulSetting("Graphics.Post effects.Bloom iterations", 10, false, "How many times bloom effect should be applied?\n(Very low performance impact)", "Graphics.Post effects.Enable bloom")]
        public static int BloomIterations;

        [OverhaulSettingSliderParameters(false, 0.1f, 2f)]
        [OverhaulSetting("Graphics.Post effects.Bloom intensity", 0.7f, false, null, "Graphics.Post effects.Enable bloom")]
        public static float BloomIntensity;

        [OverhaulSettingSliderParameters(false, 0.85f, 2f)]
        [OverhaulSetting("Graphics.Post effects.Bloom Threshold", 1.25f, false, null, "Graphics.Post effects.Enable bloom")]
        public static float BloomThreshold;

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
