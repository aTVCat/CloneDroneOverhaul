using OverhaulMod.Visuals;

namespace OverhaulMod
{
    /// <summary>
    /// Used by other mods to toggle specific features
    /// </summary>
    public static class GlobalDefinitions
    {
        private static bool s_disableOverhaulPostEffects;
        /// <summary>
        /// Disables all effects added by Overhaul
        /// </summary>
        public static bool DisableOverhaulPostEffects
        {
            get => s_disableOverhaulPostEffects;
            set
            {
                s_disableOverhaulPostEffects = value;
                if (PostEffectsManager.Instance) PostEffectsManager.Instance.RefreshCameraPostEffectsNextFrame();
            }
        }

        private static bool s_disableBloomChanges;
        /// <summary>
        /// Disable bloom effect updates. If set to <see langword="true"/>, bloom settings are never changed
        /// </summary>
        public static bool DisableBloomChanges
        {
            get => s_disableBloomChanges;
            set
            {
                s_disableBloomChanges = value;
                if (PostEffectsManager.Instance) PostEffectsManager.Instance.RefreshCameraPostEffectsNextFrame();
            }
        }
    }
}
