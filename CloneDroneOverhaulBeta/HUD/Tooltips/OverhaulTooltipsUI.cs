namespace CDOverhaul.HUD.Tooltips
{
    public class OverhaulTooltipsUI : OverhaulUI
    {
        [OverhaulSettingAttribute_Old("Game interface.Tooltips.Show tooltips", true)]
        public static bool ShowTooltips;

        [OverhaulSettingSliderParameters(false, -1f, 3f)]
        [OverhaulSettingAttribute_Old("Game interface.Tooltips.Additional show duration", 0f, false, null, "Game interface.Tooltips.Show tooltips")]
        public static float TooltipsAdditionalShowDuration;

        [OverhaulSettingAttribute_Old("Game interface.Tooltips.Show player information", true, false, null, "Game interface.Tooltips.Show tooltips")]
        public static bool ShowPlayerInfos;
    }
}
