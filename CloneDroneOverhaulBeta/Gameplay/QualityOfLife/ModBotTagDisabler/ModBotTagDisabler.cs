namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class ModBotTagDisabler : OverhaulController
    {
        [OverhaulSetting("QoL.Multiplayer.Disable Mod-Bot tags", false)]
        public static bool DisableTags;

        public override void Initialize()
        {

        }
    }
}
