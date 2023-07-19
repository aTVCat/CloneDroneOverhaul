using UnityEngine;

namespace CDOverhaul.HUD.Tooltips
{
    public class OverhaulTooltipsUI : OverhaulUI
    {
        [OverhaulSettingRequireUpdate(OverhaulVersion.Updates.VER_3)]
        [OverhaulSetting("Game interface.Tooltips.Show tooltips", true)]
        public static bool ShowTooltips;

        [OverhaulSettingRequireUpdate(OverhaulVersion.Updates.VER_3)]
        [OverhaulSettingSliderParameters(false, -1f, 3f)]
        [OverhaulSetting("Game interface.Tooltips.Additional show duration", 0f, false, null, "Mod.Tooltips.Show tooltips")]
        public static float TooltipsAdditionalShowDuration;
    }
}
