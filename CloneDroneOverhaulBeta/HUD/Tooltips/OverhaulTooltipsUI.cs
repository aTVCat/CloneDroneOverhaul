namespace CDOverhaul.HUD.Tooltips
{
    public class OverhaulTooltipsUI : OverhaulUI
    {
        [OverhaulSetting("Game interface.Tooltips.Show tooltips", true)]
        public static bool ShowTooltips;

        [OverhaulSettingSliderParameters(false, -1f, 3f)]
        [OverhaulSetting("Game interface.Tooltips.Additional show duration", 0f, false, null, "Mod.Tooltips.Show tooltips")]
        public static float TooltipsAdditionalShowDuration;

        [OverhaulSetting("Game interface.Tooltips.Show player information", true, false, null, "Mod.Tooltips.Show tooltips")]
        public static bool ShowPlayerInfos;
    }
}
