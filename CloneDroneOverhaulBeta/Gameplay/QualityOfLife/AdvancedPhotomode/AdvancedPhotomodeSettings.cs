using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public static class AdvancedPhotomodeSettings
    {
        [AdvancedPhotomodeSetting("Override parameters", "General")]
        public static bool OverrideSettings;
        public static bool IsOverridingSettings => OverrideSettings && AdvancedPhotomodeController.Instance.PhotoManager.IsInPhotoMode();

        [AdvancedPhotomodeSetting("Enable", "Ambient Occlusion")]
        public static bool SSAOEnable;
        [AdvancedPhotomodeSetting("Intensity", "Ambient Occlusion")]
        public static float SSAOIntensity;
        [AdvancedPhotomodeSetting("Intensity", "Ambient Occlusion")]
        public static int SSAOSampleCount;
    }
}
