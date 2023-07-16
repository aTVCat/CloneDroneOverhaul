namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class ModBotTagDisabler : OverhaulController
    {
        [OverhaulSettingRequireUpdate(OverhaulVersion.Updates.VER_3)]
        [OverhaulSetting("QoL.Multiplayer.Disable Mod-Bot tags", false, false, " ", null)]
        public static bool DisableTags;

        public override void Initialize()
        {

        }
    }
}
